using TerminaCore.ServiceCore.Enums;

namespace TerminaCore.ServiceCore;

public class ServiceCollection : Abstract.IServiceCollection
{
    protected HashSet<ServiceDescriptor> ServiceDescriptors = new();

    public void AddService<TService>(Enums.ServiceLifetime runtime, Func<Abstract.IServiceProvider, object> factory)
    {
        if (runtime == Enums.ServiceLifetime.Singleton) AddSingleton<TService>(factory);
        else AddTransient<TService>(factory);
    }

    public void AddService<TService>(ServiceLifetime runtime, Func<object> factory)
    {
        if (runtime == ServiceLifetime.Singleton) AddSingleton<TService>(factory);
        else AddTransient<TService>(factory);
    }

    public void AddSingleton<TService>(Func<Abstract.IServiceProvider, object> factory)
    {
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), Enums.ServiceLifetime.Singleton, factory);
        ServiceDescriptors.Add(serviceDescriptor);
    }

    public void AddSingleton<TService>(Func<object> factory)
    {
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), Enums.ServiceLifetime.Singleton, (services) => factory.Invoke());
        ServiceDescriptors.Add(serviceDescriptor);
    }

    public void AddTransient<TService>(Func<Abstract.IServiceProvider, object> factory)
    {
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), Enums.ServiceLifetime.Transient, factory);
        ServiceDescriptors.Add(serviceDescriptor);
    }

    public void AddTransient<TService>(Func<object> factory)
    {
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), Enums.ServiceLifetime.Transient, (services) => factory.Invoke());
        ServiceDescriptors.Add(serviceDescriptor);
    }

    public Abstract.IServiceProvider BuildServiceProvider() => new ServiceProvider(ServiceDescriptors);
}