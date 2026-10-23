using System;

namespace TerminaCore.EntityCore.Abstract;

public interface IEntityProvider
{
    public T? GetEntity<T>() where T : class;
    public T GetEntityRequired<T>() where T : class;
    public bool TryReleaseEntity<T>() where T : class;
    public bool TryReleaseEntity<T>(T entity) where T : class;
    public bool IsNotReleasedEntity<T>() where T : class;
    public bool IsReleasedEntity<T>() where T : class;

    public bool TryReleaseEntity(Type entityType);
    public bool TryReleaseEntity(Type entityType, object entityObject);
}