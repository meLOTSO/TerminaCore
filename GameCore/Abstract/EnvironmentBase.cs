using EntityCore.Abstract;
using GameCore.Contexts;

namespace GameCore.Abstract.GameBasicFunctional;

public abstract class GameEnvironmentBase : IGameEnvironment
{
    protected readonly GameContext GameContext;
    public GameEnvironmentBase(GameContext context)
    {
        GameContext = context;
    }
}