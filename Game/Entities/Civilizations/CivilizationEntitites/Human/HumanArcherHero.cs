using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Human;

public class HumanArcherHero : ArcherHeroBase
{
    public override int AttackPlus => 2;

    public override bool[,] StepAreaPart => Area.GetR2();

    public override bool[,] DamageAreaPart => Area.GetR6();

    public override int Damage => 4;

    public override string Name => "Арбалетчик";

    public override Texture Texture => new Texture("D", ConsoleColor.DarkBlue);

    public override int Cost => 7;

    public HumanArcherHero() : base(12, 11)
    {
    }
}