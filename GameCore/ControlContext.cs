using GameCore;

public class ControlContext : ContextBase
{
    public ConsoleKey Key { get; set; }
    public ConsoleModifiers Modifiers { get; set; }
    public bool Pressed { get; set; }

    public ControlContext(GameApplication gameApplication) : base(gameApplication)
    {
    }
}