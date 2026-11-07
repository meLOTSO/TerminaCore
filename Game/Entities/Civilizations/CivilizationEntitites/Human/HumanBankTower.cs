using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Human;

public class HumanBankTower : BankTowerBase, ICivilizationEntity
{
    public override int SummarizeValue => 2;

    public override Texture Texture => new Texture("$", ConsoleColor.DarkBlue);
    public override string Name => "Казначейство";

    public CivilizationsType Civilization => CivilizationsType.Human;

    public override int Cost => 9;

    public HumanBankTower(BankCore bankCore) : base(bankCore, 30, 12)
    {
    }
}