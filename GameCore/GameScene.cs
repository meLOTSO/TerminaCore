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
    private KeyValuePair<Type, Func<object>> _factoryItem;

    public UpdateDelegate? Update { get => _update; set => _update = value; }
    public DrawDelegate? Draw { get => _draw; set => _draw = value; }
    public OnKeyPressDelegate? OnKeyPress { get => _onKeyPress; set => _onKeyPress = value; }
    public KeyValuePair<Type, Func<object>> FactoryItem { get => _factoryItem; }

    public static GameScene FromGameEnvironment<T>() where T : class, IGameEnvironment, new()
    {
        var gameScene = new GameScene();
        gameScene.UseGameEnvironment<T>(() => new T());
        return gameScene;
    }

    public void UseGameEnvironment<T>(Func<object> factory) where T : class, IGameEnvironment
    {
        _factoryItem = new KeyValuePair<Type, Func<object>>(typeof(T), factory);
    }
}
