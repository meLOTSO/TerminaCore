using System.Runtime.InteropServices;
using Game.Display.Abstract;
using TerminaCore.GameCore.Abstract;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

namespace Game.Environments.DisplayInteractive;

public class CursorEnvironment : GameEnvironmentBase, IUpdateable, IDrawable, IControl, IExitEnvironment
{
    private char _cursorSymbol = '↑';
    private ConsoleColor? _color = ConsoleColor.White;
    private ConsoleColor? _backgroundColor = ConsoleColor.DarkGray;
    private readonly Texture _cursorTexture;
    private int _windowWidth;
    private int _windowHeight;
    private float _cursorBlickShowTimeout;
    private bool _showCursor = true;
    private IMouseMoveReactable? _mouseMovedOn = null;
    private Func<Texture?>? _getFirstClickableTexture = null;
    private Func<Texture?>? _getMouseMoveTexture = null;

    public char CursorSymbol
    {
        get => _cursorSymbol;
        set
        {
            _cursorSymbol = value;
            _cursorTexture.SetSymbol(0, 0, value);
        }
    }
    public ConsoleColor? Color
    {
        get => _color;
        set
        {
            _color = value;
            _cursorTexture.ForegroundColor = value;
        }
    }
    public ConsoleColor? BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            _cursorTexture.BackgroundColor = value;
        }
    }
    public float CursorBlickTime { get; set; } = 0.53f;
    public float CursorShowTime { get; set; } = 0.53f;

    public CursorEnvironment()
    {
        _cursorTexture = new(_cursorSymbol.ToString(), _color, _backgroundColor)
        {
            ZIndex = int.MaxValue
        };
        _cursorBlickShowTimeout = CursorShowTime;
    }

    public void Draw(DrawContext context)
    {
        if (_showCursor)
            context.Draw(_cursorTexture);
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.UpArrow || controlContext.Key == ConsoleKey.W)
        {
            if (_cursorTexture.Y - 1 >= 0)
                _cursorTexture.Y -= 1;
            else
                _cursorTexture.Y = _windowHeight - 1;
            ShowCursor();
        }
        else if (controlContext.Key == ConsoleKey.DownArrow || controlContext.Key == ConsoleKey.S)
        {
            if (_cursorTexture.Y + 1 < _windowHeight)
                _cursorTexture.Y += 1;
            else
                _cursorTexture.Y = 0;
            ShowCursor();
        }
        else if (controlContext.Key == ConsoleKey.RightArrow || controlContext.Key == ConsoleKey.D)
        {
            if (_cursorTexture.X + 1 < _windowWidth)
                _cursorTexture.X += 1;
            else
                _cursorTexture.X = 0;
            ShowCursor();
        }
        else if (controlContext.Key == ConsoleKey.LeftArrow || controlContext.Key == ConsoleKey.A)
        {
            if (_cursorTexture.X - 1 >= 0)
                _cursorTexture.X -= 1;
            else
                _cursorTexture.X = _windowWidth - 1;
            ShowCursor();
        }
        else if (controlContext.Key == ConsoleKey.Enter)
        {
            var t = _getFirstClickableTexture?.Invoke();
            if (t?.Tag is IClickable btn)
                btn.Click();
        }
    }

    private void ShowCursor()
    {
        _showCursor = true;
        _cursorBlickShowTimeout = CursorShowTime;
    }
    private void HideCursor()
    {
        _showCursor = false;
        _cursorBlickShowTimeout = CursorBlickTime;
    }

    public void Update(UpdateContext context)
    {
        _cursorBlickShowTimeout -= context.DeltaTime;

        if (_cursorBlickShowTimeout <= 0)
        {
            if (_showCursor) HideCursor();
            else ShowCursor();
        }
        MouseMoveUpdate();
    }

    private void MouseMoveUpdate()
    {
        var t = _getMouseMoveTexture?.Invoke();
        if (t?.Tag is IMouseMoveReactable mouseMove)
        {
            mouseMove.MouseMove();
            if (t != _mouseMovedOn)
            {
                _mouseMovedOn = mouseMove;
                mouseMove.FocusIn();
            }
        }
        else if (_mouseMovedOn is not null)
        {
            _mouseMovedOn.FocusOut();
            _mouseMovedOn = null;
        }
    }

    public override void Initialize(GameContext context)
    {
        base.Initialize(context);

        _windowWidth = context.GetCanvasWidth();
        _windowHeight = context.GetCanvasHeight();

        var canvas = context.GetCanvasRequired();
        _backgroundColor = canvas.DefaultBackground;

        _getFirstClickableTexture = () => canvas.FirstTexture(t => t.Tag is IClickable && t.IntersectsPixelPerfect(_cursorTexture));
        _getMouseMoveTexture = () => canvas.FirstTexture(t => t.Tag is IMouseMoveReactable && t.IntersectsPixelPerfect(_cursorTexture));
    }

    public void OnExit()
    {
        _cursorTexture.X = 0;
        _cursorTexture.Y = 0;
        ShowCursor();
    }
}