using System.Text;
using GameCore;
using GameCore.Abstract.GameBasicFunctional;
using GameCore.Extensions;

public class Entity : IControl, IDrawable
{
    private int _currentIndex = 0;

    public List<string> Menu { get; set; } = [];
    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value >= Menu.Count)
                _currentIndex = 0;
            else if (value < 0)
                _currentIndex = Menu.Count - 1;
            else
                _currentIndex = value;
        }
    }

    public Entity()
    {
        Menu = new List<string>()
        {
            "Start", "Settings", "Exit"
        };
    }

    public void OnKeyPress(GameContext context, ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.DownArrow)
            CurrentIndex++;
        else if (controlContext.Key == ConsoleKey.UpArrow)
            CurrentIndex--;
        else if (controlContext.Key == ConsoleKey.Enter)
        {
            switch (CurrentIndex)
            {
                case 0: context.SetState("game"); break;
                case 1: context.SetState("setting"); break;
                case 2: context.Stop(); break;
                default: break;
            }
        }
    }
    public void Draw(DrawContext context)
    {
        for (int i = 0; i < Menu.Count; i++)
        {
            if (i == CurrentIndex)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"> {Menu[i].PadRight(10)}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{Menu[i].PadRight(12)}");
            }
        }
    }
}