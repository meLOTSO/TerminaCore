namespace Game.Display.Abstract;

public interface IMouseMoveReactable
{
    public event Action? OnFocusIn;
    public event Action? OnFocusOut;
    public event Action? OnMouseMove;

    public void FocusIn();
    public void FocusOut();
    public void MouseMove();
}