namespace TerminaCore.Graphics.DOM.Abstract;

public abstract class ModelNodeBase<T> where T : ModelNodeBase<T>
{
    private readonly List<T> _childs = [];

    public int Index => Parent is not null ? Parent._childs.IndexOf((T)this) : -1;
    public Texture Texture { get; }
    public T? Parent { get; private set; }
    public IReadOnlyList<T> Childs => _childs;
    public IReadOnlyList<T> Neighbors => Parent?.Childs ?? [];
    public T? PrevNode => (Index > 0 && Parent is not null) ? Parent.Childs[Index - 1] : null;
    public T? NextNode => (Index >= 0 && Parent is not null && Index + 1 < Parent.Childs.Count) ? Parent.Childs[Index + 1] : null;
    public string? Id { get; set; }
    public string? ClassName { get; set; }

    public ModelNodeBase(Texture texture,
        T? parent = null,
        string? id = null,
        string? className = null)
    {
        Texture = texture;
        Parent = parent;
        Id = id;
        ClassName = className;
    }

    public virtual void AddChild(T node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node == this) throw new ArgumentException("Объект не может быть дочерним для самого себя.");
        if (node.Parent == this && _childs.Contains(node)) return;

        node.Parent?.RemoveChild(node);

        node.Parent = (T)this;
        _childs.Add(node);
    }
    public virtual bool RemoveChild(T node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (_childs.Remove(node))
        {
            node.Parent = null;
            return true;
        }

        return false;
    }
}