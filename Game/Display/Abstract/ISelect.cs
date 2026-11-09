using TerminaCore.Graphics;

namespace Game.Display.Abstract;

public interface ISelect
{
    public Texture Texture { get; set; }
    public event Action<ISelect>? OnSelect;
    public event Action<ISelect>? OnSelectOut;
    public event Action<ISelect>? OnActiveIn;
    public event Action<ISelect>? OnActiveOut;

    public bool IsSelected { get; }
    public bool IsActive { get; }

    public void SelectThis();
    public void SelectOut();
}