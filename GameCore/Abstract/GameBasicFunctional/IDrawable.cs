namespace GameCore.Abstract.GameBasicFunctional;

public interface IDrawable : IEntity
{
    public void Draw(GameCore.Contexts.DrawContext context);
}