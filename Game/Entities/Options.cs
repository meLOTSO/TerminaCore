using TerminaCore.Graphics;

public class Options
{
    private Canvas _canvas;

    public List<Select> Selects { get; private set; } = [];
    public event Action<Select>? OnSelectOn;
    public event Action<Select>? OnSelectOut;
    public event Action<Select>? OnSelectInput;
    public int Count => Selects.Count;

    public Options(int width, int height, char backgroundChar = ' ', ConsoleColor defaultFg = default, ConsoleColor defaultBg = default)
    {
        _canvas = new Canvas(height, width, backgroundChar, defaultFg, defaultBg);
    }

    public Texture GetTexture()
    {
        for (int i = 0; i < Selects.Count; i++)
        {
            Selects[i].Texture.Y = i;
            _canvas.Draw(Selects[i].Texture);
        }
        return _canvas.Render();
    }

    public void AddSelect(Select select,
        Action<Select>? onInput = null,
        Action<Select>? onSelectOn = null,
        Action<Select>? onSelectOff = null,
        bool isSelected = false)
    {
        select.OnInput += (sel) => OnSelectInput?.Invoke(sel);
        select.OnSelectOn += (sel) => OnSelectOn?.Invoke(sel);
        select.OnSelectOut += (sel) => OnSelectOut?.Invoke(sel);

        if (onInput is not null) select.OnInput += onInput;
        if (onSelectOn is not null) select.OnSelectOn += onSelectOn;
        if (onSelectOff is not null) select.OnSelectOut += onSelectOff;
        select.IsSelected = isSelected;

        Selects.Add(select);
    }

    public void AddSelect(string text,
        Action<Select>? onInput = null,
        Action<Select>? onSelectOn = null,
        Action<Select>? onSelectOff = null,
        bool isSelected = false,
        ConsoleColor? foregroundColor = null,
        ConsoleColor? backgroundColor = null)
    {
        var select = new Select(text, foregroundColor, backgroundColor);
        AddSelect(select, onInput, onSelectOn, onSelectOff, isSelected);
    }
}