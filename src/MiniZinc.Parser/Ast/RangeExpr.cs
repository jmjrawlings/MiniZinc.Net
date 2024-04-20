namespace MiniZinc.Parser.Ast;

public sealed record RangeExpr(Expr? Lower = null, Expr? Upper = null) : Expr
{
    public override string ToString() => $"{Lower}..{Upper}";
}
