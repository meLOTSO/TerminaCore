using GameCore.Abstract;
using GameCore.Contexts;

public interface IInitializableEnvironment : IGameEnvironment
{
    public void Initialize(GameContext gameContext);
}