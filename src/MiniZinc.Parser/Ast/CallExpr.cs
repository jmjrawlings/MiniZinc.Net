namespace MiniZinc.Parser.Ast;

public sealed record CallExpr : Expr, INamed
{
    public string Name { get; set; }
    public List<Node>? Args { get; set; }
}
