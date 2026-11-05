namespace TerminaCore.ServiceCore.Abstract;

public interface IServiceCollection
{
    public void AddSingleton<TService>(Func<Abstract.IServiceProvider, object> factory);
    public void AddSingleton<TService>(Func<object> factory);
    public void AddTransient<TService>(Func<Abstract.IServiceProvider, object> factory);
    public void AddTransient<TService>(Func<object> factory);

    public void AddService<TService>(Enums.ServiceLifetime runtime, Func<Abstract.IServiceProvider, object> factory);
    public void AddService<TService>(Enums.ServiceLifetime runtime, Func<object> factory);

    public Abstract.IServiceProvider BuildServiceProvider();
}