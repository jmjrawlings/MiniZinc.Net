namespace MiniZinc.Parser.Ast;

public sealed record UnaryOpExpr : IExpr
{
    public Operator Op { get; set; }
    public IExpr Expr { get; set; }
}
