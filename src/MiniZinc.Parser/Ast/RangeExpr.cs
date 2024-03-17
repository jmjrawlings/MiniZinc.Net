namespace MiniZinc.Parser.Ast;

public readonly record struct RangeExpr : IExpr
{
    public RangeExpr(IExpr? lower = null, IExpr? upper = null)
    {
        Lower = lower;
        Upper = upper;
    }

    public IExpr? Lower { get; }
    public IExpr? Upper { get; }
}
