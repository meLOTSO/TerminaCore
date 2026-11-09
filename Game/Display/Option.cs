using System;
using System.Collections.Generic;
using System.Linq;
using Game.Display.Abstract;
using Game.Tools.Abstract;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.GameCore.Contexts;
using TerminaCore.GameCore.Extensions;
using TerminaCore.Graphics;

public class Option : IOption, IControl, IDrawable
{
    private readonly List<ISelect> _selects = [];
    private readonly Indexator<ISelect> _indexator;
    private bool _active = false;

    public Texture LabelTexture { get; private set; }
    public int? MaxSelectedSelects { get; set; }
    public IReadOnlyList<ISelect> Selects => _selects;
    public bool ShowSelectedSelects { get; set; } = true;
    public bool IsActive => _active;

    public event Action<ISelect>? OnSelectAdd;
    public event Action<ISelect>? OnSelectRemove;
    public event Action<ISelect>? OnSelectActiveIn;
    public event Action<ISelect>? OnSelectActiveOut;
    public event Action<ISelect>? OnSelectEnter;
    public event Action<ISelect>? OnSelectEnterOut;

    public ConsoleColor? LabelActiveColor { get; set; } = ConsoleColor.DarkGray;
    public ConsoleColor? LabelActiveBgColor { get; set; }
    public ConsoleColor? LabelNonActiveColor { get; set; } = ConsoleColor.White;
    public ConsoleColor? LabelNonActiveBgColor { get; set; }
    public ConsoleColor? SelectedSelectColor { get; set; } = ConsoleColor.White;
    public ConsoleColor? SelectedSelectBgColor { get; set; }
    public ConsoleColor? NonSelectedSelectColor { get; set; } = ConsoleColor.Gray;
    public ConsoleColor? NonSelectedSelectBgColor { get; set; }
    public ConsoleColor? FocusNonSelectedSelectColor { get; set; } = ConsoleColor.Black;
    public ConsoleColor? FocusNonSelectedSelectBgColor { get; set; }
    public ConsoleColor? FocusSelectedSelectColor { get; set; } = ConsoleColor.DarkRed;
    public ConsoleColor? FocusSelectedSelectBgColor { get; set; }

    public Option(string label)
    {
        _indexator = new Indexator<ISelect>(_selects);
        LabelTexture = new Texture(label);
        LabelTexture.IgnoreSet.Clear();
        Close();

        _indexator.OnIndexIn += SelectInHandler;
        _indexator.OnIndexOut += SelectOutHandler;

        OnSelectAdd += select =>
        {
            select.Texture.ForegroundColor = NonSelectedSelectColor;
            select.Texture.BackgroundColor = NonSelectedSelectBgColor;
        };
    }

    public void OnKeyPress(ControlContext controlContext)
    {
        if (controlContext.Key == ConsoleKey.Enter)
        {
            if (_active)
                TrySelectEnter();
            else
                Open();
        }
        else if (controlContext.Key == ConsoleKey.Escape && _active)
        {
            Close();
        }
        else if (controlContext.Key == ConsoleKey.UpArrow && _active)
        {
            _indexator.TryPrevIndex();
        }
        else if (controlContext.Key == ConsoleKey.DownArrow && _active)
        {
            _indexator.TryNextIndex();
        }
    }

    public void Draw(DrawContext drawContext)
    {
        drawContext.Draw(LabelTexture);

        if (_active)
        {
            for (int i = 0; i < _selects.Count; i++)
            {
                _selects[i].Texture.X = LabelTexture.X;
                _selects[i].Texture.Y = LabelTexture.Y + i + 1;
                drawContext.Draw(_selects[i].Texture);
            }
        }
        else if (ShowSelectedSelects)
        {
            int gap = 1;
            foreach (var s in _selects)
            {
                if (s.IsSelected)
                {
                    s.Texture.X = LabelTexture.X;
                    s.Texture.Y = LabelTexture.Y + gap;
                    drawContext.Draw(s.Texture);
                    gap++;
                }
            }
        }
    }

    public void Open()
    {
        _active = true;
        LabelTexture.ForegroundColor = LabelActiveColor;
        LabelTexture.BackgroundColor = LabelActiveBgColor;

        _indexator.Reset();

        for (int i = 0; i < _selects.Count; i++)
        {
            SelectOutHandler(_selects[i]);
        }

        if (_selects.Count > 0)
        {
            SelectInHandler(_selects[0]);
        }
    }

    public void Close()
    {
        _active = false;
        LabelTexture.ForegroundColor = LabelNonActiveColor;
        LabelTexture.BackgroundColor = LabelNonActiveBgColor;

        // При закрытии сбрасываем цвета всех пунктов в стандартный вид (выбран / не выбран)
        foreach (var s in _selects)
        {
            SelectOutHandler(s);
        }
    }

