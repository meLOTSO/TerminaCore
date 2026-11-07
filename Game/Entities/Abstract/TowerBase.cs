using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class TowerBase : EntityBase
{
    public event Action<EntityBase>? OnTowerEffectApplied;

    public TowerBase(int health, int armor) : base(health, armor)
    {
    }

    public virtual void ApplyTowerEffect(EntityBase entityToApplyEffect)
    {
        OnTowerEffectApplied?.Invoke(entityToApplyEffect);
    }
}