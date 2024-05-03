namespace MiniZinc.Parser.Ast;

public record SyntaxNode
{
    public List<Expr>? Annotations { get; private set; }

    public void Annotate(Expr ann)
    {
        Annotations ??= new List<Expr>();
        Annotations.Add(ann);
    }
}
