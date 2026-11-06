using System;
using System.Collections.Generic;
using Game.Entities;
using Game.Enums;
using TerminaCore.Graphics;

namespace Game.Textures;

public static class MapTextures
{
    public static Texture GetSea(int width, int height)
    {
        if (width < 1 || height < 1)
            throw new ArgumentException("Ширина и высота не могут быть меньше единицы.");

        Texture seaTexture = new(width, height, foregroundColor: ConsoleColor.Blue, backgroundColor: ConsoleColor.DarkCyan)
        {
            IgnoreSet = []
        };

        int maxWaves = Math.Min(100, (width * height) / 3);

        for (int i = 0; i < maxWaves; i++)
        {
            int randX = Random.Shared.Next(width);
            int randY = Random.Shared.Next(height);

            int attempts = 0;
            while (seaTexture[randY, randX] == '~' && attempts < 10)
            {
                randX = Random.Shared.Next(width);
                randY = Random.Shared.Next(height);
                attempts++;
            }

            seaTexture[randY, randX] = '~';
        }

        return seaTexture;
    }

    public static Texture GetGround(int width, int height, ConsoleColor backgroundColor, int groundSymbolsCount, RenderType renderType, int? startX = null, int? startY = null)
    {
        if (width < 1 || height < 1)
            throw new ArgumentException("Ширина и высота не могут быть меньше единицы.");
        if (groundSymbolsCount < 1)
            throw new ArgumentException("Количество символов для земли не может быть меньше единицы.");
        if (groundSymbolsCount > width * height)
            throw new ArgumentException("Количество символов для земли не может быть больше площади земли.");

        const char GROUND_CHAR = ' ';
        const char IGNORE_CHAR = '*';

        Texture groundTexture = new(width, height, IGNORE_CHAR, backgroundColor: backgroundColor)
        {
            IgnoreSet = [IGNORE_CHAR]
        };

        bool[] grounded = new bool[width * height];
        bool[] inQueue = new bool[width * height];

        List<Pixel> listCoords = [];
        Queue<Pixel> queueCoords = [];
        Stack<Pixel> stackCoords = [];

        Action<Pixel> addPixel;
        Func<Pixel?> getPixel;

        switch (renderType)
        {
            case RenderType.Random:
                addPixel = p => listCoords.Add(p);
                getPixel = () =>
                {
                    if (listCoords.Count == 0) return null;
                    int pi = Random.Shared.Next(listCoords.Count);
                    Pixel c = listCoords[pi];
                    listCoords[pi] = listCoords[^1];
                    listCoords.RemoveAt(listCoords.Count - 1);
                    return c;
                };
                break;

            case RenderType.Queue:
                addPixel = p => queueCoords.Enqueue(p);
                getPixel = () => queueCoords.TryDequeue(out var p) ? p : null;
                break;

            case RenderType.Stack:
                addPixel = p => stackCoords.Push(p);
                getPixel = () => stackCoords.TryPop(out var p) ? p : null;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(renderType));
        }

        Pixel start = new()
        {
            X = Math.Clamp(startX ?? Random.Shared.Next(width), 0, width - 1),
            Y = Math.Clamp(startY ?? Random.Shared.Next(height), 0, height - 1)
        };

        int startIndex = start.Y * width + start.X;
        groundTexture.SetSymbol(start.Y, start.X, GROUND_CHAR);
        grounded[startIndex] = true;
        inQueue[startIndex] = true;

        NeighborsCoordsForEach(start, addPixel);

        int placed = 1;
        while (placed < groundSymbolsCount)
        {
            var nextPx = getPixel.Invoke();
            if (nextPx is null) break;

            Pixel px = nextPx.Value;
            int cellIdx = px.Y * width + px.X;

            if (grounded[cellIdx]) continue;

            if (Random.Shared.NextDouble() <= 0.85)
            {
                groundTexture.SetSymbol(px.Y, px.X, GROUND_CHAR);
                grounded[cellIdx] = true;
                placed++;

                NeighborsCoordsForEach(px, addPixel);
            }
            else
            {
                if (renderType == RenderType.Stack && stackCoords.Count > 1)
                {
                    var temp = stackCoords.Pop();
                    stackCoords.Push(px);
                    stackCoords.Push(temp);
                }
                else
                {
                    addPixel.Invoke(px);
                }
            }
        }

        void NeighborsCoordsForEach(in Pixel pixel, Action<Pixel> action)
        {
            for (int xi = pixel.X - 1; xi <= pixel.X + 1; xi++)
            {
                for (int yi = pixel.Y - 1; yi <= pixel.Y + 1; yi++)
                {
                    if (xi == pixel.X && yi == pixel.Y) continue;

                    if (xi >= 0 && xi < width && yi >= 0 && yi < height)
                    {
                        int idx = yi * width + xi;
                        if (!grounded[idx] && !inQueue[idx])
                        {
                            inQueue[idx] = true;
                            action.Invoke(new Pixel(xi, yi));
                        }
                    }
                }
            }
        }

        return groundTexture;
    }
}