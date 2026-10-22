namespace TerminaCore.EntityCore;

public class EntityBag
{
    public object? Entity { get; set; }
    public bool Released { get; set; }

    public EntityBag(object? entity, bool released)
    {
        Entity = entity;
        Released = released;
    }
}