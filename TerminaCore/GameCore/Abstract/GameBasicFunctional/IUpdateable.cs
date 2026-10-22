namespace TerminaCore.GameCore.Abstract.GameBasicFunctional;

public interface IUpdateable : IGameEnvironment
{
    public void Update(GameCore.Contexts.UpdateContext context);
}
