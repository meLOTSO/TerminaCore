namespace GameCore.Abstract.GameBasicFunctional;

public interface IUpdateable : IEntity
{
    public void Update(GameCore.Contexts.UpdateContext context);
}
