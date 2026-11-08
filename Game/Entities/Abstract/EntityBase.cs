using TerminaCore.Graphics;

namespace Game.Entities.Abstract;

public abstract class EntityBase
{
    private int _x = 0;
    private int _y = 0;

    protected const int MAX_ARMOR = 20;

    protected int HealthValue;

    public Guid Id { get; } = Guid.NewGuid();
    public abstract string Name { get; }
    public abstract Texture Texture { get; }

    public int X { get => _x; set => SetX(value); }
    public int Y { get => _y; set => SetY(value); }
    public int Armor { get; }
    public int Health { get => HealthValue; }
    public event Action<EntityBase>? OnDamageGet;
    public event Action<EntityBase>? OnDie;

    public EntityBase(int health, int armor)
    {
        if (armor > MAX_ARMOR) throw new ArgumentException($"Значение Armor не может быть больше {MAX_ARMOR}.");

        Armor = armor;
        HealthValue = health;
    }

    public void GetDamage(int damageValue)
    {
        HealthValue -= damageValue;
        OnDamageGet?.Invoke(this);
        if (HealthValue <= 0) OnDie?.Invoke(this);
    }

    public virtual void SetX(int x)
    {
        _x = x;
        Texture.X = x;
    }

    public virtual void SetY(int y)
    {
        _y = y;
        Texture.Y = y;
    }

    public virtual void SetXY(int x, int y)
    {
        SetX(x);
        SetY(y);
    }
}