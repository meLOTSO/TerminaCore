using System;
using System.Collections.Generic;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class Menu : GameEnvironmentBase, IDrawable, IControl
{
    // private readonly string[] _buttonLabels = ["New Game".PadLeft(8), "Load".PadLeft(8), "Exit".PadLeft(8)];

    // public List<Texture> Buttons { get; } = [];
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
        // Формируем кнопки из единого массива строк
        // for (int i = 0; i < _buttonLabels.Length; i++)
        // {
        //     char[] buffer = $"[ ] {_buttonLabels[i]}".ToArray();
        //     var btn = new Texture(buffer) { Y = i };
        //     btn.IgnoreSet.Clear(); // кнопки отрисовываются плотно
        //     Buttons.Add(btn);
        // }
        MenuOptions = new(12, 3);

        // Подсвечиваем начальный выбранный пункт
        // HighlightCurrentButton();
    }

    // private void HighlightCurrentButton()
    // {
    //     for (int i = 0; i < MenuOptions.Count; i++)
    //     {
    //         if (i == Index)
    //         {
    //             Buttons[i].SetTexture($"[>] {_buttonLabels[i]}", ConsoleColor.DarkRed, ConsoleColor.Gray);
    //         }
    //         else
    //         {
    //             Buttons[i].SetTexture($"[ ] {_buttonLabels[i]}", ConsoleColor.Gray, null);
    //         }
    //     }
    // }

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
                // Index--;
                IndexChangeHandler(Index--, Index);
                // HighlightCurrentButton();
                break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                // Index++;
                IndexChangeHandler(Index++, Index);
                // HighlightCurrentButton();
                break;

            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                // ExecuteSelectedAction();
                MenuOptions.Selects[Index].Input();
                break;
        }
    }

    // private void ExecuteSelectedAction()
    // {
    //     switch (Index)
    //     {
    //         case 0:
    //             GameContext.SetState("new_game");
    //             break;

    //         case 1:
    //             GameContext.SetState("load");
    //             break;

    //         case 2:
    //             GameContext.Stop();
    //             break;
    //     }
    // }

    public void Draw(DrawContext context)
    {
        // foreach (var btn in Buttons)
        // {
        //     context.Draw(btn);
        // }
        // context.Draw(MenuOptions.GetTexture());
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

        // (int x, int y) = Style.GetPosition(Buttons[0], width, height, Position.Center, Position.Center);
        // for (int i = 0; i < _buttonLabels.Length; i++)
        // {
        //     Buttons[i].Y = y + i;
        //     Buttons[i].X = x;
        // }

        // var canvas = context.Services.GetServiceRequired<Canvas>();
        // canvas.DefaultBackground = ConsoleColor.DarkRed;
        var menuSelect = new Select("New Game".PadLeft(8));
        (int x, int y) = Style.ToPosition(menuSelect.Texture, width, height, Position.Center, Position.Center);

        var loadSelect = new Select("Load".PadLeft(8));
        loadSelect.Texture.X = x;
        loadSelect.Texture.Y = y + 1;

        var exitSelect = new Select("Exit".PadLeft(8));
        exitSelect.Texture.X = x;
        exitSelect.Texture.Y = y + 2;

        MenuOptions.AddSelect(menuSelect,
            onInput: (select) =>
            {
                context.SetState("new_game");
            });
        MenuOptions.AddSelect(loadSelect,
            onInput: (select) =>
            {
                context.SetState("load");
            });
        MenuOptions.AddSelect(exitSelect,
            onInput: (select) =>
            {
                context.Stop();
            });

        for (int i = 0; i < MenuOptions.Count; i++)
        {
            MenuOptions.Selects[i].Texture.IgnoreSet.Clear();
        }

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