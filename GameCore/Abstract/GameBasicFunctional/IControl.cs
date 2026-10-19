namespace GameCore.Abstract.GameBasicFunctional;

public interface IControl : IEntityFunc
{
    public void OnKeyPress(GameContext context, ControlContext controlContext);
}