using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using Game.Display.Abstract;
using Game.Display;

namespace Game.Environments.DisplayInteractive;

public class TabEnvironment : GameEnvironmentBase, IControl
{
    private TabCollection _tabCollection = new();

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Tab)
        {
            _tabCollection.TabNext();
        }
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        var canvas = context.GetCanvasRequired();
        Action updateTabableTextures = () =>
        {
            _tabCollection.Clear();
            canvas.ForEach(t =>
            {
                if (t.Tag is ITabable tabable)
                    _tabCollection.AddTabableEntity(tabable);
            });
        };
        updateTabableTextures.Invoke();
        context.AddOnStateChangedHandler(stateContext => updateTabableTextures.Invoke());
    }
}