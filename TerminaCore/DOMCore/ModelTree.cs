using TerminaCore.Graphics;

namespace TerminaCore.DOMCore;

public class ModelTree
{
    private readonly Canvas _canvas;
    public ModelNode Body { get; set; }

    public ModelTree(Canvas canvas, string? className = null)
    {
        _canvas = canvas;
        Body = new ModelNode(_canvas.RenderTexture, id: "body", className: className);
    }

    public ModelNode? GetNodeById(string id) => First((node) => node.Id == id);
    public static ModelNode? GetNodeById(string id, ModelTree tree) => ModelTree.First((node) => node.Id == id, tree.Body);
    public List<ModelNode> GetNodeByClass(string className) => Where((node) => node.ClassName == className);
    public static List<ModelNode> GetNodeByClass(string className, ModelTree tree) => ModelTree.Where((node) => node.ClassName == className, tree.Body);

    public List<ModelNode> Where(Predicate<ModelNode> predicate) => Body.ChildsWhere(predicate);
    public static List<ModelNode> Where(Predicate<ModelNode> predicate, ModelNode node) => ModelNode.ChildsWhere(predicate, node);

    public ModelNode? First(Predicate<ModelNode> predicate) => Body.FirstChild(predicate);
    public static ModelNode? First(Predicate<ModelNode> predicate, ModelNode node) => ModelNode.FirstChild(predicate, node);

    public bool TryFirst(Predicate<ModelNode> predicate, out ModelNode? outNode) => Body.TryFirstChild(predicate, out outNode);
    public static bool TryFirst(Predicate<ModelNode> predicate, ModelNode node, out ModelNode? outNode) => ModelNode.TryFirstChild(predicate, node, out outNode);

    public List<T> Select<T>(Func<ModelNode, T> func) => Body.SelectChilds<T>(func);
    public static List<T> Select<T>(Func<ModelNode, T> func, ModelNode node) => ModelNode.SelectChilds<T>(func, node);

    public void Update()
    {
        Body.UpdateStyleBFS();
    }
}