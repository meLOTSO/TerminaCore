using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Ork;

public class OrkKingHero : KingHeroBase
{
    public override int SummarizeValue => 1;

    public override int AttackPlus => -2;

    public override bool[,] StepAreaPart => Area.GetR2();

    public override bool[,] DamageAreaPart => Area.GetR2();

    public override int Damage => 8;

    public override string Name => "Вождь Орды";

    public OrkKingHero() : base(80, 16)
    {
    }
}