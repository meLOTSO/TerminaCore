using TerminaCore.GameCore.Contexts;

namespace TerminaCore.GameCore.Abstract;

public interface IInitializableEnvironment : IGameEnvironment
{
    public void Initialize(GameContext gameContext);
}