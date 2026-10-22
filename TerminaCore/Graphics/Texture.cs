using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TerminaCore.Graphics;

public class Texture
{
    public char[] Buffer { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Texture(char[,] texture)
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
    }

    public Texture(List<string> texture)
    {
        var res = Texture.From(texture, texture.Count, texture.Count > 0 ? texture.Max(str => str.Length) : 0);
        Buffer = res.Buffer;
        Height = res.Height;
        Width = res.Width;
    }
    public Texture(params string[] lines) : this(lines.ToList())
    {
    }

    public Texture(string path)
    {
        if (Path.GetExtension(path) != ".txt")
            throw new ArgumentException("Файл должен быть формата .txt", nameof(path));

        List<string> data = new();
        using (var textReader = new StreamReader(path))
        {
            string? line;
            while ((line = textReader.ReadLine()) is not null)
            {
                Height++;
                string value = line.TrimEnd();
                data.Add(value);
                Width = int.Max(Width, value.Length);
            }
        }
        var res = Texture.From(data, Height, Width);
        Buffer = res.Buffer;
    }

    public Texture(int height, int width)
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
    }

    public Texture(int height, int width, char symbol) : this(height, width)
    {
        for (int i = 0; i < height; i++)
        {
            int rowOffset = i * (width + 1);
            for (int j = 0; j < width; j++)
                Buffer[rowOffset + j] = symbol;
        }
    }

    private Texture(char[] buffer, int height, int width)
    {
        Height = height;
        Width = width;
        Buffer = buffer;
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

    public char GetSymbol(int i, int j)
    {
        return Buffer[GetBufferIndex(i, j)];
    }

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

    public void SetSymbol(int i, int j, char symbol)
    {
        Buffer[GetBufferIndex(i, j)] = symbol;
    }

    public bool TrySetSymbol(int i, int j, char symbol)
    {
        if (TryGetBufferIndex(i, j, out int index))
        {
            Buffer[index] = symbol;
            return true;
        }
        return false;
    }

    public Texture Copy() => new Texture((char[])Buffer.Clone(), Height, Width);

    public Texture Join(Texture texture, int x, int y, string ignoreSymbols = " ", bool slicer = true)
    {
        if (!slicer) return Texture.Join(this, texture, x, y, ignoreSymbols);

        Texture result = this.Copy();
        int maxY = Math.Min(Height, y + texture.Height);
        int maxX = Math.Min(Width, x + texture.Width);

        for (int i = Math.Max(0, y); i < maxY; i++)
        {
            for (int j = Math.Max(0, x); j < maxX; j++)
            {
                char symbol = texture[i - y, j - x];
                if (!ignoreSymbols.Contains(symbol))
                    result[i, j] = symbol;
            }
        }
        return result;
    }
    public Texture Join(Texture texture, string ignoreSymbols = " ", bool slicer = true)
    {
        return this.Join(texture, texture.X, texture.Y, ignoreSymbols, slicer);
    }

    public static Texture Join(Texture mainTexture, Texture textureToJoin, int x = 0, int y = 0, string ignoreSymbols = " ")
    {
        if ((mainTexture.Width >= textureToJoin.Width + x) && (mainTexture.Height >= textureToJoin.Height + y))
        {
            var canvas = mainTexture.Copy();
            for (int i = 0; i < textureToJoin.Height; i++)
            {
                for (int j = 0; j < textureToJoin.Width; j++)
                {
                    char symbol = textureToJoin[i, j];
                    if (!ignoreSymbols.Contains(symbol))
                        canvas[i + y, j + x] = symbol;
                }
            }
            return canvas;
        }
        else
        {
            int width = int.Max(mainTexture.Width, textureToJoin.Width + x);
            int height = int.Max(mainTexture.Height, textureToJoin.Height + y);
            var canvas = new Texture(height, width); // Теперь корректно заполнен пробелами и \n

            for (int i = 0; i < mainTexture.Height; i++)
                for (int j = 0; j < mainTexture.Width; j++)
                    canvas[i, j] = mainTexture[i, j];

            for (int i = 0; i < textureToJoin.Height; i++)
            {
                for (int j = 0; j < textureToJoin.Width; j++)
                {
                    char symbol = textureToJoin[i, j];
                    if (ignoreSymbols.Contains(symbol)) continue;
                    canvas[i + y, j + x] = symbol;
                }
            }
            return canvas;
        }
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
            {
                buffer[rowOffset + j] = line[j];
            }
            for (; j < width; j++)
            {
                buffer[rowOffset + j] = ' ';
            }
            buffer[rowOffset + width] = '\n';
        }
        return new Texture(buffer, height, width);
    }

    public void DisplayInColumn()
    {
        Console.WriteLine(Buffer);
    }

    public void DisplayInRow()
    {
        Console.Write(Buffer);
    }

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

    public bool IsCollisedWith(Texture texture)
    {
        ArgumentNullException.ThrowIfNull(texture);

        return X < texture.X + texture.Width &&
               X + Width > texture.X &&
               Y < texture.Y + texture.Height &&
               Y + Height > texture.Y;
    }

    public static bool IsCollised(Texture texture1, Texture texture2) => texture1.IsCollisedWith(texture2);
}