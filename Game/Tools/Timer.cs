using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;

namespace Game.Tools;

public class Timer : IUpdateable
{
    private float _time;
    private float _timeout;
    private bool _stopped = false;

    public Action<Tools.Timer>? OnTimeout;
    public float Timeout
    {
        get => _timeout;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Время для таймера не может быть меньше нуля.");
            _timeout = value;
        }
    }

    public Timer(float timeout)
    {
        if (timeout <= 0)
            throw new ArgumentException("Время для таймера не может быть меньше нуля.");

        _time = timeout;
        _timeout = timeout;
        Timeout = timeout;
    }

    public void Start() => _stopped = false;
    public void Stop() => _stopped = true;
    public void Reset()
    {
        _time = _timeout;
    }

    public void Update(UpdateContext updateContext)
    {
        if (_stopped) return;

        _time -= updateContext.DeltaTime;
        if (_time <= 0)
        {
            OnTimeout?.Invoke(this);
            _time = _timeout;
        }
    }
}