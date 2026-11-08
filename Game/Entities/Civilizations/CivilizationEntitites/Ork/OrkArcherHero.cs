using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Ork;

public class OrkArcherHero : ArcherHeroBase
{
    public override int Cost => 6;

    public override int AttackPlus => -1;

    public override bool[,] StepAreaPart => Area.GetR2();

    public override bool[,] DamageAreaPart => Area.GetR4();

    public override int Damage => 6;

    public override string Name => "Метатель топоров";

    public OrkArcherHero() : base(20, 10)
    {

    }
}