namespace Game.Display.Abstract;

public interface ITabable
{
    public event Action? OnTabIn;
    public event Action? OnTabOut;

    public void TabThis();
    public void TabOut();
    public bool IsTabbed();
}
