using TerminaCore.Graphics;


namespace Game.Display;

public class Button : Game.Display.Abstract.IClickable, Game.Display.Abstract.IMouseMoveReactable
{
    public Texture Texture { get; set; }
    public event Action? OnMouseIn;
    public event Action? OnMouseOut;
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

    public void MouseIn() => OnMouseIn?.Invoke();
    public void MouseOut() => OnMouseOut?.Invoke();
    public void MouseMove() => OnMouseMove?.Invoke();
}