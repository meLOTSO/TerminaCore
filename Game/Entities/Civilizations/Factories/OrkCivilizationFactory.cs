using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.CivilizationEntities.Ork;

namespace Game.Entities.Civilizations.Factories;

public class OrkCivilizationFactory : ICivilizationFactory
{
    public ArcherHeroBase CreateArcherHero() => new OrkArcherHero();
    public BankTowerBase CreateBankTower(BankCore bankCore) => new OrkBankTower(bankCore);
    public KingHeroBase CreateKingHero() => new OrkKingHero();
    public WarriorHeroBase CreateWarriorHero() => new OrkWarriorHero();
}