using Game.Entities.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.Abstract;

public abstract class CivilizationModelBase
{
    private readonly ICivilizationFactory _civilizationFactory;
    private readonly BankCore _bankCore = new BankCore();
    private readonly List<HeroBase> _heroes = [];
    private readonly List<TowerBase> _towers = [];

    public KingHeroBase King { get; init; }
    public IReadOnlyList<HeroBase> Heroes => _heroes;
    public IReadOnlyList<TowerBase> Towers => _towers;

    public CivilizationModelBase(ICivilizationFactory civilizationFactory)
    {
        _civilizationFactory = civilizationFactory;
        King = _civilizationFactory.CreateKingHero();
        _heroes.Add(King);
    }

    public virtual HeroBase AddHero(HeroType heroType)
    {
        switch (heroType)
        {
            case HeroType.King: return King;
            case HeroType.Warrior:
                var warrior = _civilizationFactory.CreateWarriorHero();
                _heroes.Add(warrior);
                return warrior;
            case HeroType.Archer:
                var archer = _civilizationFactory.CreateArcherHero();
                _heroes.Add(archer);
                return archer;
            default: throw new ArgumentException($"Тип персонажа {heroType.GetType().Name} не зарегистрирован.");
        }
    }

    public virtual TowerBase AddTower(TowerType towerType)
    {
        switch (towerType)
        {
            case TowerType.Bank:
                var bank = _civilizationFactory.CreateBankTower(_bankCore);
                _towers.Add(bank);
                return bank;
            default: throw new ArgumentException($"Тип здания {towerType.GetType().Name} не зарегистрирован.");
        }
    }

    public IEnumerable<HeroBase> HeroesWhere(Predicate<HeroBase> predicate)
    {
        foreach (var hero in Heroes)
        {
            if (predicate.Invoke(hero))
                yield return hero;
        }
    }

    public IEnumerable<TowerBase> TowersWhere(Predicate<TowerBase> predicate)
    {
        foreach (var tower in Towers)
        {
            if (predicate.Invoke(tower))
                yield return tower;
        }
    }
}