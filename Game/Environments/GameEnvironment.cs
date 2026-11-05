using Game.Display;
using TerminaCore.DOMCore;
using TerminaCore.DOMCore.Enums;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

namespace Game.Environments;

public class GameEnvironment : GameEnvironmentBase, IControl, IDrawable
{
    public ModelNode GameNode { get; set; } = null!;
    public Button BackButton { get; set; } = new(new("[BACK]", ConsoleColor.DarkMagenta, ConsoleColor.Gray));

    public void OnKeyPress(ControlContext controlContext)
    {

    }

    public void Draw(DrawContext context)
    {
        GameNode.ForEachBFS(node => context.Draw(node.Texture));
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        var canvas = context.GetCanvasRequired();

        GameNode = new ModelNode(new Texture(canvas.Width, canvas.Height));

        Texture seaTexture = new Texture(canvas.Width, canvas.Height, foregroundColor: ConsoleColor.Blue, backgroundColor: ConsoleColor.DarkCyan) { IgnoreSet = new() };
        for (int i = 0; i < 20; i++)
        {
            int randX = Random.Shared.Next(0, canvas.Width - 1);
            int randY = Random.Shared.Next(0, canvas.Height - 1);
            while (seaTexture[randY, randX] == '~')
                (randX, randY) = (Random.Shared.Next(0, canvas.Width - 1), Random.Shared.Next(0, canvas.Height - 1));
            seaTexture[randY, randX] = '~';
        }

        ModelNode sea = new(seaTexture);
        ModelNode spawn = new(new Texture("&&&", ConsoleColor.DarkBlue));

        GameNode.AddChild(sea);
        sea.AddChild(spawn);

        GameNode.UpdateStyleBFS();
    }
}