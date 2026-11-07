using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.CivilizationEntities.Human;

namespace Game.Entities.Civilizations.Factories;

public class HumanCivilizationFactory : ICivilizationFactory
{
    public ArcherHeroBase CreateArcherHero() => new HumanArcherHero();
    public BankTowerBase CreateBankTower(BankCore bankCore) => new HumanBankTower(bankCore);
    public KingHeroBase CreateKingHero() => new HumanKingHero();
    public WarriorHeroBase CreateWarriorHero() => new HumanWarriorHero();
}