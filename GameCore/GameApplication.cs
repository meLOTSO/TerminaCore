using System.Diagnostics;
using GameCore.Abstract.GameBasicFunctional;
using ServiceCore;

namespace GameCore;

public class GameApplication : GameBase
{
    private bool _run = false;

    public int TargetDeltaTimeMS { get; set; } = 1000 / 60; // заданная дельта
    public float DeltaTimeSeconds { get; private set; } // фактическая дельта

    public IServiceCollection Services { get; init; } = new ServiceCollection();
    public GameContext GameContext { get; private set; }

    public ControlContext ControlContext { get; private set; }
    public DrawContext DrawContext { get; private set; }

    public event Action? PreFrame;
    public event Action? AfterFrame;
    public event Action? BeforeDraw;
    public event Action? AfterDraw;
    public event Action? BeforeUpdate;
    public event Action? AfterUpdate;
    public event Action? BeforeOnKeyPress;
    public event Action? AfterOnKeyPress;
    public event Action? BeforeStop;
    public event Action? AfterStop;

    public GameApplication(GameBase gameBase) : this()
    {
        UpdateCollection = gameBase.UpdateCollection;
        OnKeyPressCollection = gameBase.OnKeyPressCollection;
        DrawCollection = gameBase.DrawCollection;
    }
    public GameApplication()
    {
        GameContext = new GameContext(this);
        ControlContext = new ControlContext(this);
        DrawContext = new DrawContext(this);
    }

    private void Update()
    {
        BeforeUpdate?.Invoke();
        foreach (var u in UpdateCollection)
        {
            u.Invoke(GameContext);
        }
        AfterUpdate?.Invoke();
    }

    private void Draw()
    {
        BeforeDraw?.Invoke();
        foreach (var d in DrawCollection)
        {
            d.Invoke(DrawContext);
        }
        AfterDraw?.Invoke();
    }

    private void OnKeyPress()
    {
        if (Console.KeyAvailable)
        {
            BeforeOnKeyPress?.Invoke();
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            ControlContext.Pressed = true;
            ControlContext.Key = key.Key;
            ControlContext.Modifiers = key.Modifiers;
            foreach (var c in OnKeyPressCollection)
            {
                c.Invoke(GameContext, ControlContext);
            }
            AfterOnKeyPress?.Invoke();
        }
        ControlContext.Pressed = false;
        ControlContext.Key = default;
        ControlContext.Modifiers = default;
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();
        Console.TreatControlCAsInput = true;
        _run = true;

        long prevTimestamp = Stopwatch.GetTimestamp();

        while (_run)
        {
            long frameStartTimestamp = Stopwatch.GetTimestamp();
            DeltaTimeSeconds = (float)Stopwatch.GetElapsedTime(prevTimestamp).TotalSeconds;
            prevTimestamp = frameStartTimestamp;

            PreFrame?.Invoke();
            OnKeyPress();
            Update();
            Draw();
            AfterFrame?.Invoke();

            TimeSpan elapsed = Stopwatch.GetElapsedTime(frameStartTimestamp);
            int sleepTime = TargetDeltaTimeMS - (int)elapsed.TotalMilliseconds;

            if (sleepTime > 0)
                Thread.Sleep(sleepTime);
        }

        Console.CursorVisible = true;
    }

    public void Stop()
    {
        BeforeStop?.Invoke();
        _run = false;
        AfterStop?.Invoke();
    }
}