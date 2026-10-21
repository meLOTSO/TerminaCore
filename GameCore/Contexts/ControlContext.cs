namespace GameCore.Contexts;

public class ControlContext : GameCore.Abstract.ContextBase
{
    public ConsoleKey Key { get; set; }
    public ConsoleModifiers Modifiers { get; set; }
    public bool Pressed { get; set; }

    public ControlContext(GameApplication gameApplication) : base(gameApplication)
    {
    }
}