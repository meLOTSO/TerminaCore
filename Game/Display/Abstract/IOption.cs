using Game.Display.Abstract;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.Graphics;

public interface IOption
{
    public IReadOnlyList<ISelect> Selects { get; }

    public event Action<ISelect>? OnSelectAdd;
    public event Action<ISelect>? OnSelectRemove;

    public void AddSelect(ISelect select);
    public void RemoveSelect(ISelect select);
}