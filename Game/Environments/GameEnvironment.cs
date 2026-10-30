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
    public ModelNode Body { get; set; } = null!;
    public Button BackButton { get; set; } = new(new("[BACK]", ConsoleColor.DarkMagenta, ConsoleColor.Gray));

    public void OnKeyPress(ControlContext controlContext)
    {

    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        Body = new(new Texture(context.GetCanvasHeight(), context.GetCanvasWidth()));

        Body.ApplyStyle(style =>
        {
            style.Display = TerminaCore.DOMCore.Enums.Display.Flex;
            style.FlexPosition = FlexPosition.Center;
            style.FlexPath = FlexPath.FlexColumn;
        });

        var bodyInner = new ModelNode(new Texture(4, context.GetCanvasWidth()));

        bodyInner.ApplyStyle(style =>
        {
            style.Display = TerminaCore.DOMCore.Enums.Display.Flex;
            style.FlexPosition = FlexPosition.SpaceBetween;
        });

        var i1 = new ModelNode(new Texture("game", ConsoleColor.Green, ConsoleColor.Black));
        var i2 = new ModelNode(new Texture("1234", ConsoleColor.Green, ConsoleColor.Gray));
        var i3 = new ModelNode(new Texture("over", ConsoleColor.Green, ConsoleColor.Gray));
        var i4 = new ModelNode(new Texture("false", ConsoleColor.Green, ConsoleColor.Gray));

        bodyInner.AddChild(i1);
        bodyInner.AddChild(i2);
        bodyInner.AddChild(i3);
        bodyInner.AddChild(i4);

        Body.AddChild(bodyInner);

        var dom = context.GetDOM();
        dom.Body.AddChild(Body);

        BackButton.Texture.IgnoreSet.Clear();
        BackButton.OnClick += () => context.SetState("menu");
        ConsoleColor? color = BackButton.Texture.ForegroundColor;
        ConsoleColor? backgroundColor = BackButton.Texture.BackgroundColor;
        Texture defaultTexture = BackButton.Texture;
        BackButton.OnMouseIn += () =>
        {
            BackButton.Texture.ForegroundColor = ConsoleColor.Gray;
            BackButton.Texture.BackgroundColor = ConsoleColor.DarkMagenta;
        };
        BackButton.OnMouseOut += () =>
        {
            BackButton.Texture.ForegroundColor = color;
            BackButton.Texture.BackgroundColor = backgroundColor;
        };
    }

    public void Draw(DrawContext context)
    {
        context.Draw(Body.Texture);
        foreach (var child in Body.Childs)
        {
            context.Draw(child.Texture);
            foreach (var c in child.Childs)
            {
                context.Draw(c.Texture);
            }
        }
        context.Draw(BackButton.Texture);
    }
}