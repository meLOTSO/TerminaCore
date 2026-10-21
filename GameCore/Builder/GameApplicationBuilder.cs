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
    private readonly Dictionary<Type, Func<GameContext, object>> _environmentFactories = new(); // object Factory(GameContext context)
    private readonly ServiceCollection _services = new(); // сервисы
    private readonly EntityCollection _entitiesCollection = new(); // мониторинг сущностей

    public IServiceCollection Services => _services;
    public IEntityCollection Entities => _entitiesCollection;

    // игровые функции
    public List<UpdateDelegate> Updates { get => UpdateCollection; set => UpdateCollection = value; }
    public List<OnKeyPressDelegate> OnKeyPresses { get => OnKeyPressCollection; set => OnKeyPressCollection = value; }
    public List<DrawDelegate> Draws { get => DrawCollection; set => DrawCollection = value; }

    // Создание фабрики для сущности
    public void UseGameEnvironment<T>() where T : class, IGameEnvironment
    {
        // Сущность, которому передается конструктор GameContext, не может быть создана без передачи параметра, поэтому сохраняем фабрику для создания сущности
        if (typeof(T).GetConstructor([typeof(GameContext)]) is ConstructorInfo ctor)
        {
            _environmentFactories.Add(typeof(T), (context) => ctor.Invoke([context]));
            return;
        }
        // Сущность с конструктором без параметров создаем и сразу регистрируем игровые функции
        else if (typeof(T).GetConstructor(Type.EmptyTypes) is ConstructorInfo emptyCtor)
        {
            _environmentFactories.Add(typeof(T), _ => emptyCtor.Invoke(null));
            return;
        }
        else
        {
            throw new Exception("Сушность должна иметь конструктор без параметров или констуктор с одним параметром GameContext.");
        }
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
        foreach ((Type entityType, Func<GameContext, object> factory) in _environmentFactories)
        {
            object entity = factory.Invoke(game.GameContext);
            RegisterEnvitonmentFunctions(game, entity);
            game.Entities.TryReleaseEntity(entityType, entity);
        }

        return game;
    }
}