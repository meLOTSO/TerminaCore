using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Factories;

namespace Game.Entities.Civilizations.CivilizationModels;

public class HumanCivilizationModel : CivilizationModelBase
{
    public HumanCivilizationModel() : base(new HumanCivilizationFactory())
    {
    }
}
