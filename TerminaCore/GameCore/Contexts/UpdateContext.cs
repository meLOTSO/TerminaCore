using TerminaCore.GameCore.Abstract;

namespace TerminaCore.GameCore.Contexts;

public class UpdateContext : ContextBase
{
    public float DeltaTime => GameApplication.DeltaTimeSeconds;

    public UpdateContext(GameApplication gameApplication) : base(gameApplication)
    {
    }
}