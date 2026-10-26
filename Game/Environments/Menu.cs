using System;
using System.Collections.Generic;
using TerminaCore.Debug;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class Menu : GameEnvironmentBase, IDrawable, IControl
{
    public Options MenuOptions { get; }

    private int _index = 0;
    public int Index
    {
        get => _index;
        set
        {
            if (value < 0) _index = MenuOptions.Count - 1;
            else if (value >= MenuOptions.Count) _index = 0;
            else _index = value;
        }
    }

    public Menu()
    {
        MenuOptions = new(12, 3);
    }

    public void IndexChangeHandler(int indexOut, int indexOn)
    {
        MenuOptions.Selects[indexOut].SelectOut();
        MenuOptions.Selects[indexOn].SelectIt();
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        switch (controlContext.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                IndexChangeHandler(Index--, Index);
                break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                IndexChangeHandler(Index++, Index);
                break;

            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                MenuOptions.Selects[Index].Input();
                break;
        }
    }


    public void Draw(DrawContext context)
    {
        foreach (var select in MenuOptions.Selects)
        {
            context.Draw(select.Texture);
        }
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        int width = context.GetCanvasWidth();
        int height = context.GetCanvasHeight();

        Style.ToPosition(MenuOptions.GetTexture(), width, height, Position.Center, Position.Center);
        MenuOptions.GetTexture().Y += 5;

        MenuOptions.ConfigureSelect((int index, Select select) =>
        {
            select.Texture.IgnoreSet.Clear();
            select.Text = select.Text.PadLeft(8);

            select.Texture.Y = select.Texture.Y + MenuOptions.GetTexture().Y + index;
            select.Texture.X = select.Texture.X + MenuOptions.GetTexture().X;
        });


        MenuOptions.AddSelect("New Game", onInput: (select) => context.SetState("new_game"));
        MenuOptions.AddSelect("Load", onInput: (select) => context.SetState("load"));
        MenuOptions.AddSelect("Exit", onInput: (select) => context.Stop());

        MenuOptions.OnSelectOn += (select) =>
        {
            select.Texture.SetTexture($"[>] {select.Text}", ConsoleColor.DarkRed, ConsoleColor.Gray);
        };
        MenuOptions.OnSelectOut += (select) =>
        {
            select.Texture.SetTexture($"[ ] {select.Text}", ConsoleColor.Gray, null);
        };
        for (int i = 0; i < MenuOptions.Count; i++)
        {
            MenuOptions.Selects[i].SelectOut();
        }

        MenuOptions.Selects[Index].SelectIt();
    }
}