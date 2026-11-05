using Game.Display.Abstract;
using TerminaCore.Graphics;

public class Select : ISelect
{
    private bool _selected = false;
    private bool _active = false;

    public Texture Texture { get; set; }
    public event Action<ISelect>? OnSelect;
    public event Action<ISelect>? OnActiveIn;
    public event Action<ISelect>? OnActiveOut;

    public bool IsSelected => _selected;
    public bool IsActive => _active;

    public Select(Texture texture)
    {
        this.Texture = texture;
        this.Texture.Tag = this;
        OnSelect += (select) => _selected = true;
        OnActiveIn += (select) => _active = true;
        OnActiveOut += (select) => _active = false;
    }
    public Select(string text) : this(new Texture(text))
    {
    }

    public void SelectThis() => OnSelect?.Invoke(this);
}