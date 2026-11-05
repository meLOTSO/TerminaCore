using Game.Display.Abstract;
using TerminaCore.Graphics;

public interface IOption
{
    public Texture Texture { get; set; }
    public IReadOnlyList<ISelect> Selects { get; }

    public event Action<ISelect>? OnSelectAdd;
    public event Action<ISelect>? OnSelectRemove;

    public void AddSelect(ISelect select);
    public void RemoveSelect(ISelect select);
}