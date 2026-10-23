using TerminaCore.Graphics;

public static class Style
{
    public static (int X, int Y) ToPosition(Texture texture, int width, int height, Position widthPos, Position heightPos)
        => (ToPositionByWidth(texture, width, widthPos), ToPositionByHeight(texture, height, heightPos));

    public static int ToPositionByWidth(Texture texture, int width, Position position)
    {
        if (texture.Width > width) return texture.X;
        texture.X = position switch
        {
            Position.Start => 0,
            Position.End => width - texture.Width,
            Position.Center => (width - texture.Width) / 2,
            _ => texture.X
        };
        return texture.X;
    }

    public static int ToPositionByHeight(Texture texture, int height, Position position)
    {
        if (texture.Height > height) return texture.Y;
        texture.Y = position switch
        {
            Position.Start => 0,
            Position.End => height - texture.Height,
            Position.Center => (height - texture.Height) / 2,
            _ => texture.Y
        };
        return texture.Y;
    }

    public static int ToPositionByWidth(Texture childTexture, Texture parentTexture, Position position)
    {
        if (childTexture.Width > parentTexture.Width) return childTexture.X;
        childTexture.X = position switch
        {
            Position.Start => parentTexture.X,
            Position.End => parentTexture.X + parentTexture.Width - childTexture.Width,
            Position.Center => parentTexture.X + (parentTexture.Width - childTexture.Width) / 2,
            _ => childTexture.X
        };
        return childTexture.X;
    }

    public static int ToPositionByHeight(Texture childTexture, Texture parentTexture, Position position)
    {
        if (childTexture.Height > parentTexture.Height) return childTexture.Y;
        childTexture.Y = position switch
        {
            Position.Start => parentTexture.Y,
            Position.End => parentTexture.Y + parentTexture.Height - childTexture.Height,
            Position.Center => parentTexture.Y + (parentTexture.Height - childTexture.Height) / 2,
            _ => childTexture.Y
        };
        return childTexture.Y;
    }

    public static (int X, int Y) ToPosition(Texture childTexture, Texture parentTexture, Position widthPos, Position heightPos)
        => (ToPositionByWidth(childTexture, parentTexture, widthPos), ToPositionByHeight(childTexture, parentTexture, heightPos));

    public static int GetPositionWidth(Texture texture, int width, Position position)
    {
        if (texture.Width > width) return texture.X;
        return position switch
        {
            Position.Start => 0,
            Position.End => width - texture.Width,
            Position.Center => (width - texture.Width) / 2,
            _ => texture.X
        };
    }

    public static int GetPositionHeight(Texture texture, int height, Position position)
    {
        if (texture.Height > height) return texture.Y;
        return position switch
        {
            Position.Start => 0,
            Position.End => height - texture.Height,
            Position.Center => (height - texture.Height) / 2,
            _ => texture.Y
        };
    }

    public static (int X, int Y) GetPosition(Texture texture, int width, int height, Position widthPos, Position heightPos)
        => (GetPositionWidth(texture, width, widthPos), GetPositionHeight(texture, height, heightPos));

    public static int GetPositionWidth(Texture childTexture, Texture parentTexture, Position position)
    {
        if (childTexture.Width > parentTexture.Width) return childTexture.X;
        return position switch
        {
            Position.Start => parentTexture.X,
            Position.End => parentTexture.X + parentTexture.Width - childTexture.Width,
            Position.Center => parentTexture.X + (parentTexture.Width - childTexture.Width) / 2,
            _ => childTexture.X
        };
    }

    public static int GetPositionHeight(Texture childTexture, Texture parentTexture, Position position)
    {
        if (childTexture.Height > parentTexture.Height) return childTexture.Y;
        return position switch
        {
            Position.Start => parentTexture.Y,
            Position.End => parentTexture.Y + parentTexture.Height - childTexture.Height,
            Position.Center => parentTexture.Y + (parentTexture.Height - childTexture.Height) / 2,
            _ => childTexture.Y
        };
    }

    public static (int X, int Y) GetPosition(Texture childTexture, Texture parentTexture, Position widthPos, Position heightPos)
        => (GetPositionWidth(childTexture, parentTexture, widthPos), GetPositionHeight(childTexture, parentTexture, heightPos));
}