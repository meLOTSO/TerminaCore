using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.CivilizationEntities.Vampyr;

namespace Game.Entities.Civilizations.Factories;

public class VampyrCivilizationFactory : ICivilizationFactory
{
    public ArcherHeroBase CreateArcherHero() => new VampyrArcherHero();
    public BankTowerBase CreateBankTower(BankCore bankCore) => new VampyrBankTower(bankCore);
    public KingHeroBase CreateKingHero() => new VampyrKingHero();
    public WarriorHeroBase CreateWarriorHero() => new VampyrWarriorHero();
}
