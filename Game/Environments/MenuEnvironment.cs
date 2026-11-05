using System;
using System.Collections.Generic;
using Game.Display;
using TerminaCore.Debug;
using TerminaCore.DOMCore;
using TerminaCore.DOMCore.Enums;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class MenuEnvironment : GameEnvironmentBase, IDrawable, IControl
{
    public Radio Radio { get; }
    public ModelTree MenuTree { get; set; } = null!;

    public MenuEnvironment()
    {
        Radio = new Radio(12, 3);
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        switch (controlContext.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                Radio.TryPrevIndex();
                break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                Radio.TryNextIndex();
                break;

            case ConsoleKey.Enter:
                Radio.ClickFocusedButton();
                break;
        }
    }


    public void Draw(DrawContext context)
    {
        context.Draw(Radio.Texture);
        foreach (var btn in Radio.Buttons)
        {
            context.Draw(btn.Texture);
        }
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        var canvas = context.GetCanvasRequired();

        // Radio
        string[] btns = new string[3] { "[ ] New Game".PadRight(12), "[ ] Load".PadRight(12), "[ ] Exit".PadRight(12) };

        Radio.AddButton(btns[0], () => context.SetState("new_game"));
        Radio.AddButton(btns[1], () => context.SetState("load"));
        Radio.AddButton(btns[2], () => context.Stop());

        Radio.OnButtonFocusIn += (btn, i) =>
        {
            btn.Texture.SetSymbol(0, 1, '>');
            btn.Texture.ForegroundColor = ConsoleColor.DarkRed;
            btn.Texture.BackgroundColor = ConsoleColor.Gray;
        };
        Radio.OnButtonFocusOut += (btn, i) =>
        {
            btn.Texture.SetSymbol(0, 1, ' ');
            btn.Texture.ForegroundColor = ConsoleColor.Gray;
            btn.Texture.BackgroundColor = null;
        };
        for (int i = 1; i < Radio.Buttons.Count; i++)
            Radio.Buttons[i].FocusOut();
        Radio.Buttons[0].FocusIn();

        // Nodes

        MenuTree = new ModelTree(canvas);

        var radioNode = new ModelNode(Radio.Texture);
        var radioButtonsNodes = Radio.Buttons.Select(btn => new ModelNode(btn.Texture));

        foreach (var rBtn in radioButtonsNodes) radioNode.AddChild(rBtn);
        MenuTree.Body.AddChild(radioNode);

        // Style

        MenuTree.Body.ApplyStyle(style =>
        {
            style.Display = Display.Flex;
            style.FlexPath = FlexPath.FlexColumn;
            style.JustifyContent = FlexPosition.Center;
            style.FlexPosition = FlexPosition.Center;
        });

        radioNode.ApplyStyle(style =>
        {
            style.Display = Display.Flex;
            style.FlexPath = FlexPath.FlexColumn;
            style.FlexPosition = FlexPosition.Start;
        });

        MenuTree.Update();
    }
}