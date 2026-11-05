using Game.Display.Abstract;

namespace Game.Display;

public class TabCollection
{
    private int _index;
    private int _defaultIndex;
    protected List<ITabable> Tabs = [];
    public int Count => Tabs.Count;
    public int Index { get => _index; }

    public TabCollection(int defaultIndex = -1)
    {
        _defaultIndex = defaultIndex;
        _index = defaultIndex;
    }

    public event Action? OnTabIn;
    public event Action? OnTabOut;

    public void AddTabableEntity(ITabable tabable)
    {
        tabable.OnTabIn += OnTabIn;
        tabable.OnTabOut += OnTabOut;
    }

    public void RemoveTabableEntity(ITabable tabable)
    {
        bool removed = Tabs.Remove(tabable);
        if (removed)
        {
            tabable.OnTabIn -= OnTabIn;
            tabable.OnTabOut -= OnTabOut;
        }
    }

    public void TabNext()
    {
        (int prevIndex, int currIndex) = TabIndex();
        var prev = Tabs[prevIndex];
        var curr = Tabs[currIndex];
        prev.TabOut();
        curr.TabThis();
    }
    private (int PrevIndex, int CurrentIndex) TabIndex()
    {
        int prevIndex = _index;

        if (_index + 1 < Tabs.Count)
            _index += 1;
        else
            _index = _defaultIndex;
        return (prevIndex, _index);
    }

    public void Clear()
    {
        foreach (var t in Tabs)
        {
            t.OnTabIn -= OnTabIn;
            t.OnTabOut -= OnTabOut;
        }
        Tabs.Clear();
    }
}