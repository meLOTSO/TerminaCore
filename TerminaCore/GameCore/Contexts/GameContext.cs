using TerminaCore.EntityCore.Abstract;

namespace TerminaCore.GameCore.Contexts;

public class GameContext : GameCore.Abstract.ContextBase
{
    public int DeltaTimeMS => GameApplication.TargetDeltaTimeMS;
    public float DeltaTimeSeconds => GameApplication.DeltaTimeSeconds;
    public IEntityProvider Entities => GameApplication.Entities;

    public GameContext(GameApplication gameApplication) : base(gameApplication)
    {
    }

    public void Stop() => GameApplication.Stop();
}