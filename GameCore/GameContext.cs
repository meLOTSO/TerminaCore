namespace GameCore;

public class GameContext : ContextBase
{
    public int DeltaTimeMS => GameApplication.TargetDeltaTimeMS;
    public float DeltaTimeSeconds => GameApplication.DeltaTimeSeconds;

    public GameContext(GameApplication gameApplication) : base(gameApplication)
    {
    }

    public void Stop() => GameApplication.Stop();
}