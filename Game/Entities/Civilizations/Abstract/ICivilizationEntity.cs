using Game.Entities.Civilizations.Enums;

namespace Game.Entities.Civilizations.Abstract;

public interface ICivilizationEntity
{
    public CivilizationsType Civilization { get; }
}