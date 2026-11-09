namespace Game.Tools;

public class IndexatorBag<T>
{
    public T Value { get; set; }
    public event Action<T>? OnIndexIn;
    public event Action<T>? OnIndexOut;

    public IndexatorBag(T value, Action<T>? onIndexIn = null, Action<T>? onIndexOut = null)
    {
        Value = value;
        if (onIndexIn is not null) OnIndexIn += onIndexIn;
        if (onIndexOut is not null) OnIndexOut += onIndexOut;
    }

    public void ApplyOnIndexIn() => OnIndexIn?.Invoke(Value);
    public void ApplyOnIndexOut() => OnIndexOut?.Invoke(Value);
}