using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Text;
using Game.Display.Abstract;
using Game.Display.Enums;
using TerminaCore.DOMCore;
using TerminaCore.GameCore.Contexts;
using TerminaCore.Graphics;

namespace Game.Display;

public class InputText : IInput
{
    private int _currIndex = 0;
    private Tools.Timer _timer;

    public FrozenSet<char> AviableSymbols = new HashSet<char>("1234567890 qwertyuiopasdfghjklzxcvbnmйцукенгшщзхъфывапролджэячсмитьбю").ToFrozenSet();
    public Texture InputTexture { get; }
    public Texture LabelTexture { get; }
    public Texture CurrentCharTexture { get; } = new Texture(" ") { IgnoreSet = new() };
    public ConsoleColor? ShowCharColor { get; set; } = ConsoleColor.Gray;
    public ConsoleColor? ShowCharBgColor { get; set; } = ConsoleColor.Black;
    public ConsoleColor? DisableCharColor { get; set; }
    public ConsoleColor? DisableCharBgColor { get; set; }
    public float ShowCharTime { get; set; } = 0.51f;
    public float DisableCharTime { get; set; } = 0.51f;
    public StringBuilder Input { get; }
    public string Label { get; }
    public int MaxLength { get; }

    public InputText(string labelText, int maxLength)
    {
        MaxLength = maxLength;
        InputTexture = new Texture(maxLength, 1);
        LabelTexture = new Texture(labelText);
        Label = labelText;
        Input = new StringBuilder();

        _timer = new Tools.Timer(DisableCharTime);
        ResetCurrCharBlink();
    }

    public virtual void AddSymbol(char symbol)
    {
        if (!AviableSymbols.Contains(char.ToLower(symbol)) || _currIndex >= MaxLength || Input.Length >= MaxLength) return;
        _currIndex++;

        Input.Insert(_currIndex - 1, symbol);
        // InputTexture.Buffer[_currIndex - 1] = symbol;
        UpdateInputText();
        UpdateCurrentCharIndexTexture();
        ResetCurrCharBlink();
    }

    public virtual char Backspace()
    {
        if (_currIndex <= 0) return ' ';
        _currIndex--;
        char c = Input[_currIndex];
        Input.Remove(_currIndex, 1);
        // InputTexture.SetSymbol(0, _currIndex, ' ');
        UpdateInputText();
        UpdateCurrentCharIndexTexture();
        ResetCurrCharBlink();
        return c;
    }

    public bool StepLeft()
    {
        if (_currIndex <= 0) return false;
        _currIndex--;
        UpdateCurrentCharIndexTexture();
        ResetCurrCharBlink();
        return true;
    }

    public bool StepRight()
    {
        if (_currIndex >= MaxLength || _currIndex >= Input.Length) return false;
        _currIndex++;
        UpdateCurrentCharIndexTexture();
        ResetCurrCharBlink();
        return true;
    }

    public void Update(float deltaTime)
    {
        _timer.Update(deltaTime);
    }

    private void UpdateInputText()
    {
        for (int i = 0; i < MaxLength; i++)
        {
            char c = i < Input.Length ? Input[i] : ' ';
            InputTexture.SetSymbol(0, i, c);
        }
    }

    private void UpdateCurrentCharIndexTexture()
    {
        CurrentCharTexture.Y = this.InputTexture!.Y;
        CurrentCharTexture.X = this.InputTexture!.X + _currIndex;
        CurrentCharTexture.SetSymbol(0, 0, _currIndex < Input.Length ? Input[_currIndex] : ' ');
    }

    // public void UpdateLabelPosition(PositionOverType positionOverType)
    // {
    //     switch (positionOverType)
    //     {

    //         default:
    //     }
    // }

    private void DisableCurrChar(Tools.Timer timer)
    {
        CurrentCharTexture.BackgroundColor = DisableCharBgColor;
        CurrentCharTexture.ForegroundColor = DisableCharColor;
        timer.Timeout = ShowCharTime;
        timer.OnTimeout = ShowCurrChar;
    }

    private void ShowCurrChar(Tools.Timer timer)
    {
        CurrentCharTexture.BackgroundColor = ShowCharBgColor;
        CurrentCharTexture.ForegroundColor = ShowCharColor;
        timer.Timeout = DisableCharTime;
        timer.OnTimeout = DisableCurrChar;
    }

    private void ResetCurrCharBlink()
    {
        CurrentCharTexture.BackgroundColor = ShowCharBgColor;
        CurrentCharTexture.ForegroundColor = ShowCharColor;
        _timer.Reset(); // или перезапуск таймера
        _timer.Timeout = DisableCharTime;
        _timer.OnTimeout = DisableCurrChar;
    }

    public void TexturesForDraw(Action<Texture> drawAction, bool withLabel = true)
    {
        if (withLabel)
            drawAction.Invoke(LabelTexture);

        drawAction.Invoke(InputTexture);
        drawAction.Invoke(CurrentCharTexture);
    }

    public void ControlHandler(ControlContext context)
    {
        if (context.Key == ConsoleKey.Backspace)
            this.Backspace();
        else if (context.Key == ConsoleKey.LeftArrow)
            this.StepLeft();
        else if (context.Key == ConsoleKey.RightArrow)
            this.StepRight();
        else
        {
            this.AddSymbol(context.KeyChar);
        }
    }
}