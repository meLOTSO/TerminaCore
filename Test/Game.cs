using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;
using GameCore.Contexts;
using Graphics;

public class Game : GameEnvironmentBase, IControl, IDrawable, IUpdateable
{
    public Texture Texture { get; set; }
    public Texture Some { get; set; }
    public int SpeedX { get; set; } = 0;
    public int SpeedY { get; set; } = 0;

    public Game(GameContext context) : base(context)
    {
        Texture = new(@"C:\Users\MOLL\OneDrive\Документы\KAI\7 семестр\ТВП ИТС (паттерны программирования)\lab1\code\Test\texture.txt");
        Some = new Texture(
            "|-|",
            "fdf");
    }

    public void Reset()
    {
        Texture.X = 0;
        Texture.Y = 0;
        SpeedX = 0;
        SpeedY = 0;
    }

    public void OnKeyPress(ControlContext controlContext)
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
            GameContext.SetState("menu");
        }
    }

    public void Draw(DrawContext context)
    {
        context.Draw(Some);
        context.Draw(Texture);
    }

    public void Update(UpdateContext context)
    {
        Texture.X += SpeedX;
        Texture.Y += SpeedY;
    }
}