using TerminaCore.DOMCore.Abstract;
using TerminaCore.DOMCore.Enums;

namespace TerminaCore.DOMCore.Style;

public class StyleConfiguration
{
    private readonly ModelNode _node;
    private StyleEditor _styleEditor;

    public StyleConfiguration(ModelNode node)
    {
        _node = node;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public Display Display { get; set; } = Display.Block;
    public Enums.Position Position { get; set; } = Enums.Position.Relative;
    public FlexPath FlexPath { get; set; } = FlexPath.FlexRow;
    public GridPath GridPath { get; set; } = GridPath.GridRow;
    public FlexPosition FlexPosition { get; set; } = FlexPosition.Start;
    public FlexPosition JustifyContent { get; set; } = FlexPosition.Start;
    public int GridRows { get; set; }
    public int GridColumns { get; set; }
    public int GapX { get; set; }
    public int GapY { get; set; }
    public ConsoleColor? Color { get => _node.Texture.ForegroundColor; set => _node.Texture.ForegroundColor = value; }
    public ConsoleColor? BackgroundColor { get => _node.Texture.BackgroundColor; set => _node.Texture.BackgroundColor = value; }

    #region Обновление стилей

    public void Update()
    {
        // 1. Сброс предыдущих расчетов смещений
        _styleEditor = new StyleEditor();

        // 2. Базовое позиционирование (Relative / Absolute / Fixed)
        UpdatePosition();

        // 3. Вычисление потока компоновки в зависимости от режима родителя/элемента
        if (_node.Parent is not null && _node.Parent.Style.Display == Display.Flex)
        {
            UpdateFlexStyle();
        }
        else if (_node.Parent is not null && _node.Parent.Style.Display == Display.Grid)
        {
            UpdateGridStyle();
        }
        else
        {
            UpdateDisplay();
        }

        // 4. Финальное применение
        ApplyXYUpdate();
    }

    public void UpdateBFS()
    {
        Queue<ModelNode> queue = new();
        queue.Enqueue(_node);

        while (queue.TryDequeue(out ModelNode? node))
        {
            node.Style.Update();
            foreach (var child in node.Childs)
            {
                queue.Enqueue(child);
            }
        }
    }

    #endregion

    #region Обновление стилей Display { Block, Inline, Flex, Grid }

    private void UpdateDisplay()
    {
        switch (this.Display)
        {
            case Display.Block: UpdateDisplayBlock(); break;
            case Display.Inline: UpdateDisplayInline(); break;
            case Display.Flex: UpdateDisplayFlex(); break;
            case Display.Grid: UpdateDisplayGrid(); break;
        }
    }

    private void UpdateDisplayBlock()
    {
        if (_node.PrevNode is null) return;

        var prev = _node.PrevNode;
        int strideY = prev.Texture.Y + prev.Texture.Height + 1;

        if (prev.Style.Display == Display.Inline)
        {
            strideY = -1;
            for (int i = _node.Index - 1; i >= 0; i--)
            {
                if (_node.Neighbors[i].Style.Display == Display.Inline)
                    strideY = Math.Max(strideY, _node.Neighbors[i].Texture.Y + _node.Neighbors[i].Texture.Height + 1);
                else break;
            }
        }
        _styleEditor.ByDisplayStrideX = 0;
        _styleEditor.ByDisplayStrideY = strideY;
    }

    private void UpdateDisplayInline()
    {
        if (_node.PrevNode is null) return;

        var prev = _node.PrevNode;
        if (prev.Style.Display == Display.Inline)
        {
            _styleEditor.ByDisplayStrideX = prev.Texture.X + prev.Texture.Width + 1;
            _styleEditor.ByDisplayStrideY = prev.Texture.Y;
        }
        else
        {
            _styleEditor.ByDisplayStrideX = 0;
            _styleEditor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
        }
    }

    private void UpdateDisplayFlex()
    {
        if (_node.PrevNode is null) return;

        var prev = _node.PrevNode;
        _styleEditor.ByDisplayStrideX = 0;
        _styleEditor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
    }

    private void UpdateDisplayGrid()
    {
        if (_node.PrevNode is null) return;

        var prev = _node.PrevNode;
        _styleEditor.ByDisplayStrideX = 0;
        _styleEditor.ByDisplayStrideY = prev.Texture.Y + prev.Texture.Height + 1;
    }

    #endregion

    #region Обновление стилей Position { Absolute, Relative, Fixed }

    public void UpdatePosition()
    {
        switch (this.Position)
        {
            case Enums.Position.Absolute: UpdatePositionAbsolute(); break;
            case Enums.Position.Relative: UpdatePositionRelative(); break;
            case Enums.Position.Fixed: UpdatePositionFixed(); break;
        }
    }

    private void UpdatePositionAbsolute()
    {
        _styleEditor.ByPositionStrideX = _node.NearestRelativeParent?.Texture.X ?? 0;
        _styleEditor.ByPositionStrideY = _node.NearestRelativeParent?.Texture.Y ?? 0;
    }

    private void UpdatePositionRelative()
    {
        _styleEditor.ByPositionStrideX = _node.NearestRelativeParent?.Texture.X ?? 0;
        _styleEditor.ByPositionStrideY = _node.NearestRelativeParent?.Texture.Y ?? 0;
    }

    private void UpdatePositionFixed()
    {
        _styleEditor.ByPositionStrideX = 0;
        _styleEditor.ByPositionStrideY = 0;
    }

    #endregion

    #region Обновление стилей Flex

    public void UpdateFlexStyle()
    {
        if (_node.Parent is null || _node.Parent.Style.Display != Display.Flex) return;

        var parent = _node.Parent;
        var items = parent.Childs;
        int count = items.Count;
        int currIndex = _node.Index;
        if (count == 0 || currIndex < 0) return;

        int parentWidth = parent.Texture.Width;
        int parentHeight = parent.Texture.Height;
        int startX = parent.Texture.X;
        int startY = parent.Texture.Y;

        int gapX = parent.Style.GapX;
        int gapY = parent.Style.GapY;

        bool isRow = parent.Style.FlexPath == FlexPath.FlexRow;

        int mainContainerSize = isRow ? parentWidth : parentHeight;
        int crossContainerSize = isRow ? parentHeight : parentWidth;
        int mainGap = isRow ? gapX : gapY;

        int mainStride = 0;

        switch (parent.Style.JustifyContent)
        {
            case FlexPosition.Start:
                for (int k = 0; k < currIndex; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    mainStride += itemSize + mainGap;
                }
                break;

            case FlexPosition.End:
                int totalUsedMain = 0;
                for (int k = 0; k < count; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    totalUsedMain += itemSize + (k < count - 1 ? mainGap : 0);
                }

                int freeSpaceMain = Math.Max(0, mainContainerSize - totalUsedMain);
                mainStride = freeSpaceMain;
                for (int k = 0; k < currIndex; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    mainStride += itemSize + mainGap;
                }
                break;

            case FlexPosition.Center:
                int realContentSize = 0;
                for (int k = 0; k < count; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    realContentSize += itemSize + (k < count - 1 ? mainGap : 0);
                }

                int centerStart = Math.Max(0, (mainContainerSize - realContentSize) / 2);
                mainStride = centerStart;
                for (int k = 0; k < currIndex; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    mainStride += itemSize + mainGap;
                }
                break;

            case FlexPosition.SpaceBetween:
                if (count == 1)
                {
                    mainStride = 0;
                    break;
                }

                int thisItemSize = isRow ? items[currIndex].Texture.Width : items[currIndex].Texture.Height;

                if (currIndex == count - 1)
                {
                    mainStride = Math.Max(0, mainContainerSize - thisItemSize);
                    break;
                }

                int sumMainSize = 0;
                for (int k = 0; k < count; k++)
                {
                    sumMainSize += isRow ? items[k].Texture.Width : items[k].Texture.Height;
                }

                int totalFreeSpace = Math.Max(0, mainContainerSize - sumMainSize);
                int gapsCount = count - 1;
                int baseStep = totalFreeSpace / gapsCount;
                int remStep = totalFreeSpace % gapsCount;

                mainStride = 0;
                for (int k = 0; k < currIndex; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    int currentGap = baseStep + (k < remStep ? 1 : 0);
                    mainStride += itemSize + currentGap;
                }
                break;

            case FlexPosition.SpaceAround:
                if (count == 0)
                {
                    mainStride = 0;
                    break;
                }

                int sumAroundSize = 0;
                for (int k = 0; k < count; k++)
                {
                    sumAroundSize += isRow ? items[k].Texture.Width : items[k].Texture.Height;
                }

                int totalFreeAround = Math.Max(0, mainContainerSize - sumAroundSize);
                int slot = totalFreeAround / count;
                int remAround = totalFreeAround % count;

                mainStride = slot / 2;
                for (int k = 0; k < currIndex; k++)
                {
                    int itemSize = isRow ? items[k].Texture.Width : items[k].Texture.Height;
                    int currentGap = slot + (k < remAround ? 1 : 0);
                    mainStride += itemSize + currentGap;
                }
                break;
        }

        int myCrossSize = isRow ? _node.Texture.Height : _node.Texture.Width;
        int crossOffset = parent.Style.FlexPosition switch
        {
            FlexPosition.End => Math.Max(0, crossContainerSize - myCrossSize),
            FlexPosition.Center => Math.Max(0, (crossContainerSize - myCrossSize) / 2),
            _ => 0 // FlexPosition.Start
        };

        int finalX = isRow ? mainStride : crossOffset;
        int finalY = isRow ? crossOffset : mainStride;

        _styleEditor.ByFlexX = startX + finalX;
        _styleEditor.ByFlexY = startY + finalY;
    }

    #endregion

    #region Применение стилей Grid

    public void UpdateGridStyle()
    {
        if (_node.Parent is null || _node.Parent.Style.Display != Display.Grid) return;

        var parent = _node.Parent;
        int gridRows = parent.Style.GridRows;
        int gridCols = parent.Style.GridColumns;
        int gapX = parent.Style.GapX;
        int gapY = parent.Style.GapY;
        var childs = parent.Childs;
        int totalChildren = childs.Count;
        int index = _node.Index;

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

        _styleEditor.ByGridX = parent.Texture.X + shiftX;
        _styleEditor.ByGridY = parent.Texture.Y + shiftY;
    }

    #endregion

    #region Применение стилей

    public void ApplyXYUpdate()
    {
        if (_node.Parent is not null && _node.Parent.Style.Display == Display.Flex)
        {
            _node.Texture.X = X + _styleEditor.ByFlexX;
            _node.Texture.Y = Y + _styleEditor.ByFlexY;
        }
        else if (_node.Parent is not null && _node.Parent.Style.Display == Display.Grid)
        {
            _node.Texture.X = X + _styleEditor.ByGridX;
            _node.Texture.Y = Y + _styleEditor.ByGridY;
        }
        else
        {
            _node.Texture.X = X + _styleEditor.ByPositionStrideX + _styleEditor.ByDisplayStrideX;
            _node.Texture.Y = Y + _styleEditor.ByPositionStrideY + _styleEditor.ByDisplayStrideY;
        }
    }

    #endregion
}