using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.CivilizationEntities.Elf;

namespace Game.Entities.Civilizations.Factories;

public class ElfCivilizationFactory : ICivilizationFactory
{
    public ArcherHeroBase CreateArcherHero() => new ElfArcherHero();
    public BankTowerBase CreateBankTower(BankCore bankCore) => new ElfBankTower(bankCore);
    public KingHeroBase CreateKingHero() => new ElfKingHero();
    public WarriorHeroBase CreateWarriorHero() => new ElfWarriorHero();
}