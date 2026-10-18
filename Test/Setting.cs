using GameCore;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;
using Graphics;

public class Setting : IControl, IDrawable
{
    public Texture Texture { get; set; }

    public Setting()
    {
        Texture = new(
            "Space",
            "Root");
    }

    public void OnKeyPress(GameContext context, ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Escape)
        {
            context.SetState("menu");
        }
    }

    public void Draw(DrawContext context)
    {
        context.Draw(Texture);
    }
}