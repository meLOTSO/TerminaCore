using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;
using TerminaCore.Graphics;

namespace Game.Entities.Civilizations.CivilizationEntities.Human;

public class HumanKingHero : KingHeroBase, ICivilizationEntity
{
    public override int AttackPlus => 2;

    public override bool[,] StepAreaPart => Area.GetR3();

    public override bool[,] DamageAreaPart => Area.GetR2();
    public override int Damage => 4;

    public override string Name => "Верховный Король";

    public override Texture Texture => new Texture("^", ConsoleColor.DarkBlue);

    public CivilizationsType Civilization => CivilizationsType.Human;

    public override int SummarizeValue => 2;

    public HumanKingHero() : base(40, 14)
    {
    }
}