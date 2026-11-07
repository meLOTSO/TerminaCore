using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.Factories;

namespace Game.Entities.Civilizations.CivilizationModels;

public class ElfCivilizationModel : CivilizationModelBase
{
    public ElfCivilizationModel() : base(new ElfCivilizationFactory())
    {
    }
}