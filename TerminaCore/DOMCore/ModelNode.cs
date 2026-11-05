using TerminaCore.Graphics;
using TerminaCore.DOMCore.Abstract;
using TerminaCore.DOMCore.Enums;
using TerminaCore.DOMCore.Style;

namespace TerminaCore.DOMCore;

public class ModelNode : ModelNodeBase<ModelNode>
{
    public event Action<ModelNode>? OnChildAppend;
    public event Action<ModelNode>? OnChildRemove;
    public ModelNode? NearestRelativeParent => ModelNode.FirstParent((node) => node.Style.Position == Enums.Position.Relative, this);

    public StyleConfiguration Style { get; }

    public ModelNode(Texture texture,
        ModelNode? parentNode = null,
        string? id = null,
        string? className = null,
        Action<StyleConfiguration>? styleConfiguration = null) : base(texture, parentNode, id, className)
    {
        this.Style = new StyleConfiguration(this);
        styleConfiguration?.Invoke(this.Style);
    }

    public override void AddChild(ModelNode node)
    {
        OnChildAppend?.Invoke(node);
        base.AddChild(node);
    }

    public override bool RemoveChild(ModelNode node)
    {
        bool removed = base.RemoveChild(node);
        if (removed)
        {
            OnChildRemove?.Invoke(node);
        }
        return removed;
    }

    #region LINQ-выражения

    public List<ModelNode> ChildsWhere(Predicate<ModelNode> predicate) => ChildsWhere(predicate, this);
    public List<ModelNode> ParentsWhere(Predicate<ModelNode> predicate) => ParentsWhere(predicate, this);

    public ModelNode? FirstChild(Predicate<ModelNode> predicate) => FirstChild(predicate, this);
    public bool TryFirstChild(Predicate<ModelNode> predicate, out ModelNode? outNode) => TryFirstChild(predicate, this, out outNode);

    public ModelNode? FirstParent(Predicate<ModelNode> predicate) => FirstParent(predicate, this);
    public bool TryFirstParent(Predicate<ModelNode> predicate, out ModelNode? outNode) => TryFirstParent(predicate, this, out outNode);

    public List<T> SelectChilds<T>(Func<ModelNode, T> func) => SelectChilds<T>(func, this);
    public List<T> SelectParents<T>(Func<ModelNode, T> func) => SelectParents<T>(func, this);

    public static List<T> SelectParents<T>(Func<ModelNode, T> func, ModelNode node)
    {
        List<T> result = [];
        void selectParents(Func<ModelNode, T> f, ModelNode? n)
        {
            if (n is null) return;
            result.Add(f.Invoke(n));
            selectParents(f, n.Parent);
        }
        selectParents(func, node);
        return result;
    }

    public static List<T> SelectChilds<T>(Func<ModelNode, T> func, ModelNode node)
    {
        List<T> result = [];
        Queue<ModelNode> queue = new();
        queue.Enqueue(node);
        while (queue.TryDequeue(out ModelNode? n))
        {
            result.Add(func.Invoke(n));
            foreach (var c in n.Childs)
                queue.Enqueue(c);
        }
        return result;
    }

    public static bool TryFirstParent(Predicate<ModelNode> predicate, ModelNode? node, out ModelNode? outNode)
    {
        if (node is null)
        {
            outNode = null;
            return false;
        }
        if (predicate.Invoke(node))
        {
            outNode = node;
            return true;
        }
        return TryFirstParent(predicate, node.Parent, out outNode);
    }

    public static ModelNode? FirstParent(Predicate<ModelNode> predicate, ModelNode? node)
    {
        if (node is null) return null;
        if (predicate.Invoke(node)) return node;
        return FirstParent(predicate, node.Parent);
    }

    public static bool TryFirstChild(Predicate<ModelNode> predicate, ModelNode node, out ModelNode? outModel)
    {
        Queue<ModelNode> queue = new();
        queue.Enqueue(node);
        while (queue.TryDequeue(out ModelNode? n))
        {
            if (predicate.Invoke(n))
            {
                outModel = n;
                return true;
            }
            foreach (var c in n.Childs)
                queue.Enqueue(c);
        }
        outModel = null;
        return false;
    }

    public static ModelNode? FirstChild(Predicate<ModelNode> predicate, ModelNode node)
    {
        Queue<ModelNode> queue = new();
        queue.Enqueue(node);
        while (queue.TryDequeue(out ModelNode? n))
        {
            if (predicate.Invoke(n)) return n;
            foreach (var c in n.Childs)
                queue.Enqueue(c);
        }
        return null;
    }

    public static List<ModelNode> ParentsWhere(Predicate<ModelNode> predicate, ModelNode node)
    {
        List<ModelNode> result = [];
        var current = node.Parent;
        while (current is not null)
        {
            if (predicate.Invoke(current)) result.Add(current);
            current = current.Parent;
        }
        return result;
    }

    public static List<ModelNode> ChildsWhere(Predicate<ModelNode> predicate, ModelNode node)
    {
        List<ModelNode> result = [];
        Queue<ModelNode> queue = new();
        queue.Enqueue(node);
        while (queue.TryDequeue(out ModelNode? n))
        {
            if (predicate.Invoke(n)) result.Add(n);
            foreach (var c in n.Childs)
                queue.Enqueue(c);
        }
        return result;
    }

    public void ForEachBFS(Action<ModelNode> action)
    {
        Queue<ModelNode> queue = new();
        queue.Enqueue(this);

        while (queue.TryDequeue(out ModelNode? node))
        {
            action?.Invoke(node);
            foreach (var child in node.Childs)
            {
                queue.Enqueue(child);
            }
        }
    }

    #endregion

    public void ApplyStyle(Action<StyleConfiguration> configuration)
    {
        configuration.Invoke(Style);
    }

    public void UpdateStyle() => Style.Update();
    public void UpdateStyleBFS() => Style.UpdateBFS();
}