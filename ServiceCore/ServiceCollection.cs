
namespace ServiceCore;

public class ServiceCollection : Abstract.IServiceCollection
{
    protected Dictionary<Type, ServiceBag> Services = new();

    public void AddService<TService>(Enums.ServiceRuntime runtime, Func<object> factory)
    {
        if (runtime == ServiceCore.Enums.ServiceRuntime.Singleton) AddSingleton<TService>(factory);
        else AddTransient<TService>(factory);
    }

    public void AddSingleton<TService>(Func<object> factory)
    {
        var lazy = new Lazy<object>(factory);
        Services[typeof(TService)] = new ServiceBag(Enums.ServiceRuntime.Singleton, () => lazy.Value);
    }
    public void AddSingleton<TService>(object service)
    {
        Services[typeof(TService)] = new ServiceBag(Enums.ServiceRuntime.Singleton, () => service);
    }

    public void AddTransient<TService>(Func<object> factory)
    {
        Services[typeof(TService)] = new ServiceBag(Enums.ServiceRuntime.Transient, factory);
    }

    public Abstract.IServiceProvider BuildServiceProvider() => new ServiceProvider(Services);
}