namespace GameCore.Abstract.GameBasicFunctional;

public interface IGameFunctionalMutable
{
    public void Entity<T>(T entity) where T : class, IEntityFunc;
}