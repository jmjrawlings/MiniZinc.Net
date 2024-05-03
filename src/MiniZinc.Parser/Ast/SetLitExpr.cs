namespace MiniZinc.Parser.Ast;

public sealed record SetLitExpr : Expr
{
    public List<Expr> Elements { get; set; } = new();
}
