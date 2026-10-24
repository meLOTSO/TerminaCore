using TerminaCore.Graphics;

public class Select
{
    private bool _selected = false;

    public Texture Texture { get; private set; }
    public string Text { get; private set; }
    public event Action<Select>? OnSelectOn;
    public event Action<Select>? OnSelectOut;
    public event Action<Select>? OnInput;
    public bool IsSelected
    {
        get => _selected;
        set
        {
            _selected = value;
            if (value) OnSelectOn?.Invoke(this);
            else OnSelectOut?.Invoke(this);
        }
    }

    public Select(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        Texture = new Texture(text, foregroundColor, backgroundColor);
        Text = text;
    }

    public void SelectIt() => IsSelected = true;
    public void SelectOut() => IsSelected = false;
    public void Input() => OnInput?.Invoke(this);
}