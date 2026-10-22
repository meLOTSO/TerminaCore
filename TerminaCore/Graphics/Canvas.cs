using System;
using System.Collections.Generic;

namespace TerminaCore.Graphics;

public class Canvas
{
    private readonly Texture _initialState;
    private readonly Texture _renderCanvas;
    public char BackgroundChar { get; set; } = ' ';
    public int Width => _renderCanvas.Width;
    public int Height => _renderCanvas.Height;

    public List<Texture> Textures { get; set; } = [];

    public Canvas(int width, int height, char backgroundChar = ' ')
    {
        BackgroundChar = backgroundChar;
        _initialState = new Texture(height, width, BackgroundChar);
        _renderCanvas = new Texture(height, width, BackgroundChar);
    }

    public void Draw(Texture texture) => Textures.Add(texture);

    public void DrawRange(params Texture[] textures)
    {
        foreach (var texture in textures)
            Textures.Add(texture);
    }

    public void Clear()
    {
        Array.Copy(_initialState.Buffer, _renderCanvas.Buffer, _initialState.Buffer.Length);
    }

    public void Render()
    {
        Console.CursorVisible = false;

        Span<char> canvasSpan = _renderCanvas.Buffer.AsSpan();
        int canvasStride = _renderCanvas.Width + 1; // ширина строки с учетом '\n'

        foreach (var texture in Textures)
        {
            // Отсечение по вертикали
            int startY = Math.Max(0, texture.Y);
            int endY = Math.Min(_renderCanvas.Height, texture.Y + texture.Height);

            // Отсечение по горизонтали
            int startX = Math.Max(0, texture.X);
            int endX = Math.Min(_renderCanvas.Width, texture.X + texture.Width);

            if (startX >= endX || startY >= endY)
                continue;

            ReadOnlySpan<char> textureSpan = texture.Buffer.AsSpan();
            int textureStride = texture.Width + 1;
            int copyWidth = endX - startX;

            for (int i = startY; i < endY; i++)
            {
                int localY = i - texture.Y;
                int localStartX = startX - texture.X;

                // Срезы строк для конкретного Y
                ReadOnlySpan<char> srcRow = textureSpan.Slice(localY * textureStride + localStartX, copyWidth);
                Span<char> dstRow = canvasSpan.Slice(i * canvasStride + startX, copyWidth);

                // Копирование непрозрачных символов строки
                for (int col = 0; col < copyWidth; col++)
                {
                    char symbol = srcRow[col];
                    if (symbol != ' ' && symbol != '\0')
                    {
                        dstRow[col] = symbol;
                    }
                }
            }
        }
    }

    public void Display()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(_renderCanvas.Buffer);
    }
}