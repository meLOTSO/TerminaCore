using TerminaCore.Graphics;
using Game.Display.Abstract;

namespace Game.Display;

public class Button : IClickable, IMouseMoveReactable
{
    public Texture Texture { get; set; }
    public event Action? OnFocusIn;
    public event Action? OnFocusOut;
    public event Action? OnMouseMove;
    public event Action? OnClick;

    public Button(Texture texture)
    {
        Texture = texture;
        Texture.Tag = this;
    }

    public virtual void Click()
    {
        OnClick?.Invoke();
    }

    public void FocusIn() => OnFocusIn?.Invoke();
    public void FocusOut() => OnFocusOut?.Invoke();
    public void MouseMove() => OnMouseMove?.Invoke();
}