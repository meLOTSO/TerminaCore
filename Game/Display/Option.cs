using Game.Display.Abstract;
using TerminaCore.Graphics;

public class Option : IOption
{
    private List<ISelect> _selects = [];

    public Texture Texture { get; set; }
    public IReadOnlyList<ISelect> Selects => _selects;

    public event Action<ISelect>? OnSelectAdd;
    public event Action<ISelect>? OnSelectRemove;

    public Option(int width, int height)
    {
        this.Texture = new Texture(width, height);
        Texture.Tag = this;
    }

    public Option(Texture texture)
    {
        this.Texture = texture;
        Texture.Tag = this;
    }

    public void AddSelect(ISelect select)
    {
        OnSelectAdd?.Invoke(select);
        _selects.Add(select);
    }
    public void AddSelect(Texture texture)
    {
        var select = new Select(texture);
        OnSelectAdd?.Invoke(select);
        _selects.Add(select);
    }
    public void AddSelect(string text)
    {
        var select = new Select(new Texture(text));
        OnSelectAdd?.Invoke(select);
        _selects.Add(select);
    }

    public void RemoveSelect(ISelect select)
    {
        _selects.Remove(select);
        OnSelectRemove?.Invoke(select);
    }
    public void RemoveSelect(Texture texture)
    {
        var select = _selects.FirstOrDefault(s => s.Texture == texture);
        if (select is null) return;
        _selects.Remove(select);
        OnSelectRemove?.Invoke(select);
    }
    public void RemoveSelect(string text)
    {
        var select = _selects.FirstOrDefault(s => s.Texture.Buffer.ToString()?.Trim() == text);
        if (select is null) return;
        _selects.Remove(select);
        OnSelectRemove?.Invoke(select);
    }
}