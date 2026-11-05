namespace TerminaCore.ServiceCore.Abstract;

public interface IServiceProvider
{
    public TService? GetService<TService>();
    public TService GetServiceRequired<TService>();
}
