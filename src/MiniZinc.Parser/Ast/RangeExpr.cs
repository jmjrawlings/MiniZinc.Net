namespace MiniZinc.Parser.Ast;

public readonly record struct RangeExpr(IExpr? Lower = null, IExpr? Upper = null) : IExpr
{
    public override string ToString() => $"{Lower}..{Upper}";
}
