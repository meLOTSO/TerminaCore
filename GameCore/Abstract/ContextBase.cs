using GameCore;

public abstract class ContextBase
{
    protected readonly GameApplication GameApplication;

    public IServiceCollection Services => GameApplication.Services;

    public ContextBase(GameApplication gameApplication)
    {
        GameApplication = gameApplication;
    }
}