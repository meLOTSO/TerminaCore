namespace GameCore.Abstract.GameBasicFunctional;

public interface IControl : IEntity
{
    public void OnKeyPress(GameCore.Contexts.ControlContext controlContext);
}