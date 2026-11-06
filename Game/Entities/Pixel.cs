using System;

namespace Game.Entities;

public readonly struct Pixel : IEquatable<Pixel>
{
    public int X { get; init; }
    public int Y { get; init; }

    public Pixel(int x, int y)
    {
        if (x < 0 || y < 0)
            throw new ArgumentOutOfRangeException(nameof(x), "Координаты пикселя не могут быть меньше нуля.");

        X = x;
        Y = y;
    }

    public static Pixel CreateRandom(int width, int height)
    {
        if (width < 1 || height < 1)
            throw new ArgumentException("Размеры для генерации пикселя должны быть больше нуля.");

        return new Pixel(Random.Shared.Next(0, width), Random.Shared.Next(0, height));
    }

    // Деконструкция для удобного синтаксиса: var (x, y) = pixel;
    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

    public bool Equals(Pixel other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Pixel other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Pixel left, Pixel right) => left.Equals(right);
    public static bool operator !=(Pixel left, Pixel right) => !left.Equals(right);

    public override string ToString() => $"({X}, {Y})";
}