namespace MiniZinc.Parser.Ast;

public sealed record UnaryOpExpr : IExpr
{
    public Operator UnOp { get; set; }
    public IExpr Expr { get; set; }
}