namespace MiniZinc.Parser.Ast;

public sealed record TupleAccessExpr : IExpr
{
    public IExpr Expr { get; set; }
    public int Index { get; set; }
}