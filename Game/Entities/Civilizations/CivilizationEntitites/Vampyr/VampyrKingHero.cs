using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Vampyr;

public class VampyrKingHero : KingHeroBase
{
    public override int SummarizeValue => 1;

    public override int AttackPlus => 4;

    public override bool[,] StepAreaPart => Area.GetR4();

    public override bool[,] DamageAreaPart => Area.GetR2();

    public override int Damage => 3;

    public override string Name => "Лорд Ночи";

    public VampyrKingHero() : base(30, 17)
    {
    }
}