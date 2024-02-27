namespace MiniZinc.Parser.Ast;

public sealed record FloatLit(double Value) : Expr
{
    public static implicit operator double(FloatLit expr) => expr.Value;

    public override string ToString() => Value.ToString("g");
}
