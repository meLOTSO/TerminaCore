using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerminaCore.Graphics;

public class Texture : IComparable<Texture>
{
    public char[] Buffer { get; set; } = [];
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int ZIndex { get; set; }
    public HashSet<char> IgnoreSet { get; set; } = [' ', '\0'];

    public ConsoleColor? ForegroundColor { get; set; } = null;
    public ConsoleColor? BackgroundColor { get; set; } = null;

    public Texture(char[,] texture, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(texture, foregroundColor, backgroundColor);

    public Texture(char[] buffer, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(buffer, foregroundColor, backgroundColor);

    public Texture(List<string> texture, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(texture, foregroundColor, backgroundColor);

    public Texture(string buffer, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(buffer, foregroundColor, backgroundColor);

    public Texture(int height, int width, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(height, width, foregroundColor, backgroundColor);

    public Texture(int height, int width, char symbol, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(height, width, symbol, foregroundColor, backgroundColor);

    private Texture(char[] buffer, int height, int width, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(buffer, height, width, foregroundColor, backgroundColor);

    public void SetTexture(char[,] texture, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        Height = texture.GetLength(0);
        Width = texture.GetLength(1);
        Buffer = new char[Height * (Width + 1)];
        for (int i = 0; i < Height; i++)
        {
            int rowOffset = i * (Width + 1);
            for (int j = 0; j < Width; j++)
            {
                Buffer[rowOffset + j] = texture[i, j];
            }
            Buffer[rowOffset + Width] = '\n';
        }
        SetColor(foregroundColor, backgroundColor);
    }

    public void SetTexture(char[] buffer, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        Height = 1;
        Width = buffer.Length;
        Buffer = new char[Height * (Width + 1)];
        buffer.CopyTo(Buffer, 0);
        Buffer[Width] = '\n';
        SetColor(foregroundColor, backgroundColor);
    }

    public void SetTexture(string texture, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        => SetTexture(texture.ToCharArray(), foregroundColor, backgroundColor);

    public void SetTexture(List<string> texture, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        var res = Texture.From(texture, texture.Count, texture.Count > 0 ? texture.Max(str => str.Length) : 0);
        Buffer = res.Buffer;
        Height = res.Height;
        Width = res.Width;
        SetColor(foregroundColor, backgroundColor);
    }

    public void SetTextureTxt(string path, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        if (Path.GetExtension(path) != ".txt")
            throw new ArgumentException("Файл должен быть формата .txt", nameof(path));

        List<string> data = new();
        int maxWidth = 0;
        using (var textReader = new StreamReader(path))
        {
            string? line;
            while ((line = textReader.ReadLine()) is not null)
            {
                string value = line.TrimEnd();
                data.Add(value);
                maxWidth = Math.Max(maxWidth, value.Length);
            }
        }
        var res = Texture.From(data, data.Count, maxWidth);
        Buffer = res.Buffer;
        Height = res.Height;
        Width = res.Width;
        SetColor(foregroundColor, backgroundColor);
    }

    public void SetTexture(int height, int width, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        Height = height;
        Width = width;
        Buffer = new char[height * (width + 1)];
        for (int i = 0; i < height; i++)
        {
            int rowOffset = i * (width + 1);
            for (int j = 0; j < width; j++)
                Buffer[rowOffset + j] = ' ';
            Buffer[rowOffset + width] = '\n';
        }
        SetColor(foregroundColor, backgroundColor);
    }

    public void SetTexture(int height, int width, char symbol, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        SetTexture(height, width, foregroundColor, backgroundColor);
        for (int i = 0; i < height; i++)
        {
            int rowOffset = i * (width + 1);
            for (int j = 0; j < width; j++)
                Buffer[rowOffset + j] = symbol;
        }
    }

    private void SetTexture(char[] buffer, int height, int width, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        Height = height;
        Width = width;
        Buffer = buffer;
        SetColor(foregroundColor, backgroundColor);
    }

    public void SetColor(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
    {
        ForegroundColor = foregroundColor;
        BackgroundColor = backgroundColor;
    }

    public char this[int i, int j]
    {
        get => GetSymbol(i, j);
        set => SetSymbol(i, j, value);
    }

    public int GetBufferIndex(int i, int j)
    {
        if (i < 0 || i >= Height || j < 0 || j >= Width)
            throw new IndexOutOfRangeException($"Координаты [{i}, {j}] выходят за пределы {Height}x{Width}");
        return i * (Width + 1) + j;
    }

    public bool TryGetBufferIndex(int i, int j, out int index)
    {
        if (i >= 0 && i < Height && j >= 0 && j < Width)
        {
            index = i * (Width + 1) + j;
            return true;
        }
        index = -1;
        return false;
    }

    public char GetSymbol(int i, int j) => Buffer[GetBufferIndex(i, j)];

    public bool TryGetSymbol(int i, int j, out char symbol)
    {
        if (TryGetBufferIndex(i, j, out int index))
        {
            symbol = Buffer[index];
            return true;
        }
        symbol = default;
        return false;
    }

    public void SetSymbol(int i, int j, char symbol) => Buffer[GetBufferIndex(i, j)] = symbol;

    public bool TrySetSymbol(int i, int j, char symbol)
    {
        if (TryGetBufferIndex(i, j, out int index))
        {
            Buffer[index] = symbol;
            return true;
        }
        return false;
    }

    public Texture Join(Texture texture, int x, int y, bool slicer = true)
    {
        ArgumentNullException.ThrowIfNull(texture);

        if (!slicer)
            return Texture.Join(this, texture, x, y);

        Texture result = this.Copy();

        int startY = Math.Max(0, y);
        int endY = Math.Min(Height, y + texture.Height);

        int startX = Math.Max(0, x);
        int endX = Math.Min(Width, x + texture.Width);

        if (startX >= endX || startY >= endY)
            return result;

        Span<char> dstBuffer = result.Buffer.AsSpan();
        ReadOnlySpan<char> srcBuffer = texture.Buffer.AsSpan();

        int dstStride = Width + 1;
        int srcStride = texture.Width + 1;
        int copyWidth = endX - startX;

        for (int rowY = startY; rowY < endY; rowY++)
        {
            int srcLocalY = rowY - y;
            int srcLocalX = startX - x;

            Span<char> dstRow = dstBuffer.Slice(rowY * dstStride + startX, copyWidth);
            ReadOnlySpan<char> srcRow = srcBuffer.Slice(srcLocalY * srcStride + srcLocalX, copyWidth);

            for (int col = 0; col < copyWidth; col++)
            {
                char symbol = srcRow[col];
                if (symbol != '\0' && !texture.IgnoreSet.Contains(symbol))
                {
                    dstRow[col] = symbol;
                }
            }
        }

        return result;
    }

    public Texture Join(Texture texture, bool slicer = true)
        => this.Join(texture, texture.X, texture.Y, slicer);

    public static Texture Join(Texture mainTexture, Texture textureToJoin, int x = 0, int y = 0)
    {
        ArgumentNullException.ThrowIfNull(mainTexture);
        ArgumentNullException.ThrowIfNull(textureToJoin);

        int newWidth = Math.Max(mainTexture.Width, textureToJoin.Width + x);
        int newHeight = Math.Max(mainTexture.Height, textureToJoin.Height + y);

        if (newWidth == mainTexture.Width && newHeight == mainTexture.Height && x >= 0 && y >= 0)
        {
            return mainTexture.Join(textureToJoin, x, y, slicer: true);
        }

        var result = new Texture(newHeight, newWidth, mainTexture.ForegroundColor, mainTexture.BackgroundColor);
        Span<char> dstBuffer = result.Buffer.AsSpan();
        int dstStride = newWidth + 1;

        ReadOnlySpan<char> mainBuffer = mainTexture.Buffer.AsSpan();
        int mainStride = mainTexture.Width + 1;
        for (int row = 0; row < mainTexture.Height; row++)
        {
            ReadOnlySpan<char> srcRow = mainBuffer.Slice(row * mainStride, mainTexture.Width);
            Span<char> dstRow = dstBuffer.Slice(row * dstStride, mainTexture.Width);
            srcRow.CopyTo(dstRow);
        }

        ReadOnlySpan<char> joinBuffer = textureToJoin.Buffer.AsSpan();
        int joinStride = textureToJoin.Width + 1;

        int startY = Math.Max(0, y);
        int endY = Math.Min(newHeight, y + textureToJoin.Height);
        int startX = Math.Max(0, x);
        int endX = Math.Min(newWidth, x + textureToJoin.Width);
        int copyWidth = endX - startX;

        if (copyWidth > 0)
        {
            for (int rowY = startY; rowY < endY; rowY++)
            {
                int srcLocalY = rowY - y;
                int srcLocalX = startX - x;

                ReadOnlySpan<char> srcRow = joinBuffer.Slice(srcLocalY * joinStride + srcLocalX, copyWidth);
                Span<char> dstRow = dstBuffer.Slice(rowY * dstStride + startX, copyWidth);

                for (int col = 0; col < copyWidth; col++)
                {
                    char symbol = srcRow[col];
                    if (symbol != '\0' && !textureToJoin.IgnoreSet.Contains(symbol))
                    {
                        dstRow[col] = symbol;
                    }
                }
            }
        }

        return result;
    }

    public static Texture From(List<string> texture, int height, int width)
    {
        char[] buffer = new char[height * (width + 1)];
        int j;
        for (int i = 0; i < height; i++)
        {
            string line = texture[i];
            int rowOffset = i * (width + 1);
            for (j = 0; j < line.Length; j++)
                buffer[rowOffset + j] = line[j];
            for (; j < width; j++)
                buffer[rowOffset + j] = ' ';
            buffer[rowOffset + width] = '\n';
        }
        return new Texture(buffer, height, width);
    }

    public void DisplayInColumn() => Console.WriteLine(Buffer);
    public void DisplayInRow() => Console.Write(Buffer);

    public void DisplayAbsolute(int x, int y)
    {
        int stride = Width + 1;
        for (int i = 0; i < Height; i++)
        {
            Console.SetCursorPosition(x, y + i);
            ReadOnlySpan<char> rowSpan = Buffer.AsSpan(i * stride, Width);
            Console.Out.Write(rowSpan);
        }
    }

    public bool CollidesWith(Texture other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return X < other.X + other.Width &&
               X + Width > other.X &&
               Y < other.Y + other.Height &&
               Y + Height > other.Y;
    }

    public static bool CheckCollision(Texture texture1, Texture texture2) => texture1.CollidesWith(texture2);

    public bool IntersectsPixelPerfect(Texture other)
    {
        ArgumentNullException.ThrowIfNull(other);

        int startX = Math.Max(X, other.X);
        int endX = Math.Min(X + Width, other.X + other.Width);
        int startY = Math.Max(Y, other.Y);
        int endY = Math.Min(Y + Height, other.Y + other.Height);

        if (startX >= endX || startY >= endY)
            return false;

        ReadOnlySpan<char> thisBuffer = Buffer.AsSpan();
        ReadOnlySpan<char> otherBuffer = other.Buffer.AsSpan();

        int thisStride = Width + 1;
        int otherStride = other.Width + 1;
        int overlapWidth = endX - startX;

        for (int worldY = startY; worldY < endY; worldY++)
        {
            int localY1 = worldY - Y;
            int localX1 = startX - X;

            int localY2 = worldY - other.Y;
            int localX2 = startX - other.X;

            ReadOnlySpan<char> row1 = thisBuffer.Slice(localY1 * thisStride + localX1, overlapWidth);
            ReadOnlySpan<char> row2 = otherBuffer.Slice(localY2 * otherStride + localX2, overlapWidth);

            for (int col = 0; col < overlapWidth; col++)
            {
                char c1 = row1[col];
                char c2 = row2[col];

                if (!IsIgnoreSymbol(c1) && !other.IsIgnoreSymbol(c2))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Texture Copy() => new Texture((char[])Buffer.Clone(), Height, Width, ForegroundColor, BackgroundColor)
    {
        X = this.X,
        Y = this.Y,
        ZIndex = this.ZIndex,
        IgnoreSet = new HashSet<char>(this.IgnoreSet)
    };

    public int CompareTo(Texture? other)
    {
        if (other is null) return 1;
        return ZIndex.CompareTo(other.ZIndex);
    }

    public bool IsIgnoreSymbol(char symbol) => IgnoreSet.Contains(symbol);
}