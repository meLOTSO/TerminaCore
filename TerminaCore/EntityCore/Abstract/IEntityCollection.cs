namespace TerminaCore.EntityCore.Abstract;

public interface IEntityCollection
{
    public void AddEntity<T>(T entity) where T : class;
    public void UseEntity<T>() where T : class;
}
