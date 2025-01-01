namespace MiniZinc.Parser.Syntax;

public sealed class FloatExpr : Expr
{
    public decimal Value { get; }

    public FloatExpr(in Token Start)
        : base(Start)
    {
        Value = Start.FloatValue;
    }

    public FloatExpr(in Token Start, decimal value)
        : base(Start)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString("g");

    public static implicit operator decimal(FloatExpr f) => f.Value;
}
