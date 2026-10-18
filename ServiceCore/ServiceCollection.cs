using ServiceCore.Abstract;

namespace ServiceCore;

public class ServiceCollection : IServiceCollection, IServiceCollectionBuilder
{
    protected readonly Dictionary<Type, object> SingletonServices = new();
    protected readonly Dictionary<Type, Func<object>> TransientServices = new();

    public void AddSingleton<TService>(Func<object> func)
    {
        var serviceObj = func.Invoke();
        SingletonServices.Add(typeof(TService), serviceObj!);
    }
    public void AddSingleton<TService>(object serviceObj)
    {
        SingletonServices.Add(typeof(TService), serviceObj);
    }

    public void AddTransient<TService>(Func<object> func)
    {
        TransientServices.Add(typeof(TService), func);
    }

    public TService? GetService<TService>()
    {
        if (TransientServices.TryGetValue(typeof(TService), out Func<object>? tfunc))
        {
            var tservice = tfunc.Invoke();
            return (TService)tservice;
        }
        else if (SingletonServices.TryGetValue(typeof(TService), out object? svalue))
        {
            return (TService)svalue;
        }
        return default;
    }
    public TService GetServiceRequired<TService>()
    {
        var service = GetService<TService>();
        if (service is null)
            throw new ArgumentException($"Сервис '{typeof(TService).Name}' не был найден.");
        return service;
    }
}