using TerminaCore.Graphics;
using TerminaCore.DOMCore.Abstract;
using TerminaCore.DOMCore.Enums;
using TerminaCore.DOMCore.Style;

namespace TerminaCore.DOMCore;

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
    public ModelNode? NearestRelativeParent => ModelNode.FirstParent((node) => node.Style.Position == Enums.Position.Relative, this);
    public int X { get; set; }
    public int Y { get; set; }

    public void ApplyStyle(Action<StyleConfiguration> configuration)
    {
        configuration.Invoke(Style);
    }

    #endregion

    #region Применение всех стилей

    public void Update()
    {
        // 1. Сброс предыдущих расчетов смещений
        _editor = new ModelNodeStyleOptions();

        // 2. Базовое позиционирование (Relative / Absolute / Fixed)
        UpdatePosition();

        // 3. Вычисление потока компоновки в зависимости от режима родителя/элемента
        if (Parent is not null && Parent.Style.Display == Display.Flex)
        {
            UpdateFlexStyle();
        }
        else if (Parent is not null && Parent.Style.Display == Display.Grid)
        {
            UpdateGridStyle();
        }
        else
        {
            UpdateDisplay();
        }

        // 4. Финальное применение
        ApplyColorUpdate();
        ApplyXYUpdate();
    }

    #endregion

    #region Обновление стилей Display { Block, Inline, Flex, Grid }

    public void UpdateDisplay()
    {
        switch (Style.Display)
        {
            case Display.Block: UpdateDisplayBlock(); break;
            case Display.Inline: UpdateDisplayInline(); break;
            case Display.Flex: UpdateDisplayFlex(); break;
            case Display.Grid: UpdateDisplayGrid(); break;
        }
    }

    private void UpdateDisplayBlock()
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

    private void UpdateDisplayInline()
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

    private void UpdateDisplayFlex()
    {
        if (PrevNode is null) return;

        var prev = PrevNode;
        _editor.ByDisplayStrideX = 0;
        _editor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
    }

    private void UpdateDisplayGrid()
    {
        if (PrevNode is null) return;

        var prev = PrevNode;
        _editor.ByDisplayStrideX = 0;
        _editor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
    }

    #endregion

    #region Обновление стилей Position { Absolute, Relative, Fixed }

    public void UpdatePosition()
    {
        switch (Style.Position)
        {
            case Enums.Position.Absolute: UpdatePositionAbsolute(); break;
            case Enums.Position.Relative: UpdatePositionRelative(); break;
            case Enums.Position.Fixed: UpdatePositionFixed(); break;
        }
    }

    private void UpdatePositionAbsolute()
    {
        _editor.ByPositionStrideX = NearestRelativeParent?.Texture.X ?? 0;
        _editor.ByPositionStrideY = NearestRelativeParent?.Texture.Y ?? 0;
    }

    private void UpdatePositionRelative()
    {
        _editor.ByPositionStrideX = NearestRelativeParent?.Texture.X ?? 0;
        _editor.ByPositionStrideY = NearestRelativeParent?.Texture.Y ?? 0;
    }

    private void UpdatePositionFixed()
    {
        _editor.ByPositionStrideX = 0;
        _editor.ByPositionStrideY = 0;
    }

    #endregion

    #region Обновление стилей Flex

    public void UpdateFlexStyle()
    {
        if (Parent is null || Parent.Style.Display != Display.Flex) return;

        var parent = Parent;
        var items = parent.Childs;
        int count = items.Count;
        int currIndex = Index;
        if (count == 0 || currIndex < 0) return;

        int width = parent.Texture.Width;
        int height = parent.Texture.Height;
        int startX = parent.Texture.X;
        int startY = parent.Texture.Y;

        int gapX = parent.Style.GapX;
        int gapY = parent.Style.GapY;

        bool isRow = parent.Style.FlexPath == FlexPath.FlexRow;
        int alpha = isRow ? 1 : 0;
        int beta = isRow ? 0 : 1;

        int strideX = 0;
        int strideY = 0;

        switch (parent.Style.FlexPosition)
        {
            case FlexPosition.Start:
                // Суммируем размеры всех предшествующих элементов + гэпы (без лишних +1)
                for (int k = 0; k < currIndex; k++)
                {
                    strideX += items[k].Texture.Width + gapX;
                    strideY += items[k].Texture.Height + gapY;
                }
                break;

            case FlexPosition.End:
                int totalUsedWidth = 0;
                int totalUsedHeight = 0;
                for (int k = 0; k < count; k++)
                {
                    totalUsedWidth += items[k].Texture.Width + (k < count - 1 ? gapX : 0);
                    totalUsedHeight += items[k].Texture.Height + (k < count - 1 ? gapY : 0);
                }

                int freeSpaceX = Math.Max(0, width - totalUsedWidth);
                int freeSpaceY = Math.Max(0, height - totalUsedHeight);

                strideX = freeSpaceX;
                strideY = freeSpaceY;
                for (int k = 0; k < currIndex; k++)
                {
                    strideX += items[k].Texture.Width + gapX;
                    strideY += items[k].Texture.Height + gapY;
                }
                break;

            case FlexPosition.Center:
                int realContentWidth = 0;
                int realContentHeight = 0;
                for (int k = 0; k < count; k++)
                {
                    realContentWidth += items[k].Texture.Width + (k < count - 1 ? gapX : 0);
                    realContentHeight += items[k].Texture.Height + (k < count - 1 ? gapY : 0);
                }

                int centerStartX = Math.Max(0, (width - realContentWidth) / 2);
                int centerStartY = Math.Max(0, (height - realContentHeight) / 2);

                strideX = centerStartX;
                strideY = centerStartY;
                for (int k = 0; k < currIndex; k++)
                {
                    strideX += items[k].Texture.Width + gapX;
                    strideY += items[k].Texture.Height + gapY;
                }
                break;

            case FlexPosition.SpaceBetween:
                if (count == 1)
                {
                    strideX = 0;
                    strideY = 0;
                    break;
                }

                // 1. Последний элемент ВСЕГДА жестко прижимается к концу контейнера
                if (currIndex == count - 1)
                {
                    strideX = Math.Max(0, width - items[currIndex].Texture.Width);
                    strideY = Math.Max(0, height - items[currIndex].Texture.Height);
                    break;
                }

                // 2. Для промежуточных элементов распределяем остаток целочисленного деления
                int sumW = 0, sumH = 0;
                for (int k = 0; k < count; k++)
                {
                    sumW += items[k].Texture.Width;
                    sumH += items[k].Texture.Height;
                }

                int totalFreeX = Math.Max(0, width - sumW);
                int totalFreeY = Math.Max(0, height - sumH);
                int gapsCount = count - 1;

                int baseStepX = totalFreeX / gapsCount;
                int remX = totalFreeX % gapsCount; // остаток, который надо распределить

                int baseStepY = totalFreeY / gapsCount;
                int remY = totalFreeY % gapsCount;

                strideX = 0;
                strideY = 0;

                for (int k = 0; k < currIndex; k++)
                {
                    // Добавляем по 1 символу из остатка к первым промежуткам, пока остаток не исчерпан
                    int currentGapX = baseStepX + (k < remX ? 1 : 0);
                    int currentGapY = baseStepY + (k < remY ? 1 : 0);

                    strideX += items[k].Texture.Width + currentGapX;
                    strideY += items[k].Texture.Height + currentGapY;
                }
                break;

            case FlexPosition.SpaceAround:
                if (count == 0)
                {
                    strideX = 0;
                    strideY = 0;
                    break;
                }

                int sumWidth = 0, sumHeight = 0;
                for (int k = 0; k < count; k++)
                {
                    sumWidth += items[k].Texture.Width;
                    sumHeight += items[k].Texture.Height;
                }

                int totalFreeXA = Math.Max(0, width - sumWidth);
                int totalFreeYA = Math.Max(0, height - sumHeight);

                // Базовый размер одного слота свободного места
                int slotX = totalFreeXA / count;
                int remXA = totalFreeXA % count; // остаток, который нужно раздать промежуткам

                int slotY = totalFreeYA / count;
                int remYA = totalFreeYA % count;

                // Начальный отступ первого элемента равен половине слота
                strideX = slotX / 2;
                strideY = slotY / 2;

                for (int k = 0; k < currIndex; k++)
                {
                    // Целый слот между элементами + 1 символ из остатка (пока он не исчерпан)
                    int currentGapX = slotX + (k < remXA ? 1 : 0);
                    int currentGapY = slotY + (k < remYA ? 1 : 0);

                    strideX += items[k].Texture.Width + currentGapX;
                    strideY += items[k].Texture.Height + currentGapY;
                }
                break;
        }

        // Поперечное выравнивание (Cross-axis): если строка, то Y центрируется или прижимается к 0
        int crossY = isRow ? 0 : strideY;
        int crossX = isRow ? strideX : 0;

        _editor.ByFlexX = startX + crossX;
        _editor.ByFlexY = startY + crossY;
    }

    #endregion

    #region Применение стилей Grid

    public void UpdateGridStyle()
    {
        if (Parent is null || Parent.Style.Display != Display.Grid) return;

        var parent = Parent;
        int gridRows = parent.Style.GridRows;
        int gridCols = parent.Style.GridColumns;
        int gapX = parent.Style.GapX;
        int gapY = parent.Style.GapY;
        var childs = parent.Childs;
        int totalChildren = childs.Count;
        int index = Index;

        if (gridCols < 1 || gridRows < 1 || index < 0) return;

        Span<int> maxColWidth = stackalloc int[gridCols];
        Span<int> maxRowHeight = stackalloc int[gridRows];

        // 1. Поиск максимальных размеров колонок и строк (с continue вместо break)
        for (int r = 0; r < gridRows; r++)
        {
            for (int c = 0; c < gridCols; c++)
            {
                int itemIndex = r * gridCols + c;
                if (itemIndex >= totalChildren) continue;

                var tex = childs[itemIndex].Texture;
                if (tex.Width > maxColWidth[c]) maxColWidth[c] = tex.Width;
                if (tex.Height > maxRowHeight[r]) maxRowHeight[r] = tex.Height;
            }
        }

        // 2. Координаты текущей ячейки
        int rowI = index / gridCols;
        int colI = index % gridCols;

        // 3. Вычисление суммарного сдвига
        int shiftX = 0;
        for (int c = 0; c < colI; c++)
        {
            shiftX += maxColWidth[c] + gapX;
        }

        int shiftY = 0;
        for (int r = 0; r < rowI; r++)
        {
            shiftY += maxRowHeight[r] + gapY;
        }

        _editor.ByGridX = parent.Texture.X + shiftX;
        _editor.ByGridY = parent.Texture.Y + shiftY;
    }

    #endregion

    #region Применение стилей

    public void ApplyColorUpdate()
    {
        if (Style.Color.HasValue)
            Texture.ForegroundColor = Style.Color;
        if (Style.BackgroundColor.HasValue)
            Texture.BackgroundColor = Style.BackgroundColor;
    }

    public void ApplyXYUpdate()
    {
        if (Parent is not null && Parent.Style.Display == Display.Flex)
        {
            Texture.X = X + _editor.ByFlexX;
            Texture.Y = Y + _editor.ByFlexY;
        }
        else if (Parent is not null && Parent.Style.Display == Display.Grid)
        {
            Texture.X = X + _editor.ByGridX;
            Texture.Y = Y + _editor.ByGridY;
        }
        else
        {
            Texture.X = X + _editor.ByPositionStrideX + _editor.ByDisplayStrideX;
            Texture.Y = Y + _editor.ByPositionStrideY + _editor.ByDisplayStrideY;
        }
    }

    #endregion
}