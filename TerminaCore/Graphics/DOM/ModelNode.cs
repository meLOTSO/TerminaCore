using System;
using System.Collections.Generic;
using System.Linq;
using TerminaCore.Graphics;
using TerminaCore.Graphics.DOM.Abstract;
using TerminaCore.Graphics.DOM.Enums;
using TerminaCore.Graphics.DOM.Style;

namespace TerminaCore.Graphics.DOM;

public class ModelNode : ModelNodeBase<ModelNode>
{
    #region Базовая конструкция

    public event Action<ModelNode>? OnChildAppend;
    public event Action<ModelNode>? OnChildRemove;

    public ModelNode(Texture texture,
        ModelNode? parentNode = null,
        string? id = null,
        string? className = null,
        StyleConfiguration? styleConfiguration = null) : base(texture, parentNode, id, className)
    {
        if (styleConfiguration is not null) Style = styleConfiguration;
    }

    public override void AddChild(ModelNode node)
    {
        OnChildAppend?.Invoke(node);
        base.AddChild(node);
    }

    public override bool RemoveChild(ModelNode node)
    {
        OnChildRemove?.Invoke(node);
        return base.RemoveChild(node);
    }

    #endregion

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

    #endregion

    #region Style конструкция

    private ModelNodeStyleOptions _editor = new();

    public StyleConfiguration Style { get; set; } = new();
    public ModelNode? NearestRelativeParent => ModelNode.FirstParent((node) => node.Style.Position == DOM.Enums.Position.Relative, this);
    public int X { get; set; }
    public int Y { get; set; }

    #endregion

    #region Применение всех стилей

    public void Update()
    {
        PositionUpdate(Style.Position);
        DisplayUpdate(Style.Display);

        if (Parent is not null && Parent.Style.Display == Display.Flex)
        {
            FlexStyleUpdate();
        }

        // Применение настроек со всеми смещениями
        Texture.X = X + _editor.ByDisplayStrideX + _editor.ByPositionStrideX + _editor.ByFlexX;
        Texture.Y = Y + _editor.ByDisplayStrideY + _editor.ByPositionStrideY + _editor.ByFlexY;
    }

    #endregion

    #region Применение стилей Display { Block, Inline, Flex, Grid }

    public void DisplayUpdate(Display display)
    {
        switch (display)
        {
            case Display.Block: DisplayBlockUpdate(); break;
            case Display.Inline: DisplayInlineUpdate(); break;
            case Display.Flex: DisplayFlexUpdate(); break;
            case Display.Grid: DisplayGridUpdate(); break;
        }
    }

    private void DisplayBlockUpdate()
    {
        if (PrevNode is null) return;

        var prev = PrevNode;
        int strideY = prev.Texture.Y + prev.Texture.Height + 1;

        if (prev.Style.Display == Display.Inline)
        {
            strideY = -1;
            for (int i = Index - 1; i >= 0; i--)
            {
                if (Neighbors[i].Style.Display == Display.Inline)
                    strideY = Math.Max(strideY, Neighbors[i].Texture.Y + Neighbors[i].Texture.Height + 1);
                else break;
            }
        }
        _editor.ByDisplayStrideX = 0;
        _editor.ByDisplayStrideY = strideY;
    }

    private void DisplayInlineUpdate()
    {
        if (PrevNode is null) return;

        var prev = PrevNode;
        if (prev.Style.Display == Display.Inline)
        {
            _editor.ByDisplayStrideX = prev.Texture.X + prev.Texture.Width + 1;
            _editor.ByDisplayStrideY = prev.Texture.Y;
        }
        else
        {
            _editor.ByDisplayStrideX = 0;
            _editor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
        }
    }

    private void DisplayFlexUpdate()
    {
        if (PrevNode is null) return;

        var prev = PrevNode;
        _editor.ByDisplayStrideX = 0;
        _editor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
    }

