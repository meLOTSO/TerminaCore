using System;

namespace TerminaCore.ServiceCore;

public record struct ServiceBag(ServiceCore.Enums.ServiceRuntime Runtime, Func<object> Factory);