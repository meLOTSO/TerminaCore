using Game.Entities.Abstract;
using Game.Entities.Civilizations.Abstract;

namespace Game.Entities.Civilizations;

public class CivilizationsInterface
{
    private int _activeCivilizationIndex = 0;
    private List<CivilizationModelBase> _civilizationModels;

    public event Action<CivilizationModelBase, KingHeroBase>? OnKingDie;
    public event Action<CivilizationModelBase, EntityBase>? OnEntityDie;

    public IReadOnlyList<CivilizationModelBase> CivilizationModels => _civilizationModels;
    public CivilizationModelBase ActiveCivilization { get; protected set; }

    public CivilizationsInterface(List<CivilizationModelBase> civilizationModels)
    {
        if (civilizationModels.Count < 1)
            throw new ArgumentException("Число цивилизаций должно быть не меньше единицы.");

        _civilizationModels = new List<CivilizationModelBase>(civilizationModels);
        this.ActiveCivilization = CivilizationModels[_activeCivilizationIndex];

        foreach (var civilizationModel in _civilizationModels)
        {
            civilizationModel.OnKingDie += OnKingTargetInvoke;
            civilizationModel.OnEntityDie += OnEntityDieTargetInvoke;
        }
        OnKingDie += OnKingDieHandler;
    }

    public CivilizationModelBase NextCivilization()
    {
        _activeCivilizationIndex = (_activeCivilizationIndex + 1) % CivilizationModels.Count;
        ActiveCivilization = CivilizationModels[_activeCivilizationIndex];
        return ActiveCivilization;
    }

    public CivilizationModelBase PrevCivilization()
    {
        _activeCivilizationIndex = _activeCivilizationIndex == 0 ? CivilizationModels.Count - 1 : _activeCivilizationIndex - 1;
        ActiveCivilization = CivilizationModels[_activeCivilizationIndex];
        return ActiveCivilization;
    }

    private void OnKingTargetInvoke(CivilizationModelBase civilizationModel, KingHeroBase kingHero)
    {
        OnKingDie?.Invoke(civilizationModel, kingHero);
    }

    private void OnEntityDieTargetInvoke(CivilizationModelBase civilizationModel, EntityBase entity)
    {
        OnEntityDie?.Invoke(civilizationModel, entity);
    }

    private void OnKingDieHandler(CivilizationModelBase civilizationModel, KingHeroBase kingHero)
    {
        _civilizationModels.Remove(civilizationModel);
        civilizationModel.OnKingDie -= OnKingTargetInvoke;
    }
}