    private void DisplayGridUpdate()
    {
        if (PrevNode is null) return;

        var prev = PrevNode;
        _editor.ByDisplayStrideX = 0;
        _editor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
    }

    #endregion

    #region Применение стилей Position { Absolute, Relative, Fixed }

    public void PositionUpdate(DOM.Enums.Position position)
    {
        switch (Style.Position)
        {
            case DOM.Enums.Position.Absolute: PositionAbsoluteUpdate(); break;
            case DOM.Enums.Position.Relative: PositionRelativeUpdate(); break;
            case DOM.Enums.Position.Fixed: PositionFixedUpdate(); break;
        }
    }

    private void PositionAbsoluteUpdate()
    {
        _editor.ByPositionStrideX = NearestRelativeParent?.Texture.X ?? 0;
        _editor.ByPositionStrideY = NearestRelativeParent?.Texture.Y ?? 0;
    }

    private void PositionRelativeUpdate()
    {
        _editor.ByPositionStrideX = NearestRelativeParent?.Texture.X ?? 0;
        _editor.ByPositionStrideY = NearestRelativeParent?.Texture.Y ?? 0;
    }

    private void PositionFixedUpdate()
    {
        _editor.ByPositionStrideX = 0;
        _editor.ByPositionStrideY = 0;
    }

    #endregion

    #region Применение стилей Flex

