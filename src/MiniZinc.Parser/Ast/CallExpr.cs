namespace MiniZinc.Parser.Ast;

public sealed record CallExpr : Expr, INamed
{
    public required string Name { get; init; }
    public List<Node>? Args { get; set; }
}
