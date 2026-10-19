using GameCore;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;
using Graphics;

public class Game : IControl, IDrawable, IUpdateable
{
    public Texture Texture { get; set; }
    public Texture Some { get; set; }
    public int SpeedX { get; set; } = 0;
    public int SpeedY { get; set; } = 0;

    public Game()
    {
        Texture = new(@"C:\Users\MOLL\OneDrive\Документы\KAI\7 семестр\ТВППИТС (паттерны программирования)\lab1\code\Test\texture.txt");
        Some = new Texture(
            "|-|",
            "fdf");
    }

    public void OnKeyPress(GameContext context, ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.RightArrow)
        {
            SpeedX = 1;
            SpeedY = 0;
        }
        else if (controlContext.Key == ConsoleKey.LeftArrow)
        {
            SpeedX = -1;
            SpeedY = 0;
        }
        else if (controlContext.Key == ConsoleKey.UpArrow)
        {
            SpeedY = -1;
            SpeedX = 0;
        }
        else if (controlContext.Key == ConsoleKey.DownArrow)
        {
            SpeedY = 1;
            SpeedX = 0;
        }
        else if (controlContext.Key == ConsoleKey.Escape)
        {
            context.SetState("menu");
        }
    }

    public void Draw(DrawContext context)
    {
        context.Draw(Some);
        context.Draw(Texture);
    }

    public void Update(GameContext context)
    {
        Texture.X += SpeedX;
        Texture.Y += SpeedY;
    }
}