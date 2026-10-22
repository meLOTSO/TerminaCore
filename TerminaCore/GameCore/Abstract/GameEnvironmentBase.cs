using System;
using TerminaCore.GameCore.Contexts;

namespace TerminaCore.GameCore.Abstract.GameBasicFunctional;

public abstract class GameEnvironmentBase : IInitializableEnvironment, IDisposable
{
    private GameContext? _gameContext;
    protected GameContext GameContext => _gameContext ?? throw new Exception("GameContext еще не был инициализирован.");

    public virtual void Initialize(GameContext context)
    {
        _gameContext = context;
    }

    public virtual void Dispose()
    {
    }
}