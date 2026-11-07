using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Ork;

public class OrkBankTower : BankTowerBase, ICivilizationEntity
{
    public override int SummarizeValue => 2;

    public override int Cost => 6;

    public override string Name => "Лагерь налетчиков";

    public CivilizationsType Civilization => CivilizationsType.Ork;

    public OrkBankTower(BankCore bankCore) : base(bankCore, 45, 10)
    {

    }
}