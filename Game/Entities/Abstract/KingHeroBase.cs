using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class KingHeroBase : HeroBase, IBank
{
    public override Texture Texture => new Texture("^");

    public abstract int SummarizeValue { get; }

    protected KingHeroBase(int health, int armor) : base(health, armor)
    {
    }
}