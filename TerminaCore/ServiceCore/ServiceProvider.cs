namespace TerminaCore.ServiceCore;

public class ServiceProvider : Abstract.IServiceProvider
{
    private readonly HashSet<ServiceDescriptor> _serviceDescriptors;
    private Dictionary<Type, object> _cachedSingletons = new();

    public ServiceProvider(HashSet<ServiceDescriptor> serviceDescriptors)
    {
        _serviceDescriptors = new HashSet<ServiceDescriptor>(serviceDescriptors);
    }
    public ServiceProvider()
    {
        _serviceDescriptors = new HashSet<ServiceDescriptor>();
    }

    public TService? GetService<TService>()
    {
        if (!(_serviceDescriptors.FirstOrDefault(sd => sd.ServiceType == typeof(TService)) is ServiceDescriptor serviceDescriptor))
            return default;

        if (serviceDescriptor.Lifetime == Enums.ServiceLifetime.Singleton)
        {
            if (_cachedSingletons.TryGetValue(typeof(TService), out object? value)) return (TService)value;
            var singletonService = serviceDescriptor.Factory.Invoke(this);
            _cachedSingletons.Add(typeof(TService), singletonService);
            return (TService)singletonService;
        }
        return (TService)serviceDescriptor.Factory.Invoke(this);
    }

    public TService GetServiceRequired<TService>()
    {
        if (!(_serviceDescriptors.FirstOrDefault(sd => sd.ServiceType == typeof(TService)) is ServiceDescriptor serviceDescriptor))
            throw new Exception($"Сервис '{typeof(TService).Name}' не найден.");

        if (serviceDescriptor.Lifetime == Enums.ServiceLifetime.Singleton)
        {
            if (_cachedSingletons.TryGetValue(typeof(TService), out object? value)) return (TService)value;
            var singletonService = serviceDescriptor.Factory.Invoke(this);
            _cachedSingletons.Add(typeof(TService), singletonService);
            return (TService)singletonService;
        }
        return (TService)serviceDescriptor.Factory.Invoke(this);
    }
}
