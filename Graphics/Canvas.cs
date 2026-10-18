namespace Graphics;

public class Canvas
{
    private readonly Texture _initialState;
    private readonly Texture _renderCanvas;
    public char BackgroundChar { get; set; } = ' ';
    public int Width { get; init; }
    public int Height { get; init; }

    public List<Texture> Textures { get; set; } = [];

    public Canvas(int width, int height, char backgroundChar = ' ')
    {
        Width = width;
        Height = height;
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

        foreach (var texture in Textures)
        {
            int startY = Math.Max(0, texture.Y);
            int endY = Math.Min(_renderCanvas.Height, texture.Y + texture.Height);

            int startX = Math.Max(0, texture.X);
            int endX = Math.Min(_renderCanvas.Width, texture.X + texture.Width);

            if (startX >= endX || startY >= endY)
                continue;

            for (int i = startY; i < endY; i++)
            {
                int localY = i - texture.Y;

                for (int j = startX; j < endX; j++)
                {
                    int localX = j - texture.X;

                    char symbol = texture[localY, localX];
                    if (symbol != ' ' && symbol != '\0')
                    {
                        _renderCanvas[i, j] = symbol;
                    }
                }
            }
        }
    }

    public void DisplayRow() => Console.Write(_renderCanvas.Buffer);

    public void DisplayColumn() => Console.WriteLine(_renderCanvas.Buffer);

    public void Display()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(_renderCanvas.Buffer);
    }
}