using EntityCore.Abstract;
using GameCore.Contexts;

namespace GameCore.Abstract.GameBasicFunctional;

public abstract class GameEnvironmentBase : IInitializableEnvironment
{
    private GameContext? _gameContext;
    protected GameContext GameContext => _gameContext ?? throw new Exception("GameContext еще не был инициализирован.");
    public virtual void Initialize(GameContext context)
    {
        _gameContext = context;
    }
}