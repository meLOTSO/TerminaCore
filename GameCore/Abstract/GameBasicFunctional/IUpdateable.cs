namespace GameCore.Abstract.GameBasicFunctional;

public interface IUpdateable : IEntityFunc
{
    public void Update(GameContext context);
}
