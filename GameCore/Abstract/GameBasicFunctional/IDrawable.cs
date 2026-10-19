namespace GameCore.Abstract.GameBasicFunctional;

public interface IDrawable : IEntityFunc
{
    public void Draw(DrawContext context);
}