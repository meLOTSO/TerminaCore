using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Elf;

public class ElfBankTower : BankTowerBase, ICivilizationEntity
{
    public override string Name => "Древо жизни";

    public CivilizationsType Civilization => CivilizationsType.Elf;

    public override int SummarizeValue => 4;

    public override int Cost => 13;

    public ElfBankTower(BankCore bankCore) : base(bankCore, 35, 12)
    {
        this.Texture.ForegroundColor = ConsoleColor.DarkMagenta;
    }
}