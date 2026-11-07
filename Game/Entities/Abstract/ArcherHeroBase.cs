using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class ArcherHeroBase : HeroBase, ICostEntity
{
    public override Texture Texture => new Texture("D");
    public abstract int Cost { get; }

    protected ArcherHeroBase(int health, int armor) : base(health, armor)
    {
    }
}