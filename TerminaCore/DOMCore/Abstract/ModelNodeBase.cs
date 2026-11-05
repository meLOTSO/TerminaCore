using System;
using System.Collections.Generic;
using TerminaCore.Graphics;

namespace TerminaCore.DOMCore.Abstract;

public abstract class ModelNodeBase<T> where T : ModelNodeBase<T>
{
    private readonly List<T> _childs = [];
    private string? _id;

    public int Index => Parent is not null ? Parent._childs.IndexOf((T)this) : -1;
    public Texture Texture { get; }
    public T? Parent { get; private set; }
    public IReadOnlyList<T> Childs => _childs;
    public IReadOnlyList<T> Neighbors => Parent?.Childs ?? [];
    public T? PrevNode => (Index > 0 && Parent is not null) ? Parent.Childs[Index - 1] : null;
    public T? NextNode => (Index >= 0 && Parent is not null && Index + 1 < Parent.Childs.Count) ? Parent.Childs[Index + 1] : null;
    public string? ClassName { get; set; }

    public string? Id
    {
        get => _id;
        set
        {
            if (value is null)
            {
                _id = null;
                return;
            }

            T root = (T)this;
            while (root.Parent is not null)
            {
                root = root.Parent;
            }

            Queue<T> queue = new();
            queue.Enqueue(root);
            while (queue.TryDequeue(out T? item))
            {
                if (!ReferenceEquals(item, this) && item.Id == value)
                    throw new InvalidOperationException($"Идентификатор '{value}' уже используется в DOM-дереве.");

                foreach (var child in item.Childs)
                {
                    queue.Enqueue(child);
                }
            }
            _id = value;
        }
    }

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

        if (ReferenceEquals(node, this))
            throw new ArgumentException("Объект не может быть дочерним для самого себя.");

        if (node.Parent == this && _childs.Contains(node))
            return;

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