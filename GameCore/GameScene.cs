using System.Reflection;
using GameCore.Abstract;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Contexts;
using GameCore.Delegates;

namespace GameCore;

public class GameScene : IGameScene
{
    private UpdateDelegate? _update;
    private DrawDelegate? _draw;
    private OnKeyPressDelegate? _onKeyPress;
    private KeyValuePair<Type, Func<GameContext, object>> _factoryItem;

    public UpdateDelegate? Update { get => _update; set => _update = value; }
    public DrawDelegate? Draw { get => _draw; set => _draw = value; }
    public OnKeyPressDelegate? OnKeyPress { get => _onKeyPress; set => _onKeyPress = value; }
    public KeyValuePair<Type, Func<GameContext, object>> FactoryItem { get => _factoryItem; }

    public static GameScene FromEntity<T>() where T : class, IEntity
    {
        var gameScene = new GameScene();

        Type type = typeof(T);
        if (type.GetConstructor([typeof(GameContext)]) is ConstructorInfo ctor)
            gameScene.UseEntity<T>(gameContext => ctor.Invoke([gameContext]));
        else if (type.GetConstructor(Type.EmptyTypes) is ConstructorInfo emptyCtor)
            gameScene.UseEntity<T>(_ => emptyCtor.Invoke(null));

        return gameScene;
    }

    public void UseEntity<T>(Func<GameContext, object> factory) where T : class, IEntity
    {
        _factoryItem = new KeyValuePair<Type, Func<GameContext, object>>(typeof(T), factory);
    }
}
