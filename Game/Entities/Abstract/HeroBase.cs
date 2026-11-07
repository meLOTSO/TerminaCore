using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class HeroBase : EntityBase
{
    public abstract int AttackPlus { get; }
    public abstract bool[,] StepAreaPart { get; }
    public abstract bool[,] DamageAreaPart { get; }
    public abstract int Damage { get; }

    public bool[,] StepArea { get; }
    public bool[,] DamageArea { get; }
    public Texture StepAreaTexture { get; }
    public Texture DamageAreaTexture { get; }

    public HeroBase(int health, int armor) : base(health, armor)
    {
        StepArea = Area.CreateFullAreaFromPart(StepAreaPart);
        DamageArea = Area.CreateFullAreaFromPart(DamageAreaPart);
        StepAreaTexture = Area.GetTextureFromArea(StepArea);
        DamageAreaTexture = Area.GetTextureFromArea(DamageArea);
        UpdateAreaX(Texture.X);
        UpdateAreaY(Texture.Y);
    }

    public virtual bool TryAttack(EntityBase otherEntity)
    {
        if (Random.Shared.Next(MAX_ARMOR + 1) >= otherEntity.Armor)
        {
            otherEntity.GetDamage(this.Damage);
            return true;
        }
        return false;
    }

    public override void SetX(int x)
    {
        base.SetX(x);
        UpdateAreaX(x);
    }

    public override void SetY(int y)
    {
        base.SetY(y);
        UpdateAreaY(y);
    }

    public override void SetXY(int x, int y)
    {
        base.SetXY(x, y);
        UpdateAreaX(x);
        UpdateAreaY(y);
    }

    private void UpdateAreaX(int x)
    {
        StepAreaTexture.X = x - StepAreaTexture.Width / 2;
        DamageAreaTexture.X = x - DamageAreaTexture.Width / 2;
    }
    private void UpdateAreaY(int y)
    {
        StepAreaTexture.Y = y - StepAreaTexture.Height / 2;
        DamageAreaTexture.Y = y - DamageAreaTexture.Height / 2;
    }
}