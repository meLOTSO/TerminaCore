using System;
using System.Collections.Generic;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class Menu : GameEnvironmentBase, IDrawable, IControl
{
    private readonly string[] _buttonLabels = ["New Game".PadLeft(8), "Load".PadLeft(8), "Exit".PadLeft(8)];

    public List<Texture> Buttons { get; } = [];

    private int _index = 0;
    public int Index
    {
        get => _index;
        set
        {
            if (value < 0) _index = Buttons.Count - 1;
            else if (value >= Buttons.Count) _index = 0;
            else _index = value;
        }
    }

    public Menu()
    {
        // Формируем кнопки из единого массива строк
        for (int i = 0; i < _buttonLabels.Length; i++)
        {
            char[] buffer = $"[ ] {_buttonLabels[i]}".ToArray();
            var btn = new Texture(buffer) { Y = i };
            btn.IgnoreSet.Clear(); // кнопки отрисовываются плотно
            Buttons.Add(btn);
        }

        // Подсвечиваем начальный выбранный пункт
        HighlightCurrentButton();
    }

    private void HighlightCurrentButton()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            if (i == Index)
            {
                Buttons[i].SetTexture($"[>] {_buttonLabels[i]}", ConsoleColor.DarkRed, ConsoleColor.Gray);
            }
            else
            {
                Buttons[i].SetTexture($"[ ] {_buttonLabels[i]}", ConsoleColor.Gray, null);
            }
        }
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        switch (controlContext.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                Index--;
                HighlightCurrentButton();
                break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                Index++;
                HighlightCurrentButton();
                break;

            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                ExecuteSelectedAction();
                break;
        }
    }

    private void ExecuteSelectedAction()
    {
        switch (Index)
        {
            case 0:
                GameContext.SetState("new_game");
                break;

            case 1:
                GameContext.SetState("load");
                break;

            case 2:
                GameContext.Stop();
                break;
        }
    }

    public void Draw(DrawContext context)
    {
        foreach (var btn in Buttons)
        {
            context.Draw(btn);
        }
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        int width = context.GetCanvasWidth();
        int height = context.GetCanvasHeight();

        (int x, int y) = Style.GetPosition(Buttons[0], width, height, Position.Center, Position.Center);
        for (int i = 0; i < _buttonLabels.Length; i++)
        {
            Buttons[i].Y = y + i;
            Buttons[i].X = x;
        }

        var canvas = context.Services.GetServiceRequired<Canvas>();
        canvas.DefaultBackground = ConsoleColor.DarkRed;
    }
}