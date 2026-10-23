using TerminaCore.Graphics;

public class Nums
{
    public Texture[] NumberTextures { get; private set; } = new Texture[10];
    public Texture DoublePoints { get; private set; }
    public Nums()
    {
        var n0 = new Texture(new List<string>
        {
            "████",
            "█  █",
            "█  █",
            "████"
        });
        var n1 = new Texture(new List<string>
        {
            "  ██",
            " █ █",
            "   █",
            "   █"
        });
        var n2 = new Texture(new List<string>
        {
            "████",
            "   █",
            "██  ",
            "████"
        });
        var n3 = new Texture(new List<string>
        {
            "████",
            "  █ ",
            "   █",
            "████"
        });
        var n4 = new Texture(new List<string>
        {
            "█  █",
            "█  █",
            "████",
            "   █"
        });
        var n5 = new Texture(new List<string>
        {
            "████",
            "█   ",
            "  ██",
            "████"
        });
        var n6 = new Texture(new List<string>
        {
            " ███",
            "█   ",
            "████",
            "████"
        });
        var n7 = new Texture(new List<string>
        {
            "████",
            "   █",
            "   █",
            "   █"
        });
        var n8 = new Texture(new List<string>
        {
            "████",
            " ██ ",
            "████",
            "████"
        });
        var n9 = new Texture(new List<string>
        {
            "████",
            "████",
            "   █",
            "   █"
        });
        var points = new Texture(new List<string>
        {
            " ",
            "#",
            " ",
            "#"
        });
        NumberTextures = new Texture[10] { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9 };
        DoublePoints = points;
    }
}