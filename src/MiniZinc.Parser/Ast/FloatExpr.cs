namespace MiniZinc.Parser.Ast;

public sealed record FloatExpr(double Value) : Expr
{
    public static implicit operator double(FloatExpr expr) => expr.Value;

    public override string ToString() => Value.ToString("g");
}
