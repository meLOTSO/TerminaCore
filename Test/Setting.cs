using GameCore;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;

public class Setting : IControl, IDrawable
{
    public char[] Buffer { get; set; } = new[] { 'S', 'p', 'a', 'c', 'e', '\n', 'R', 'o', 'o', 't' };

    public void OnKeyPress(GameContext context, ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Enter)
        {
            context.SetState("menu");
        }
    }

    public void Draw(DrawContext context)
    {
        Console.WriteLine(Buffer);
    }
}