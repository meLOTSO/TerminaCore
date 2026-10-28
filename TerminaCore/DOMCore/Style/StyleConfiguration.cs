using TerminaCore.DOMCore.Enums;

namespace TerminaCore.DOMCore.Style;

public class StyleConfiguration
{
    public StyleConfiguration()
    {
    }

    public Display Display { get; set; } = Display.Block;
    public Enums.Position Position { get; set; } = Enums.Position.Relative;
    public FlexPath FlexPath { get; set; } = FlexPath.FlexRow;
    public GridPath GridPath { get; set; } = GridPath.GridRow;
    public FlexPosition FlexPosition { get; set; } = FlexPosition.Start;
    public int GridRows { get; set; }
    public int GridColumns { get; set; }
    public ConsoleColor? Color { get; set; } = null;
    public ConsoleColor? BackgroundColor { get; set; } = null;
    public int GapX { get; set; }
    public int GapY { get; set; }
}