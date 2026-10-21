namespace ServiceCore.Abstract;

public interface IServiceCollection
{
    public void AddSingleton<TService>(Func<object> factory);
    public void AddSingleton<TService>(object service);
    public void AddTransient<TService>(Func<object> factory);
    public void AddService<TService>(ServiceCore.Enums.ServiceRuntime runtime, Func<object> factory);

    public IServiceProvider BuildServiceProvider();
}