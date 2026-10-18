using GameCore;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;
using Graphics;

public class Game : IControl, IDrawable
{
    public Texture Texture { get; set; }

    public Game()
    {
        Texture = new(new char[,] { { '.' } });
    }

    public void OnKeyPress(GameContext context, ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.RightArrow)
        {
            Texture.X++;
        }
        else if (controlContext.Key == ConsoleKey.LeftArrow)
        {
            Texture.X--;
        }
        else if (controlContext.Key == ConsoleKey.UpArrow)
        {
            Texture.Y--;
        }
        else if (controlContext.Key == ConsoleKey.DownArrow)
        {
            Texture.Y++;
        }
        else if (controlContext.Key == ConsoleKey.Escape)
        {
            context.SetState("menu");
        }
    }

    public void Draw(DrawContext context)
    {
        context.Draw(Texture);
    }
}