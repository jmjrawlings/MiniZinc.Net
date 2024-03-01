namespace MiniZinc.Parser.Ast;

public readonly record struct ArrayAccessExpr : IExpr
{
    public IExpr Expr { get; }
    public List<IExpr> Slice { get; }

    public ArrayAccessExpr(IExpr expr, List<IExpr> slice)
    {
        Expr = expr;
        Slice = slice;
    }
}
