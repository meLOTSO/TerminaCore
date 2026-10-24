using TerminaCore.EntityCore;
using TerminaCore.EntityCore.Abstract;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Abstract;
using TerminaCore.GameCore.Delegates;
using TerminaCore.ServiceCore;
using TerminaCore.ServiceCore.Abstract;
using TerminaCore.GameCore.Enums;

namespace TerminaCore.GameCore.Builder;

public class GameApplicationBuilder : GameBase
{
    private readonly Dictionary<Type, Func<object>> _environmentFactories = new(); // object Factory(GameContext context)
    private readonly Dictionary<Type, GameFunctions> _environmentGameFunctions = new(); // object Factory(GameContext context)
    private readonly ServiceCollection _services = new(); // сервисы
    private readonly EntityCollection _entitiesCollection = new(); // мониторинг сущностей

    public IServiceCollection Services => _services;
    public IEntityCollection Entities => _entitiesCollection;

    // игровые функции
    public List<UpdateDelegate> Updates { get => UpdateCollection; set => UpdateCollection = value; }
    public List<OnKeyPressDelegate> OnKeyPresses { get => OnKeyPressCollection; set => OnKeyPressCollection = value; }
    public List<DrawDelegate> Draws { get => DrawCollection; set => DrawCollection = value; }

    // Создание фабрики для сущности
    public void UseGameEnvironment<T>(GameFunctions gameFunctions = GameFunctions.All) where T : class, IGameEnvironment, new()
    {
        _environmentFactories.Add(typeof(T), () => new T());
        _environmentGameFunctions.Add(typeof(T), gameFunctions);
    }

    // регистрация игровых функций сущности
    private static void RegisterEnvitonmentFunctions(GameApplication gameApplication, object entity, GameFunctions gameFunctions)
    {
        if (gameFunctions.HasFlag(GameFunctions.Update) && entity is IUpdateable u) gameApplication.UpdateCollection.Add(u.Update);
        if (gameFunctions.HasFlag(GameFunctions.OnKeyPress) && entity is IControl c) gameApplication.OnKeyPressCollection.Add(c.OnKeyPress);
        if (gameFunctions.HasFlag(GameFunctions.Draw) && entity is IDrawable d) gameApplication.DrawCollection.Add(d.Draw);
    }

    public GameApplication Build()
    {
        var game = new GameApplication(this)
        {
            Services = _services.BuildServiceProvider(),
            Entities = _entitiesCollection.BuildEntityProvider()
        };

        List<object> environments = new(_environmentFactories.Count);
        foreach ((Type environmentType, Func<object> factory) in _environmentFactories)
        {
            object environment = factory.Invoke();
            RegisterEnvitonmentFunctions(game, environment, _environmentGameFunctions[environmentType]);
            game.Entities.TryReleaseEntity(environmentType, environment);
            environments.Add(environment);
        }
        foreach (object environment in environments)
        {
            if (environment is IInitializableEnvironment initializableEnvironment)
            {
                initializableEnvironment.Initialize(game.GameContext);
            }
        }

        game.AfterStop += () =>
        {
            foreach (var env in environments)
            {
                if (env is IDisposable disposable)
                    disposable.Dispose();
            }
        };

        return game;
    }
}