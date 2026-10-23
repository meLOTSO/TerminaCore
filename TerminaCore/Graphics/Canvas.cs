using System;
using System.Collections.Generic;

namespace TerminaCore.Graphics;

public class Canvas
{
    private readonly Texture _initialState;
    private readonly Texture _renderCanvas;

    // Буферы цветов для каждой ячейки холста
    private readonly ConsoleColor?[] _fgColorBuffer;
    private readonly ConsoleColor?[] _bgColorBuffer;

    public char BackgroundChar { get; set; } = ' ';
    public ConsoleColor DefaultForeground { get; set; } = ConsoleColor.White;
    public ConsoleColor DefaultBackground { get; set; } = ConsoleColor.Black;

    public int Width => _renderCanvas.Width;
    public int Height => _renderCanvas.Height;

    public List<Texture> Textures { get; set; } = [];

    public Canvas(int width, int height, char backgroundChar = ' ',
                  ConsoleColor defaultFg = ConsoleColor.White, ConsoleColor defaultBg = ConsoleColor.Black)
    {
        BackgroundChar = backgroundChar;
        DefaultForeground = defaultFg;
        DefaultBackground = defaultBg;

        _initialState = new Texture(height, width, BackgroundChar, DefaultForeground, DefaultBackground);
        _renderCanvas = new Texture(height, width, BackgroundChar, DefaultForeground, DefaultBackground);

        int totalCells = height * (width + 1);
        _fgColorBuffer = new ConsoleColor?[totalCells];
        _bgColorBuffer = new ConsoleColor?[totalCells];
    }

    public void Draw(Texture texture)
    {
        if (texture is not null)
            Textures.Add(texture);
    }

    public void DrawRange(params Texture[] textures)
    {
        foreach (var texture in textures)
        {
            if (texture is not null)
                Textures.Add(texture);
        }
    }

    public void Clear()
    {
        Array.Copy(_initialState.Buffer, _renderCanvas.Buffer, _initialState.Buffer.Length);
        Array.Clear(_fgColorBuffer, 0, _fgColorBuffer.Length);
        Array.Clear(_bgColorBuffer, 0, _bgColorBuffer.Length);

        // Предотвращаем бесконечное разрастание списка в цикле кадров
        Textures.Clear();
    }

    public void Render()
    {
        // Сортировка текстур по ZIndex перед отрисовкой
        Textures.Sort();

        Span<char> canvasSpan = _renderCanvas.Buffer.AsSpan();
        int canvasStride = Width + 1;

        foreach (var texture in Textures)
        {
            int startY = Math.Max(0, texture.Y);
            int endY = Math.Min(Height, texture.Y + texture.Height);

            int startX = Math.Max(0, texture.X);
            int endX = Math.Min(Width, texture.X + texture.Width);

            if (startX >= endX || startY >= endY)
                continue;

            ReadOnlySpan<char> textureSpan = texture.Buffer.AsSpan();
            int textureStride = texture.Width + 1;
            int copyWidth = endX - startX;

            ConsoleColor effectiveFg = texture.ForegroundColor ?? DefaultForeground;

            for (int i = startY; i < endY; i++)
            {
                int localY = i - texture.Y;
                int localStartX = startX - texture.X;

                ReadOnlySpan<char> srcRow = textureSpan.Slice(localY * textureStride + localStartX, copyWidth);
                int dstRowOffset = i * canvasStride + startX;
                Span<char> dstRow = canvasSpan.Slice(dstRowOffset, copyWidth);

                for (int col = 0; col < copyWidth; col++)
                {
                    char symbol = srcRow[col];
                    if (!texture.IsIgnoreSymbol(symbol))
                    {
                        dstRow[col] = symbol;

                        int cellIndex = dstRowOffset + col;
                        _fgColorBuffer[cellIndex] = effectiveFg;
                        if (texture.BackgroundColor.HasValue)
                            _bgColorBuffer[cellIndex] = texture.BackgroundColor.Value;
                    }
                }
            }
        }
    }

    public void Display()
    {
        Console.CursorVisible = false;
        Console.SetCursorPosition(0, 0);

        ConsoleColor currentFg = DefaultForeground;
        ConsoleColor currentBg = DefaultBackground;
        Console.ForegroundColor = currentFg;
        Console.BackgroundColor = currentBg;

        int stride = Width + 1;
        ReadOnlySpan<char> buffer = _renderCanvas.Buffer.AsSpan();

        for (int y = 0; y < Height; y++)
        {
            int rowOffset = y * stride;
            int runStart = 0;

            while (runStart < Width)
            {
                int firstCellIdx = rowOffset + runStart;
                ConsoleColor targetFg = _fgColorBuffer[firstCellIdx] ?? DefaultForeground;
                ConsoleColor targetBg = _bgColorBuffer[firstCellIdx] ?? DefaultBackground;

                // Переключаем цвет только когда палитра изменилась
                if (targetFg != currentFg)
                {
                    Console.ForegroundColor = targetFg;
                    currentFg = targetFg;
                }
                if (targetBg != currentBg)
                {
                    Console.BackgroundColor = targetBg;
                    currentBg = targetBg;
                }

                // Ищем длину непрерывного блока символов с одинаковым цветом
                int runLength = 1;
                while (runStart + runLength < Width)
                {
                    int nextCellIdx = rowOffset + runStart + runLength;
                    ConsoleColor nextFg = _fgColorBuffer[nextCellIdx] ?? DefaultForeground;
                    ConsoleColor nextBg = _bgColorBuffer[nextCellIdx] ?? DefaultBackground;

                    if (nextFg != targetFg || nextBg != targetBg)
                        break;

                    runLength++;
                }

                // Выводим целый срез строки за один системный вызов
                ReadOnlySpan<char> textSlice = buffer.Slice(firstCellIdx, runLength);
                Console.Out.Write(textSlice);

                runStart += runLength;
            }

            // Переход на новую строку (если не последний ряд экрана)
            if (y < Height - 1)
                Console.Out.WriteLine();
        }
    }
}