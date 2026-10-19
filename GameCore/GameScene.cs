using GameCore.Abstract.GameBasicFunctional;
using GameCore.Delegates;

namespace GameCore;

public class GameScene : IGameScene
{
    private UpdateDelegate? _update;
    private DrawDelegate? _draw;
    private OnKeyPressDelegate? _onKeyPress;

    public static GameScene FromEntity<T>(T entity) where T : class, IEntityFunc
    {
        var gameScene = new GameScene();
        if (entity is IUpdateable u) gameScene.Update = u.Update;
        if (entity is IControl c) gameScene.OnKeyPress = c.OnKeyPress;
        if (entity is IDrawable d) gameScene.Draw = d.Draw;
        return gameScene;
    }

    public UpdateDelegate? Update { get => _update; set => _update = value; }
    public DrawDelegate? Draw { get => _draw; set => _draw = value; }
    public OnKeyPressDelegate? OnKeyPress { get => _onKeyPress; set => _onKeyPress = value; }
}
