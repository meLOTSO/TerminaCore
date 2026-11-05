using TerminaCore.DOMCore;
using TerminaCore.GameCore;
using TerminaCore.GameCore.Contexts;
using TerminaCore.Graphics;
using TerminaCore.ServiceCore.Abstract;

public static class DOMServiceExtensions
{
    public static IServiceCollection AddDOMModel(this IServiceCollection services, Func<TerminaCore.ServiceCore.Abstract.IServiceProvider, Canvas> canvasFactory)
    {
        ArgumentNullException.ThrowIfNull(canvasFactory);

        services.AddSingleton<ModelTree>(services => new ModelTree(canvasFactory.Invoke(services)));
        return services;
    }

    public static GameApplication UseDOMModel(this GameApplication gameApplication)
    {
        var dom = gameApplication.Services.GetService<ModelTree>();

        if (dom is null)
            throw new Exception("Сервис 'ModelTree' не был добавлен. Воспользуйтесь AddDOMModel.");

        gameApplication.BeforeDraw += dom.Update;

        return gameApplication;
    }

    public static ModelTree GetDOM(this GameContext context)
    {
        return context.Services.GetServiceRequired<ModelTree>();
    }

    public static ModelNode? GetNodeById(this GameContext context, string id)
    {
        var dom = context.Services.GetServiceRequired<ModelTree>();
        return dom.GetNodeById(id);
    }

    public static List<ModelNode> GetNodeByClass(this GameContext context, string className)
    {
        var dom = context.Services.GetServiceRequired<ModelTree>();
        return dom.GetNodeByClass(className);
    }

    public static ModelNode? GetFirstNode(this GameContext context, Predicate<ModelNode> predicate)
    {
        var dom = context.Services.GetServiceRequired<ModelTree>();
        return dom.First(predicate);
    }

    public static bool TryGetFirstNode(this GameContext context, Predicate<ModelNode> predicate, out ModelNode? outNode)
    {
        var dom = context.Services.GetServiceRequired<ModelTree>();
        return dom.TryFirst(predicate, out outNode);
    }

    public static List<ModelNode> NodesWhere(this GameContext context, Predicate<ModelNode> predicate)
    {
        var dom = context.Services.GetServiceRequired<ModelTree>();
        return dom.Where(predicate);
    }

    public static List<T> SelectNodes<T>(this GameContext context, Func<ModelNode, T> func)
    {
        var dom = context.Services.GetServiceRequired<ModelTree>();
        return dom.Select(func);
    }
}