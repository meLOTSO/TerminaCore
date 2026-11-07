using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Factories;

namespace Game.Entities.Civilizations.CivilizationModels;

public class VampyrCivilizationModel : CivilizationModelBase
{
    public VampyrCivilizationModel() : base(new VampyrCivilizationFactory())
    {
    }
}
