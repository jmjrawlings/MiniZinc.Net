namespace MiniZinc.Parser.Ast;

public sealed record CallExpr : Expr, INamed
{
    public required string Name { get; init; }
    public List<SyntaxNode>? Args { get; set; }
}
