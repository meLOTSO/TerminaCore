namespace TerminaCore.GameCore.Enums;

[Flags]
public enum GameFunctions : byte
{
    None = 0,
    Update = 1 << 0,
    OnKeyPress = 1 << 1,
    Draw = 1 << 2,
    All = Update | OnKeyPress | Draw
}