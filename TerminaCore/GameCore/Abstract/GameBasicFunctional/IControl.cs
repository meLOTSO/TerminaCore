namespace TerminaCore.GameCore.Abstract.GameBasicFunctional;

public interface IControl : IGameEnvironment
{
    public void OnKeyPress(GameCore.Contexts.ControlContext controlContext);
}