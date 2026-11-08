using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Vampyr;

public class VampyrArcherHero : ArcherHeroBase
{
    public override int Cost => 8;

    public override int AttackPlus => 4;

    public override bool[,] StepAreaPart => Area.GetR3();

    public override bool[,] DamageAreaPart => Area.GetR5();

    public override int Damage => 4;

    public override string Name => "Теневой маг";

    public VampyrArcherHero() : base(10, 13)
    {

    }
}