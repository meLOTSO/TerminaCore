using Game.Entities.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.Abstract;

public abstract class CivilizationModelBase
{
    private readonly ICivilizationFactory _civilizationFactory;
    private readonly BankCore _bankCore = new BankCore();
    private readonly List<HeroBase> _heroes = [];
    private readonly List<TowerBase> _towers = [];

    public event Action<CivilizationModelBase, KingHeroBase>? OnKingDie;
    public event Action<CivilizationModelBase, EntityBase>? OnEntityDie;

    public KingHeroBase King { get; init; }
    public IReadOnlyList<HeroBase> Heroes => _heroes;
    public IReadOnlyList<TowerBase> Towers => _towers;
    public CivilizationType CivilizationType { get; }

    public CivilizationModelBase(CivilizationType civilizationType, ICivilizationFactory civilizationFactory)
    {
        this.CivilizationType = civilizationType;
        _civilizationFactory = civilizationFactory;
        King = _civilizationFactory.CreateKingHero();
        King.OnDie += OnEntityDieTargetInvoke;
        King.OnDie += OnKingDieTargetInvoke;
        _heroes.Add(King);
        this.OnEntityDie += OnEntityDieHandler;
    }

    public virtual HeroBase AddHero(HeroType heroType)
    {
        if (heroType == HeroType.King) return King;

        HeroBase hero = heroType switch
        {
            HeroType.Warrior => _civilizationFactory.CreateWarriorHero(),
            HeroType.Archer => _civilizationFactory.CreateArcherHero(),
            _ => throw new ArgumentException($"Тип персонажа {heroType.GetType().Name} не зарегистрирован.")
        };
        hero.OnDie += OnEntityDieTargetInvoke;
        _heroes.Add(hero);
        return hero;
    }

    public HeroBase? RemoveFirstHero(Predicate<HeroBase> predicate)
    {
        for (int i = 0; i < _heroes.Count; i++)
        {
            if (predicate.Invoke(_heroes[i]))
            {
                _heroes[i].OnDie -= OnEntityDieTargetInvoke;
                var hero = _heroes[i];
                _heroes.RemoveAt(i);
                return hero;
            }
        }
        return null;
    }

    public virtual TowerBase AddTower(TowerType towerType)
    {
        TowerBase towerBase = towerType switch
        {
            TowerType.Bank => _civilizationFactory.CreateBankTower(_bankCore),
            _ => throw new ArgumentException($"Тип здания {towerType.GetType().Name} не зарегистрирован.")
        };
        towerBase.OnDie += OnEntityDieTargetInvoke;
        _towers.Add(towerBase);
        return towerBase;
    }

    public TowerBase? RemoveFirstTower(Predicate<TowerBase> predicate)
    {
        for (int i = 0; i < _towers.Count; i++)
        {
            if (predicate.Invoke(_towers[i]))
            {
                _towers[i].OnDie -= OnEntityDieTargetInvoke;
                var tower = _towers[i];
                _towers.RemoveAt(i);
                return tower;
            }
        }
        return null;
    }

    public EntityBase? RemoveFirstEntity(Predicate<EntityBase> predicate)
    {
        for (int i = 0; i < _heroes.Count; i++)
        {
            if (predicate.Invoke(_heroes[i]))
            {
                _heroes[i].OnDie -= OnEntityDieTargetInvoke;
                var hero = _heroes[i];
                _heroes.RemoveAt(i);
                return hero;
            }
        }
        for (int i = 0; i < _towers.Count; i++)
        {
            if (predicate.Invoke(_towers[i]))
            {
                _towers[i].OnDie -= OnEntityDieTargetInvoke;
                var tower = _towers[i];
                _towers.RemoveAt(i);
                return tower;
            }
        }
        return null;
    }

    public IEnumerable<HeroBase> HeroesWhere(Predicate<HeroBase> predicate)
    {
        foreach (var hero in Heroes)
            if (predicate.Invoke(hero))
                yield return hero;
    }
    public HeroBase? HeroesFirst(Predicate<HeroBase> predicate)
    {
        foreach (var hero in Heroes)
            if (predicate.Invoke(hero))
                return hero;
        return null;
    }

    public IEnumerable<TowerBase> TowersWhere(Predicate<TowerBase> predicate)
    {
        foreach (var tower in Towers)
            if (predicate.Invoke(tower))
                yield return tower;
    }
    public TowerBase? TowersFirst(Predicate<TowerBase> predicate)
    {
        foreach (var tower in Towers)
            if (predicate.Invoke(tower))
                return tower;
        return null;
    }

    public IEnumerable<EntityBase> EntitiesWhere(Predicate<EntityBase> predicate)
    {
        foreach (var hero in Heroes)
            if (predicate.Invoke(hero))
                yield return hero;

        foreach (var tower in Towers)
            if (predicate.Invoke(tower))
                yield return tower;
    }

    private void OnEntityDieTargetInvoke(EntityBase entity)
    {
        OnEntityDie?.Invoke(this, entity);
    }
    private void OnKingDieTargetInvoke(EntityBase entity)
    {
        OnKingDie?.Invoke(this, King);
    }

    private void OnEntityDieHandler(CivilizationModelBase civilizationModel, EntityBase entity)
    {
        for (int i = 0; i < _heroes.Count; i++)
            if (_heroes[i].Id == entity.Id)
            {
                _heroes[i].OnDie -= OnEntityDieTargetInvoke;
                _heroes.RemoveAt(i);
                break;
            }
        for (int i = 0; i < _towers.Count; i++)
            if (_towers[i].Id == entity.Id)
            {
                _heroes.RemoveAt(i);
                break;
            }
    }
}