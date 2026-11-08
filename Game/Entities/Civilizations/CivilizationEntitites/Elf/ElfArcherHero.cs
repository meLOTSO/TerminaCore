using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Elf;

public class ElfArcherHero : ArcherHeroBase
{
    public override int AttackPlus => 3;

    public override bool[,] StepAreaPart => new bool[,]
    {
        {false, true, true, true},
        {true, true, true, true},
    };

    public override bool[,] DamageAreaPart => new bool[,]
    {
        {false, false, false, false, true, true, true},
        {false, false, false, true, true, true, true},
        {false, false, true, true, true, true, true},
        {false, true, true, true, true, true, true},
        {true, true, true, true, true, true, true},
    };

    public override int Damage => 3;

    public override string Name => "Лесной рейнджер";

    public override int Cost => 3;

    public ElfArcherHero() : base(12, 12)
    {
        this.Texture.ForegroundColor = ConsoleColor.DarkMagenta;
    }
}