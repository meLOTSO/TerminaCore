namespace GameCore.Abstract.GameBasicFunctional;

public interface IDrawable : IGameEnvironment
{
    public void Draw(GameCore.Contexts.DrawContext context);
}