namespace MiniZinc.Parser.Ast;

public sealed record RangeExpr(Expr? Lower = null, Expr? Upper = null) : Expr
{
    public override string ToString() => $"{Lower}..{Upper}";

    public List<Expr>? Annotations => null;

    public void SetAnnotations(List<Expr> anns) => throw new NotImplementedException();
}
