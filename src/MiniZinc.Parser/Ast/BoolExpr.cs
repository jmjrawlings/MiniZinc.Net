namespace MiniZinc.Parser.Ast;

public sealed record BoolExpr(bool Value) : Expr
{
    public static implicit operator bool(BoolExpr expr) => expr.Value;

    public override string ToString() => Value.ToString();
}
