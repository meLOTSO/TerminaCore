using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Vampyr;

public class VampyrBankTower : BankTowerBase
{
    public override int SummarizeValue => 2;

    public override int Cost => 7;

    public override string Name => "Кровавый склеп";

    public VampyrBankTower(BankCore bankCore) : base(bankCore, 25, 15)
    {

    }
}