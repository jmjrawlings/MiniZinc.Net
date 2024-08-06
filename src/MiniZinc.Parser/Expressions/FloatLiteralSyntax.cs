namespace MiniZinc.Parser.Syntax;

public sealed class FloatLiteralSyntax : ExpressionSyntax
{
    public decimal Value { get; }

    public FloatLiteralSyntax(in Token Start)
        : base(Start) { }

    public FloatLiteralSyntax(in Token Start, decimal value)
        : base(Start)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString("g");

    public static implicit operator decimal(FloatLiteralSyntax f) => f.Value;
}
