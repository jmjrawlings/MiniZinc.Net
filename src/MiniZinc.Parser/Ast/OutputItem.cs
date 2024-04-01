namespace MiniZinc.Parser.Ast;

public sealed record OutputItem : Item
{
    public INode Expr { get; set; }
}

public record Item : INode, IAnnotations
{
    private List<IExpr>? _annotations;
    public IEnumerable<IExpr> Annotations => _annotations;

    public Item()
    {
        _annotations = null;
    }

    public void Annotate(IExpr expr)
    {
        _annotations ??= new List<IExpr>();
        _annotations.Add(expr);
    }
}
