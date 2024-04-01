namespace MiniZinc.Parser.Ast;

public readonly struct TupleAccess(IExpr expr, int field) : IExpr
{
    public IExpr Expr => expr;
    public int Field => field;
}
