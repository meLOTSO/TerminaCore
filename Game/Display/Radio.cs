using Game.Display;
using TerminaCore.Graphics;

public class Radio
{
    private int _index = 0;

    private readonly Dictionary<Button, (Action OnFocusIn, Action OnFocusOut, Action OnClick)> _handlers = new();

    public Texture Texture { get; set; }
    public List<Button> Buttons { get; } = [];
    public int Index { get => _index; }
    public int ButtonsCount => Buttons.Count;
    public event Action<Button, int>? OnButtonFocusIn;
    public event Action<Button, int>? OnButtonFocusOut;
    public event Action<Button, int>? OnFocusedButtonClick;

    public Radio(Texture texture)
    {
        Texture = texture;
    }

    public Radio(int width, int height) : this(new Texture(width, height))
    {
    }

    public void AddButton(Button button)
    {
        Action onFocusIn = () => OnButtonFocusIn?.Invoke(button, Buttons.IndexOf(button));
        Action onFocusOut = () => OnButtonFocusOut?.Invoke(button, Buttons.IndexOf(button));
        Action onClick = () => OnFocusedButtonClick?.Invoke(button, Buttons.IndexOf(button));

        button.OnFocusIn += onFocusIn;
        button.OnFocusOut += onFocusOut; ;
        button.OnClick += onClick;

        _handlers[button] = (onFocusIn, onFocusOut, onClick);
        Buttons.Add(button);
    }

    public void AddButton(Texture texture, Action? onClick)
    {
        var button = new Button(texture);
        if (onClick is not null) button.OnClick += onClick;
        AddButton(button);
    }

    public void AddButton(string text, Action? onClick)
    {
        var t = new Texture(text);
        t.IgnoreSet.Clear();
        AddButton(t, onClick);
    }

    public bool RemoveButton(Button button)
    {
        var removed = Buttons.Remove(button);
        if (removed)
        {
            var (OnFocusIn, OnFocusOut, OnClick) = _handlers[button];
            button.OnFocusIn -= OnFocusIn;
            button.OnFocusOut -= OnFocusOut;
            button.OnClick -= OnClick;
        }
        return removed;
    }
    public bool RemoveButton(Texture texture)
    {
        var button = Buttons.FirstOrDefault(btn => btn.Texture == texture);
        return button is not null && RemoveButton(button);
    }

    public bool RemoveButton(string text)
    {
        var trimmedText = text.Trim();

        var button = Buttons.FirstOrDefault(btn =>
        {
            if (btn.Texture?.Buffer is null) return false;

            ReadOnlySpan<char> bufferSpan = btn.Texture.Buffer.AsSpan().Trim();
            return bufferSpan.SequenceEqual(trimmedText.AsSpan());
        });

        return button is not null && RemoveButton(button);
    }

    public bool TryNextIndex()
    {
        if (Buttons.Count < 2)
        {
            return false;
        }
        Buttons[_index].FocusOut();
        if (_index + 1 < Buttons.Count)
            _index += 1;
        else
            _index = 0;
        Buttons[_index].FocusIn();
        return true;
    }

    public bool TryPrevIndex()
    {
        if (Buttons.Count < 2)
        {
            return false;
        }
        Buttons[_index].FocusOut();
        if (_index - 1 >= 0)
            _index -= 1;
        else
            _index = Buttons.Count - 1;
        Buttons[_index].FocusIn();
        return true;
    }

    public bool TryIndexTo(int index)
    {
        if (index == _index || index < 0 || index >= Buttons.Count) return false;

        Buttons[_index].FocusOut();
        _index = index;
        Buttons[_index].FocusIn();
        return true;
    }

    public void ClickFocusedButton()
    {
        Buttons[Index].Click();
    }
}