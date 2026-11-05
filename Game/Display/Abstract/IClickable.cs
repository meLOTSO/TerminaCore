namespace Game.Display.Abstract;

public interface IClickable
{
    public event Action? OnClick;

    public void Click();
}