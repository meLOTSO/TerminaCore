using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Elf;

public class ElfKingHero : KingHeroBase
{
    public override int AttackPlus => 0;

    public override bool[,] StepAreaPart => new bool[,]
    {
        {false, true, true, true},
        {true, true, true, true}
    };

    public override bool[,] DamageAreaPart => new bool[,]
    {
        {false, true, true},
        {true, true, true},
    };

    public override int Damage => 3;

    public override string Name => "Владыка леса";

    public override int SummarizeValue => 3;

    public ElfKingHero() : base(60, 14)
    {
        this.Texture.ForegroundColor = ConsoleColor.DarkMagenta;
    }
}