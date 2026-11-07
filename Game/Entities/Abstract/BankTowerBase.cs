using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class BankTowerBase : TowerBase, ICostEntity, IBank
{
    private readonly BankCore _bankCore;

    public abstract int SummarizeValue { get; }

    public override Texture Texture => new Texture("$");

    public abstract int Cost { get; }

    public BankTowerBase(BankCore bankCore, int health, int armor) : base(health, armor)
    {
        _bankCore = bankCore;
    }

    public override void ApplyTowerEffect(EntityBase entityToApplyEffect)
    {
        base.ApplyTowerEffect(entityToApplyEffect);
        _bankCore.IntentionToSummarize(SummarizeValue);
        _bankCore.ApplyIntention();
    }
}