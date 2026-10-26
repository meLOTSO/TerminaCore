namespace TerminaCore.Graphics.DOM;

public class ModelTree
{
    private readonly Canvas _canvas;
    public ModelNode Body { get; set; }

    public ModelTree(Canvas canvas, string? id = null, string? className = null)
    {
        _canvas = canvas;
        Body = new ModelNode(_canvas.RenderTexture, id: id, className: className);
    }

    public ModelNode? GetNodeById(string id) => Body.FirstChild((node) => node.Id == id);
    public static ModelNode? GetNodeById(string id, ModelNode node) => ModelNode.FirstChild((node) => node.Id == id, node);

    public List<ModelNode> Where(Predicate<ModelNode> predicate) => Body.ChildsWhere(predicate);
    public static List<ModelNode> Where(Predicate<ModelNode> predicate, ModelNode node) => ModelNode.ChildsWhere(predicate, node);

    public ModelNode? First(Predicate<ModelNode> predicate) => Body.FirstChild(predicate);
    public static ModelNode? First(Predicate<ModelNode> predicate, ModelNode node) => ModelNode.FirstChild(predicate, node);

    public bool TryFirst(Predicate<ModelNode> predicate, out ModelNode? outNode) => Body.TryFirstChild(predicate, out outNode);
    public static bool TryFirst(Predicate<ModelNode> predicate, ModelNode node, out ModelNode? outNode) => ModelNode.TryFirstChild(predicate, node, out outNode);

    public List<T> Select<T>(Func<ModelNode, T> func) => Body.SelectChilds<T>(func);
    public static List<T> Select<T>(Func<ModelNode, T> func, ModelNode node) => ModelNode.SelectChilds<T>(func, node);
}