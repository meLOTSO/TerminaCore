using EntityCore.Abstract;

namespace EntityCore;

public class EntityCollection : IEntityCollection
{
    protected Dictionary<Type, EntityBag> Entities = new();

    public void AddEntity<T>(T entity) where T : class
    {
        Entities.Add(typeof(T), new EntityBag(entity, true));
    }

    public void UseEntity<T>() where T : notnull
    {
        Entities.Add(typeof(T), new EntityBag(null, false));
    }

    public IEntityProvider BuildEntityProvider()
        => new EntityProvider(new Dictionary<Type, EntityBag>(Entities));
}