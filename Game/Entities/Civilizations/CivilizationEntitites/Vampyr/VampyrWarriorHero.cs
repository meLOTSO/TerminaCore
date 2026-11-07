using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Vampyr;

public class VampyrWarriorHero : WarriorHeroBase, ICivilizationEntity
{
    public override int Cost => 6;

    public override int AttackPlus => 3;

    public override bool[,] StepAreaPart => Area.GetR4();

    public override bool[,] DamageAreaPart => Area.GetR2();

    public override int Damage => 3;

    public override string Name => "Вампир-страж";

    public CivilizationsType Civilization => CivilizationsType.Vampyr;

    public VampyrWarriorHero() : base(14, 15)
    {

    }
}