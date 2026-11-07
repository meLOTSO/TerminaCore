using Game.Entities.Abstract;

namespace Game.Entities.Civilizations.Abstract;

public interface ICivilizationFactory
{
    public KingHeroBase CreateKingHero();
    public WarriorHeroBase CreateWarriorHero();
    public ArcherHeroBase CreateArcherHero();
    public BankTowerBase CreateBankTower(BankCore bankCore);
}