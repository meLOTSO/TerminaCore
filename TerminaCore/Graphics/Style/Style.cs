namespace TerminaCore.Graphics.Style;

public static class Style
{
    // --- Применение позиций (Mutating methods) ---

    public static (int X, int Y) ToPosition(this Texture texture, int width, int height, Position widthPos, Position heightPos)
    {
        ArgumentNullException.ThrowIfNull(texture);
        return (texture.ToPositionByWidth(width, widthPos), texture.ToPositionByHeight(height, heightPos));
    }

    public static int ToPositionByWidth(this Texture texture, int width, Position position)
    {
        ArgumentNullException.ThrowIfNull(texture);
        return texture.X = GetPositionWidth(texture.Width, width, position);
    }

    public static int ToPositionByHeight(this Texture texture, int height, Position position)
    {
        ArgumentNullException.ThrowIfNull(texture);
        return texture.Y = GetPositionHeight(texture.Height, height, position);
    }

    public static (int X, int Y) ToPosition(this Texture childTexture, Texture parentTexture, Position widthPos, Position heightPos)
    {
        ArgumentNullException.ThrowIfNull(childTexture);
        ArgumentNullException.ThrowIfNull(parentTexture);
        return (childTexture.ToPositionByWidth(parentTexture, widthPos), childTexture.ToPositionByHeight(parentTexture, heightPos));
    }

    public static int ToPositionByWidth(this Texture childTexture, Texture parentTexture, Position position)
    {
        ArgumentNullException.ThrowIfNull(childTexture);
        ArgumentNullException.ThrowIfNull(parentTexture);
        return childTexture.X = GetPositionWidth(childTexture, parentTexture, position);
    }

    public static int ToPositionByHeight(this Texture childTexture, Texture parentTexture, Position position)
    {
        ArgumentNullException.ThrowIfNull(childTexture);
        ArgumentNullException.ThrowIfNull(parentTexture);
        return childTexture.Y = GetPositionHeight(childTexture, parentTexture, position);
    }

    // --- Чистые вычисления (Pure Functions) ---

    public static (int X, int Y) GetPosition(this Texture texture, int width, int height, Position widthPos, Position heightPos)
    {
        ArgumentNullException.ThrowIfNull(texture);
        return (GetPositionWidth(texture.Width, width, widthPos), GetPositionHeight(texture.Height, height, heightPos));
    }

    public static int GetPositionWidth(int itemWidth, int containerWidth, Position position)
    {
        return position switch
        {
            Position.Start => 0,
            Position.Center => Math.Max(0, (containerWidth - itemWidth) / 2),
            Position.End => Math.Max(0, containerWidth - itemWidth),
            _ => 0
        };
    }

    public static int GetPositionHeight(int itemHeight, int containerHeight, Position position)
    {
        return position switch
        {
            Position.Start => 0,
            Position.Center => Math.Max(0, (containerHeight - itemHeight) / 2),
            Position.End => Math.Max(0, containerHeight - itemHeight),
            _ => 0
        };
    }

    public static (int X, int Y) GetPosition(Texture childTexture, Texture parentTexture, Position widthPos, Position heightPos)
    {
        ArgumentNullException.ThrowIfNull(childTexture);
        ArgumentNullException.ThrowIfNull(parentTexture);
        return (GetPositionWidth(childTexture, parentTexture, widthPos), GetPositionHeight(childTexture, parentTexture, heightPos));
    }

    public static int GetPositionWidth(Texture childTexture, Texture parentTexture, Position position)
    {
        ArgumentNullException.ThrowIfNull(childTexture);
        ArgumentNullException.ThrowIfNull(parentTexture);
        return parentTexture.X + GetPositionWidth(childTexture.Width, parentTexture.Width, position);
    }

    public static int GetPositionHeight(Texture childTexture, Texture parentTexture, Position position)
    {
        ArgumentNullException.ThrowIfNull(childTexture);
        ArgumentNullException.ThrowIfNull(parentTexture);
        return parentTexture.Y + GetPositionHeight(childTexture.Height, parentTexture.Height, position);
    }
}