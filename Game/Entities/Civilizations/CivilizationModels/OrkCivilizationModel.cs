using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Factories;

namespace Game.Entities.Civilizations.CivilizationModels;

public class OrkCivilizationModel : CivilizationModelBase
{
    public OrkCivilizationModel() : base(new OrkCivilizationFactory())
    {
    }
}