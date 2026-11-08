using Game.Display;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;

public class NewGameEnvironment : GameEnvironmentBase, IDrawable, IControl, IUpdateable
{
    public InputText Input { get; } = new("Civilization Name", 20);

    public void Update(UpdateContext context)
    {
        Input.Update(context.DeltaTime);
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Escape || controlContext.Key == ConsoleKey.Enter)
        {
            GameContext.SetState("menu");
        }
        else
        {
            Input.ControlHandler(controlContext);
        }
    }

    public void Draw(DrawContext context)
    {
        Input.TexturesForDraw(context.Draw, false);
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);
    }
}
