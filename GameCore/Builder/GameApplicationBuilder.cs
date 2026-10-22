using EntityCore;
using EntityCore.Abstract;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Abstract;
using GameCore.Delegates;
using ServiceCore;
using ServiceCore.Abstract;
using GameCore.Contexts;
using System.Reflection;

namespace GameCore.Builder;


public class GameApplicationBuilder : GameBase
{
    private readonly Dictionary<Type, Func<object>> _environmentFactories = new(); // object Factory(GameContext context)
    private readonly ServiceCollection _services = new(); // сервисы
    private readonly EntityCollection _entitiesCollection = new(); // мониторинг сущностей

    public IServiceCollection Services => _services;
    public IEntityCollection Entities => _entitiesCollection;

    // игровые функции
    public List<UpdateDelegate> Updates { get => UpdateCollection; set => UpdateCollection = value; }
    public List<OnKeyPressDelegate> OnKeyPresses { get => OnKeyPressCollection; set => OnKeyPressCollection = value; }
    public List<DrawDelegate> Draws { get => DrawCollection; set => DrawCollection = value; }

    // Создание фабрики для сущности
    public void UseGameEnvironment<T>() where T : class, IGameEnvironment, new()
    {
        _environmentFactories.Add(typeof(T), () => new T());
    }

    // регистрация игровых функций сущности
    private static void RegisterEnvitonmentFunctions(GameApplication gameApplication, object entity)
    {
        if (entity is IUpdateable u) gameApplication.UpdateCollection.Add(u.Update);
        if (entity is IControl c) gameApplication.OnKeyPressCollection.Add(c.OnKeyPress);
        if (entity is IDrawable d) gameApplication.DrawCollection.Add(d.Draw);
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
            RegisterEnvitonmentFunctions(game, environment);
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

        return game;
    }
}