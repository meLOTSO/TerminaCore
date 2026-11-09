using Game.Display.Abstract;
using TerminaCore.Graphics;

public class Select : ISelect
{
    private bool _selected = false;
    private bool _active = false;

    public Texture Texture { get; set; }
    public event Action<ISelect>? OnSelect;
    public event Action<ISelect>? OnSelectOut;
    public event Action<ISelect>? OnActiveIn;
    public event Action<ISelect>? OnActiveOut;

    public bool IsSelected => _selected;
    public bool IsActive => _active;

    public Select(string text)
    {
        var texture = new Texture(text) { IgnoreSet = new() };
        this.Texture = texture;
        this.Texture.Tag = this;
        OnActiveIn += (select) => _active = true;
        OnActiveOut += (select) => _active = false;
    }

    public void SelectThis()
    {
        _selected = true;
        OnSelect?.Invoke(this);
    }

    public void SelectOut()
    {
        _selected = false;
        OnSelectOut?.Invoke(this);
    }
}