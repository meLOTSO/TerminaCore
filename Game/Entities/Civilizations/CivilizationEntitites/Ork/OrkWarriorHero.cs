using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.CivilizationEntities.Ork;

public class OrkWarriorHero : WarriorHeroBase, ICivilizationEntity
{
    public override int Cost => 5;

    public override int AttackPlus => -1;

    public override bool[,] StepAreaPart => Area.GetR2();

    public override bool[,] DamageAreaPart => Area.GetR2();

    public override int Damage => 5;

    public override string Name => "Орг-громила";

    public CivilizationsType Civilization => CivilizationsType.Ork;

    public OrkWarriorHero() : base(32, 11)
    {

    }
}