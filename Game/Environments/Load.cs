using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;

public class Load : GameEnvironmentBase, IControl, IDrawable
{
    public Load()
    {

    }

    public void OnKeyPress(ControlContext controlContext)
    {
        throw new NotImplementedException();
    }

    public void Draw(DrawContext context)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

    }
}