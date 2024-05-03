namespace MiniZinc.Parser.Ast;

public sealed record TupleExpr : Expr
{
    public List<SyntaxNode> Fields { get; set; } = new();
}
