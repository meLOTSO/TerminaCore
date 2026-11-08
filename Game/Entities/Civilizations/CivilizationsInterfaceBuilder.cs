using Game.Entities.Civilizations.Abstract;
using Game.Entities.Civilizations.CivilizationModels;
using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations;

public class CivilizationsInterfaceBuilder
{
    protected List<Func<CivilizationModelBase>> CivilizationModelCreators = [];

    public void AddCivilization(Func<CivilizationModelBase> civilizationModelCreator)
    {
        CivilizationModelCreators.Add(civilizationModelCreator);
    }

    public CivilizationsInterface Build()
    {
        int len = Enum.GetValues<CivilizationType>().Length;
        bool[] added = new bool[len];
        List<CivilizationModelBase> civilizationModels = [];
        CivilizationModelBase civilizationModel;
        int civTypeIndex;
        foreach (var creator in CivilizationModelCreators)
        {
            civilizationModel = creator.Invoke();
            civTypeIndex = (int)civilizationModel.CivilizationType;
            if (!added[civTypeIndex])
            {
                civilizationModels.Add(civilizationModel);
                added[civTypeIndex] = true;
            }
        }
        return new CivilizationsInterface(civilizationModels);
    }
}

