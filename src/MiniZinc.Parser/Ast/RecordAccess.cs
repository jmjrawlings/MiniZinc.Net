namespace MiniZinc.Parser.Ast;

public readonly struct RecordAccess(IExpr expr, string field) : IExpr
{
    public IExpr Expr => expr;
    public string Field => field;
}
