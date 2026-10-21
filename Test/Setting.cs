using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;
using GameCore.Contexts;
using Graphics;

public class Setting : EntityBase, IControl, IDrawable
{
    public Texture Texture { get; set; }

    public Setting(GameContext context) : base(context)
    {
        Texture = new(
            "Space",
            "Root");
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Escape)
        {
            GameContext.SetState("menu");
        }
    }

    public void Draw(DrawContext context)
    {
        context.Draw(Texture);
    }
}