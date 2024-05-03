namespace MiniZinc.Parser.Ast;

public sealed record FloatLitExpr(double Value) : Expr
{
    public static implicit operator double(FloatLitExpr expr) => expr.Value;

    public override string ToString() => Value.ToString("g");
}
