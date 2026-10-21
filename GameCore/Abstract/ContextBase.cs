namespace GameCore.Abstract;

public abstract class ContextBase
{
    protected readonly GameApplication GameApplication;

    public ServiceCore.Abstract.IServiceProvider Services => GameApplication.Services;

    public ContextBase(GameApplication gameApplication)
    {
        GameApplication = gameApplication;
    }
}