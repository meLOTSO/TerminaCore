public class ServiceProvider : ServiceCore.Abstract.IServiceProvider
{
    private readonly Dictionary<Type, ServiceBag> _services;

    public ServiceProvider(Dictionary<Type, ServiceBag> services)
    {
        _services = new Dictionary<Type, ServiceBag>(services);
    }
    public ServiceProvider()
    {
        _services = new Dictionary<Type, ServiceBag>();
    }

    public TService? GetService<TService>()
    {
        if (_services.TryGetValue(typeof(TService), out ServiceBag bag)
            && bag.Factory.Invoke() is TService service)
        {
            return service;
        }
        return default;
    }

    public TService GetServiceRequired<TService>()
    {
        if (_services.TryGetValue(typeof(TService), out ServiceBag bag)
            && bag.Factory.Invoke() is TService service)
        {
            return service;
        }
        throw new Exception($"Сервис '{typeof(TService).Name}' не найден.");
    }
}