    public bool TryNext() => _indexator.TryNextIndex();
    public bool TryPrev() => _indexator.TryPrevIndex();

    public void SelectInHandler(ISelect select)
    {
        if (select.IsSelected)
        {
            select.Texture.ForegroundColor = FocusSelectedSelectColor;
            select.Texture.BackgroundColor = FocusSelectedSelectBgColor;
        }
        else
        {
            select.Texture.ForegroundColor = FocusNonSelectedSelectColor;
            select.Texture.BackgroundColor = FocusNonSelectedSelectBgColor;
        }
    }

    public void SelectOutHandler(ISelect select)
    {
        if (select.IsSelected)
        {
            select.Texture.ForegroundColor = SelectedSelectColor;
            select.Texture.BackgroundColor = SelectedSelectBgColor;
        }
        else
        {
            select.Texture.ForegroundColor = NonSelectedSelectColor;
            select.Texture.BackgroundColor = NonSelectedSelectBgColor;
        }
    }

    public bool TrySelectEnter()
    {
        if (_indexator.TryGetCurrentValue(out ISelect? select) && select is not null)
        {
            if (select.IsSelected)
            {
                select.SelectOut();
                SelectInHandler(select);
                return true;
            }

            select.SelectThis();

            // Если задан лимит выбора, вытесняем самый ранний выбранный пункт
            if (MaxSelectedSelects.HasValue && _selects.Count(s => s.IsSelected) > MaxSelectedSelects.Value)
            {
                var firstSelected = _selects.FirstOrDefault(s => s.IsSelected && !ReferenceEquals(s, select));
                if (firstSelected is not null)
                {
                    firstSelected.SelectOut();
                    SelectOutHandler(firstSelected);
                }
            }

            SelectInHandler(select);
            return true;
        }

        return false;
    }

    public void TargetSelectActiveInInvoke(ISelect select) => OnSelectActiveIn?.Invoke(select);
    public void TargetSelectActiveOutInvoke(ISelect select) => OnSelectActiveOut?.Invoke(select);
    public void TargetSelectEnterInvoke(ISelect select) => OnSelectEnter?.Invoke(select);
    public void TargetSelectEnterOutInvoke(ISelect select) => OnSelectEnterOut?.Invoke(select);

    public void RegisterTargetEventsFor(ISelect select)
    {
        select.OnActiveIn += TargetSelectActiveInInvoke;
        select.OnActiveOut += TargetSelectActiveOutInvoke;
        select.OnSelect += TargetSelectEnterInvoke;
        select.OnSelectOut += TargetSelectEnterOutInvoke;
    }

    public void UnregisterTargetEventsFor(ISelect select)
    {
        select.OnActiveIn -= TargetSelectActiveInInvoke;
        select.OnActiveOut -= TargetSelectActiveOutInvoke;
        select.OnSelect -= TargetSelectEnterInvoke;
        select.OnSelectOut -= TargetSelectEnterOutInvoke;
    }

    public void AddSelect(ISelect select)
    {
        _selects.Add(select);
        RegisterTargetEventsFor(select);
        OnSelectAdd?.Invoke(select);
    }

    public void AddSelect(string text)
    {
        var select = new Select(text);
        AddSelect(select);
    }

    public void RemoveSelect(ISelect select)
    {
        bool removed = _selects.Remove(select);
        if (removed)
        {
            UnregisterTargetEventsFor(select);
            OnSelectRemove?.Invoke(select);
            _indexator.Reset();
        }
    }

    public void RemoveSelect(string text)
    {
        var select = _selects.FirstOrDefault(s =>
            new string(s.Texture.Buffer).Trim().Equals(text, StringComparison.OrdinalIgnoreCase));

        if (select is not null)
            RemoveSelect(select);
    }

    #region Методы для настройки цветов

    public void SetColorsForLabel(bool forActive, ConsoleColor? fg, ConsoleColor? bg)
    {
        if (forActive) (LabelActiveColor, LabelActiveBgColor) = (fg, bg);
        else (LabelNonActiveColor, LabelNonActiveBgColor) = (fg, bg);
    }

    public void SetColorsForSelected(bool forSelected, ConsoleColor? fg, ConsoleColor? bg)
    {
        if (forSelected) (SelectedSelectColor, SelectedSelectBgColor) = (fg, bg);
        else (NonSelectedSelectColor, NonSelectedSelectBgColor) = (fg, bg);
    }

    public void SetColorsForFocusSelect(bool forSelected, ConsoleColor? fg, ConsoleColor? bg)
    {
        if (forSelected) (FocusSelectedSelectColor, FocusSelectedSelectBgColor) = (fg, bg);
        else (FocusNonSelectedSelectColor, FocusNonSelectedSelectBgColor) = (fg, bg);
    }

    #endregion
}