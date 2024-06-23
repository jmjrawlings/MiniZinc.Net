namespace MiniZinc.Parser.Syntax;

public sealed record FloatLiteralSyntax : ExpressionSyntax<decimal>
{
    public FloatLiteralSyntax(in Token Start)
        : base(Start, Start.DecimalValue) { }

    public FloatLiteralSyntax(in Token Start, decimal Value)
        : base(Start, Value) { }

    public override string ToString() => Value.ToString("g");
}
