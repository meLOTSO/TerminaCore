using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class WarriorHeroBase : HeroBase, ICostEntity
{
    public override Texture Texture => new Texture("/");
    public abstract int Cost { get; }

    protected WarriorHeroBase(int health, int armor) : base(health, armor)
    {
    }
}