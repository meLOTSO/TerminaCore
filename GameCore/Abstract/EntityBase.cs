using EntityCore.Abstract;
using GameCore.Contexts;

namespace GameCore.Abstract.GameBasicFunctional;

public abstract class EntityBase : IEntity
{
    protected readonly GameContext GameContext;
    public EntityBase(GameContext context)
    {
        GameContext = context;
    }
}