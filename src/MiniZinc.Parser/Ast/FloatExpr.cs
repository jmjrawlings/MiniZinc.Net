namespace MiniZinc.Parser.Ast;

public readonly record struct FloatExpr(float Value) : IExpr
{
    public static implicit operator float(FloatExpr expr) => expr.Value;
}
