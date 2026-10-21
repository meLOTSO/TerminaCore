using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using EntityCore.Abstract;
using Debug;

namespace EntityCore;

public class EntityProvider : IEntityProvider
{
    protected Dictionary<Type, EntityBag> Entities;

    public EntityProvider()
    {
        Entities = new();
    }
    public EntityProvider(Dictionary<Type, EntityBag> entities)
    {
        Entities = entities;

        StringBuilder sb = new("В EntityProvider переданы:\n");
        foreach ((Type entityType, EntityBag bag) in entities)
        {
            sb.AppendLine($"\t{entityType.Name}: {bag.Released}");
        }
        FileDebug.Write(sb.ToString());
    }

    public T? GetEntity<T>() where T : notnull
    {
        if (Entities.TryGetValue(typeof(T), out EntityBag? bag) && bag is not null && bag.Entity is T entity) return entity;
        return default;
    }

    public T GetEntityRequired<T>() where T : notnull
    {
        FileDebug.WriteLine($"GetEntityRequired: {typeof(T).Name}.");
        if (Entities.TryGetValue(typeof(T), out EntityBag? bag) && bag is not null)
        {
            FileDebug.WriteLine($"\tПолучена зарегистрированная сущность '{typeof(T).Name}-{bag}'.");
            if (bag.Entity is object value)
            {
                var entity = (T)value;
                FileDebug.WriteLine($"\tСущность '{entity}' принадлежит типу '{typeof(T).Name}'.");
                return entity;
            }
            else
            {
                FileDebug.WriteLine($"\tСущность '{bag.Entity}' не принадлежит типу '{typeof(T).Name}'.");
            }
        }
        FileDebug.WriteLine($"\tСущность '{typeof(T).Name}' не найдена.");
        throw new Exception($"Сущность '{typeof(T).Name}' не найдена.");
    }

    public bool TryReleaseEntity<T>() where T : notnull
    {
        if (Entities.TryGetValue(typeof(T), out EntityBag? bag) && bag is not null && !bag.Released)
        {
            T value = Activator.CreateInstance<T>();
            bag.Entity = value;
            bag.Released = true;
            return true;
        }
        return false;
    }

    public bool TryReleaseEntity<T>(T entity) where T : notnull
    {
        if (Entities.TryGetValue(typeof(T), out EntityBag? bag) && bag is not null && !bag.Released)
        {
            bag.Entity = entity;
            bag.Released = true;
            return true;
        }
        return false;
    }

    public bool IsNotReleasedEntity<T>() where T : notnull
    {
        if (Entities.TryGetValue(typeof(T), out EntityBag? bag) && bag is not null && !bag.Released) return true;
        return false;
    }

    public bool IsReleasedEntity<T>() where T : notnull
    {
        if (Entities.TryGetValue(typeof(T), out EntityBag? bag) && bag is not null && bag.Released) return true;
        return false;
    }

    public bool TryReleaseEntity(Type entityType)
    {
        if (Entities.TryGetValue(entityType, out EntityBag? bag) && bag is not null && !bag.Released)
        {
            var ctor = entityType.GetConstructor(Type.EmptyTypes);
            var entity = ctor?.Invoke(null);
            bag.Entity = entity;
            bag.Released = true;
            return true;
        }
        return false;
    }

    public bool TryReleaseEntity(Type entityType, object entityObject)
    {
        FileDebug.WriteLine($"TryReleaseEntity: ({entityType.Name}) {entityObject}.");
        if (Entities.TryGetValue(entityType, out EntityBag? bag) && bag is not null && !bag.Released)
        {
            FileDebug.WriteLine($"\tПолучена зарегистрированная и не реализованная сущность '{entityObject}'.");
            bag.Entity = entityObject;
            bag.Released = true;
            FileDebug.WriteLine($"\t{bag.Entity}: {bag.Released}");
            return true;
        }
        return false;
    }
}