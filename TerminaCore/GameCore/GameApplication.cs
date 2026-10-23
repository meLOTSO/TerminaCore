using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TerminaCore.EntityCore;
using TerminaCore.EntityCore.Abstract;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.ServiceCore;

namespace TerminaCore.GameCore;

public class GameApplication : GameBase
{
    private bool _run = false;

    public int TargetDeltaTimeMS { get; set; } = 1000 / 60; // заданная дельта
    public float DeltaTimeSeconds { get; private set; } // фактическая дельта

    public ServiceCore.Abstract.IServiceProvider Services { get; init; } = new ServiceProvider();
    public IEntityProvider Entities { get; init; } = new EntityProvider();
    public GameContext GameContext { get; private set; }
    public UpdateContext UpdateContext { get; private set; }
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
        UpdateCollection = new List<Delegates.UpdateDelegate>(gameBase.UpdateCollection);
        OnKeyPressCollection = new List<Delegates.OnKeyPressDelegate>(gameBase.OnKeyPressCollection);
        DrawCollection = new List<Delegates.DrawDelegate>(gameBase.DrawCollection);
    }
    public GameApplication()
    {
        GameContext = new GameContext(this);
        UpdateContext = new UpdateContext(this);
        ControlContext = new ControlContext(this);
        DrawContext = new DrawContext(this);
    }

    private void Update()
    {
        BeforeUpdate?.Invoke();
        var updates = UpdateCollection;
        for (int i = 0; i < updates.Count; i++)
        {
            updates[i].Invoke(UpdateContext);
        }
        AfterUpdate?.Invoke();
    }

    private void Draw()
    {
        BeforeDraw?.Invoke();
        var draws = DrawCollection;
        for (int i = 0; i < draws.Count; i++)
        {
            draws[i].Invoke(DrawContext);
        }
        AfterDraw?.Invoke();
    }

    private void OnKeyPress()
    {
        int processedKeys = 0;
        while (Console.KeyAvailable && processedKeys < 10)
        {
            BeforeOnKeyPress?.Invoke();
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            ControlContext.Pressed = true;
            ControlContext.Key = key.Key;
            ControlContext.Modifiers = key.Modifiers;
            foreach (var c in OnKeyPressCollection)
            {
                c.Invoke(ControlContext);
            }
            AfterOnKeyPress?.Invoke();
            processedKeys++;
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

        try
        {
            while (_run)
            {
                long frameStartTimestamp = Stopwatch.GetTimestamp();
                float rawDelta = (float)Stopwatch.GetElapsedTime(prevTimestamp).TotalSeconds;
                DeltaTimeSeconds = Math.Clamp(rawDelta, 0.0001f, 0.1f);
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
        }
        finally
        {
            Console.CursorVisible = true;
            Console.ResetColor();
        }
    }

    public void Stop()
    {
        BeforeStop?.Invoke();
        _run = false;
        AfterStop?.Invoke();
    }
}