using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Elf;

public class ElfWarriorHero : WarriorHeroBase
{
    public override int AttackPlus => 1;

    public override bool[,] StepAreaPart => new bool[,]
    {
        {false, false, true, true, true},
        {false, true, true, true, true},
        {true, true, true, true, true},
    };

    public override bool[,] DamageAreaPart => new bool[,]
    {
        {false, true, true},
        {true, true, true},
    };

    public override int Damage => 2;

    public override string Name => "Эльфийский танцор клинка";

    public override int Cost => 6;

    public ElfWarriorHero() : base(14, 14)
    {
        this.Texture.ForegroundColor = ConsoleColor.DarkMagenta;
    }
}