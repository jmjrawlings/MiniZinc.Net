namespace MiniZinc.Parser.Ast;

public readonly record struct FloatLit(double Value) : IExpr
{
    public static implicit operator double(FloatLit expr) => expr.Value;
}
