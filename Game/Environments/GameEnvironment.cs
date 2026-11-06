using Game.Display;
using Game.Textures;
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

        GameNode = new ModelNode(MapTextures.GetSea(canvas.Width, canvas.Height));
        ModelNode ground = new(MapTextures.GetGround(canvas.Width, canvas.Height, ConsoleColor.DarkGreen, 2700, Enums.RenderType.Random));
        ModelNode snow = new(MapTextures.GetGround(canvas.Width, canvas.Height, ConsoleColor.DarkGray, 100, Enums.RenderType.Random));
        snow.Style.Position = TerminaCore.DOMCore.Enums.Position.Absolute;
        ModelNode nature = new(MapTextures.GetGround(canvas.Width, canvas.Height, ConsoleColor.White, 300, Enums.RenderType.Random));
        nature.Style.Position = TerminaCore.DOMCore.Enums.Position.Absolute;

        GameNode.AddChild(ground);
        GameNode.AddChild(nature);
        GameNode.AddChild(snow);

        GameNode.UpdateStyleBFS();
    }
}