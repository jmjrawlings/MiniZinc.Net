namespace MiniZinc.Parser.Ast;

public sealed record OutputStatement : Node
{
    public Node Expr { get; set; }
}

public record Node : IAnnotations
{
    public List<Expr>? Annotations { get; private set; }

    public void Annotate(Expr ann)
    {
        Annotations ??= new List<Expr>();
        Annotations.Add(ann);
    }
}
