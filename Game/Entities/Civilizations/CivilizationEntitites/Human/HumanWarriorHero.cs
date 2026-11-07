using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Human;

public class HumanWarriorHero : WarriorHeroBase, ICivilizationEntity
{

    public override int AttackPlus => 2;
    public override bool[,] StepAreaPart => Area.GetR3();

    public override bool[,] DamageAreaPart => Area.GetR2();

    public override int Damage => 3;

    public override string Name => "Латник";

    public override Texture Texture => new Texture("/", ConsoleColor.DarkBlue);

    public CivilizationsType Civilization => CivilizationsType.Human;

    public override int Cost => 5;

    public HumanWarriorHero() : base(18, 13)
    {
    }
}