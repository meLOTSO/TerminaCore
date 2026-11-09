namespace Game.Tools.Abstract;

public class Indexator<T>
{
    private int _currentIndex = 0;
    private readonly IReadOnlyList<T> _list;

    public int CurrentIndex => _currentIndex;
    public event Action<T>? OnIndexOut;
    public event Action<T>? OnIndexIn;

    public Indexator(IReadOnlyList<T> list)
    {
        _list = list;
    }

    public virtual bool TryNextIndex()
    {
        if (_list.Count < 2) return false;

        OnIndexOut?.Invoke(_list[CurrentIndex]);
        if (_currentIndex + 1 == _list.Count)
            _currentIndex = 0;
        else _currentIndex++;
        OnIndexIn?.Invoke(_list[CurrentIndex]);
        return true;
    }

    public virtual bool TryPrevIndex()
    {
        if (_list.Count < 2) return false;

        OnIndexOut?.Invoke(_list[CurrentIndex]);
        if (_currentIndex == 0)
            _currentIndex = _list.Count - 1;
        else _currentIndex--;
        OnIndexIn?.Invoke(_list[CurrentIndex]);
        return true;
    }

    public T? GetCurrentValueOrDefault()
    {
        if (_list.Count == 0) return default;
        return _list[CurrentIndex];
    }

    public bool TryGetCurrentValue(out T? value)
    {
        if (_list.Count == 0)
        {
            value = default;
            return false;
        }
        value = _list[CurrentIndex];
        return true;
    }

    public void Reset()
    {
        _currentIndex = 0;
    }
}