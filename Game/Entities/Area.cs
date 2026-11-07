using System;
using System.Collections.Generic;
using TerminaCore.Graphics;

namespace Game.Entities;

public static class Area
{
    public static bool[,] CreateFullAreaFromPart(bool[,] areaPart)
    {
        ArgumentNullException.ThrowIfNull(areaPart);

        int partHeight = areaPart.GetLength(0);
        int partWidth = areaPart.GetLength(1);

        if (partWidth == 0 || partHeight == 0)
            return new bool[0, 0];

        int fullWidth = partWidth * 2 - 1;
        int fullHeight = partHeight * 2 - 1;

        bool[,] fullArea = new bool[fullHeight, fullWidth];

        int centerX = partWidth - 1;
        int centerY = partHeight - 1;

        for (int y = 0; y < partHeight; y++)
        {
            for (int x = 0; x < partWidth; x++)
            {
                if (!areaPart[y, x]) continue;

                int dx = centerX - x;
                int dy = centerY - y;

                fullArea[centerY - dy, centerX - dx] = true;
                fullArea[centerY - dy, centerX + dx] = true;
                fullArea[centerY + dy, centerX - dx] = true;
                fullArea[centerY + dy, centerX + dx] = true;
            }
        }

        return fullArea;
    }

    public static Texture GetTextureFromArea(bool[,] area, char fillChar = ' ')
    {
        ArgumentNullException.ThrowIfNull(area);

        int height = area.GetLength(0);
        int width = area.GetLength(1);

        const char ignoreChar = '*';
        var texture = new Texture(width, height, ignoreChar)
        {
            IgnoreSet = [ignoreChar]
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (area[y, x])
                    texture.SetSymbol(y, x, fillChar);
            }
        }

        return texture;
    }

    // R2: X: +-2, Y: +-1 (Полный размер 3x5)
    public static bool[,] GetR2()
    {
        return new bool[,]
        {
            { false, true, true },
            { true,  true, true }
        };
    }

    // R3: X: +-3, Y: +-1 (Полный размер 3x7)
    public static bool[,] GetR3()
    {
        return new bool[,]
        {
            { false, true, true, true },
            { true,  true, true, true }
        };
    }

    // R4: X: +-4, Y: +-2 (Полный размер 5x9)
    public static bool[,] GetR4()
    {
        return new bool[,]
        {
            { false, false, true, true, true },
            { false, true,  true, true, true },
            { true,  true,  true, true, true }
        };
    }

    // R5: X: +-5, Y: +-2 (Полный размер 5x11)
    public static bool[,] GetR5()
    {
        return new bool[,]
        {
            { false, false, true, true, true, true },
            { false, true,  true, true, true, true },
            { true,  true,  true, true, true, true }
        };
    }

    // R6: X: +-6, Y: +-3 (Полный размер 7x13)
    public static bool[,] GetR6()
    {
        return new bool[,]
        {
            { false, false, false, false, true, true, true },
            { false, false, false, true,  true, true, true },
            { false, true,  true,  true,  true, true, true },
            { true,  true,  true,  true,  true, true, true }
        };
    }

    // R8: X: +-8, Y: +-4 (Полный размер 9x17)
    public static bool[,] GetR8()
    {
        return new bool[,]
        {
            { false, false, false, false, false, false, true, true, true },
            { false, false, false, false, false, true,  true, true, true },
            { false, false, false, false, true,  true,  true, true, true },
            { false, false, true,  true,  true,  true,  true, true, true },
            { true,  true,  true,  true,  true,  true,  true, true, true }
        };
    }
}