using System;

namespace TerminaCore.EntityCore.Abstract;

public interface IEntityProvider
{
    public T? GetEntity<T>() where T : notnull;
    public T GetEntityRequired<T>() where T : notnull;
    public bool TryReleaseEntity<T>() where T : notnull;
    public bool TryReleaseEntity<T>(T entity) where T : notnull;
    public bool IsNotReleasedEntity<T>() where T : notnull;
    public bool IsReleasedEntity<T>() where T : notnull;

    public bool TryReleaseEntity(Type entityType);
    public bool TryReleaseEntity(Type entityType, object entityObject);
}