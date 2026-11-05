using TerminaCore.ServiceCore.Enums;

public class ServiceDescriptor : IEquatable<ServiceDescriptor>
{
    public Type ServiceType { get; }
    public ServiceLifetime Lifetime { get; }
    public Func<TerminaCore.ServiceCore.Abstract.IServiceProvider, object> Factory { get; }

    public ServiceDescriptor(Type serviceType, ServiceLifetime lifetime, Func<TerminaCore.ServiceCore.Abstract.IServiceProvider, object> factory)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
        Factory = factory;
    }

    public bool Equals(ServiceDescriptor? other)
    {
        return ServiceType == other?.ServiceType;
    }
}