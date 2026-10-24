using System;
using System.Collections.Generic;
using TerminaCore.GameCore.Abstract;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Delegates;
using TerminaCore.GameCore.Enums;

namespace TerminaCore.GameCore;

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

    public GameFunctions GameFunctions { get; set; } = GameFunctions.All;

    public static GameScene FromGameEnvironment<T>(GameFunctions gameFunctions = GameFunctions.All) where T : class, IGameEnvironment, new()
    {
        var gameScene = new GameScene();
        gameScene.UseGameEnvironment<T>(() => new T());
        gameScene.GameFunctions = gameFunctions;
        return gameScene;
    }

    public void UseGameEnvironment<T>(Func<object> factory) where T : class, IGameEnvironment
    {
        _factoryItem = new KeyValuePair<Type, Func<object>>(typeof(T), factory);
    }
}
