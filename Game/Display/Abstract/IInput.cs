using System.Text;
using TerminaCore.GameCore.Abstract.GameBasicFunctional;
using TerminaCore.Graphics;

namespace Game.Display.Abstract;

public interface IInput
{
    public Texture InputTexture { get; }
    public Texture LabelTexture { get; }
    public StringBuilder Input { get; }
    public string Label { get; }

    public void AddSymbol(char symbol);
    public char Backspace();
}