    public void FlexStyleUpdate()
    {
        if (Parent is null || Parent.Style.Display != Display.Flex) return;
        var parent = Parent;

        int alpha = 0, beta = 0;
        switch (Parent.Style.FlexPath)
        {
            case FlexPath.FlexRow: alpha = 1; break;
            case FlexPath.FlexColumn: beta = 1; break;
        }

        int width = parent.Texture.Width;
        int height = parent.Texture.Height;
        int startX = parent.Texture.X;
        int startY = parent.Texture.Y;
        // Func<int, int> strideX = (i) => i;
        // Func<int, int> strideY = (i) => i;
        int strideX = 0;
        int strideY = 0;

        var items = Neighbors;
        int count = parent.Childs.Count;
        int currIndex = Index;

        switch (parent.Style.FlexPosition)
        {
            case FlexPosition.Start:
                for (int k = 0; k < currIndex; k++)
                {
                    strideX += items[k].Texture.Width + parent.Style.GapX;
                }
                strideX += 1;
                // strideX = (i) =>
                // {
                //     int r = 0;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Width + parent.Style.GapX;
                //     }
                //     return r + 1;
                // };
                for (int k = 0; k < currIndex; k++)
                {
                    strideY += items[k].Texture.Height + parent.Style.GapY;
                }
                strideY += 1;
                // strideY = (i) =>
                // {
                //     int r = 0;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Height + parent.Style.GapY;
                //     }
                //     return r + 1;
                // };
                break;

            case FlexPosition.End:
                if (currIndex == count - 1) strideX = width - items[currIndex].Texture.Width;
                else
                {
                    strideX = width;
                    for (int k = currIndex; k < count; k++)
                    {
                        strideX -= items[k].Texture.Width;
                        strideX -= parent.Style.GapX;
                    }
                    strideX += 1;
                }
                // strideX = (i) =>
                // {
                //     if (i == count - 1) return width - items[i].Texture.Width;
                //     int r = width;
                //     for (int k = i; k < count; k++)
                //     {
                //         r -= items[k].Texture.Width;
                //         r -= parent.Style.GapX;
                //     }
                //     return r + 1;
                // };
                if (currIndex == count - 1) strideX = height - items[currIndex].Texture.Height;
                else
                {
                    strideY = height;
                    for (int k = currIndex; k < count; k++)
                    {
                        strideY -= items[k].Texture.Height;
                        strideY -= parent.Style.GapY;
                    }
                    strideY += 1;
                }
                // strideY = (i) =>
                // {
                //     if (i == count - 1) return height - items[i].Texture.Height;
                //     int r = height;
                //     for (int k = i; k < count; k++)
                //     {
                //         r -= items[k].Texture.Height;
                //         r -= parent.Style.GapY;
                //     }
                //     return r + 1;
                // };
                break;

            case FlexPosition.Center:
                int realWidth = parent.Childs.Sum(c => c.Texture.Width);
                int realHeight = parent.Childs.Sum(c => c.Texture.Height);
                int sX = (width - realWidth) / 2;
                int sY = (height - realHeight) / 2;

                strideX = sX;
                for (int k = 0; k < currIndex; k++)
                {
                    strideX += items[k].Texture.Width + parent.Style.GapX;
                }
                strideX += 1;
                // strideX = (i) =>
                // {
                //     int r = sX;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Width + parent.Style.GapX;
                //     }
                //     return r + 1;
                // };
                strideY = sY;
                for (int k = 0; k < currIndex; k++)
                {
                    strideY += items[k].Texture.Height + parent.Style.GapY;
                }
                strideY += 1;
                // strideY = (i) =>
                // {
                //     int r = sY;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Height + parent.Style.GapY;
                //     }
                //     return r + 1;
                // };
                break;

            case FlexPosition.SpaceBetween:
                realWidth = parent.Childs.Sum(c => c.Texture.Width);
                realHeight = parent.Childs.Sum(c => c.Texture.Height);
                int sbX = count > 1 ? (width - realWidth) / (count - 1) : 0;
                int sbY = count > 1 ? (height - realHeight) / (count - 1) : 0;

                if (currIndex == count - 1) strideX = width - items[currIndex].Texture.Width;
                else
                {
                    for (int k = 0; k < currIndex; k++)
                    {
                        strideX += items[k].Texture.Width + sbX;
                    }
                    strideX += 1;
                }
                // strideX = (i) =>
                // {
                //     if (i == count - 1) return width - items[i].Texture.Width;
                //     int r = 0;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Width + sbX;
                //     }
                //     return r + 1;
                // };
                if (currIndex == count - 1) strideY = height - items[currIndex].Texture.Height;
                else
                {
                    for (int k = 0; k < currIndex; k++)
                    {
                        strideY += items[k].Texture.Height + sbY;
                    }
                    strideY += 1;
                }
                // strideY = (i) =>
                // {
                //     if (i == count - 1) return height - items[i].Texture.Height;
                //     int r = 0;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Height + sbY;
                //     }
                //     return r + 1;
                // };
                break;

            case FlexPosition.SpaceAround:
                realWidth = parent.Childs.Sum(c => c.Texture.Width);
                realHeight = parent.Childs.Sum(c => c.Texture.Height);
                sbX = count > 0 ? (width - realWidth) / count : 0;
                sbY = count > 0 ? (height - realHeight) / count : 0;

                strideX = sbX;
                for (int k = 0; k < currIndex; k++)
                {
                    strideX += items[k].Texture.Width + sbX;
                }
                strideX += 1;
                // strideX = (i) =>
                // {
                //     int r = sbX;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Width + sbX;
                //     }
                //     return r + 1;
                // };
                strideY = sbY;
                for (int k = 0; k < currIndex; k++)
                {
                    strideY += items[k].Texture.Height + sbY;
                }
                strideY += 1;
                // strideY = (i) =>
                // {
                //     int r = sbY;
                //     for (int k = 0; k < i; k++)
                //     {
                //         r += items[k].Texture.Height + sbY;
                //     }
                //     return r + 1;
                // };
                break;
        }

        _editor.ByFlexX = startX + alpha * strideX;
        // _editor.ByFlexX = startX + alpha * strideX.Invoke(Index);
        _editor.ByFlexY = startY + beta * strideY;
        // _editor.ByFlexY = startY + beta * strideY.Invoke(Index);
    }

    #endregion

    #region Применение стилей Grid

    #endregion
}