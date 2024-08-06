namespace MiniZinc.Parser;

using Syntax;

public sealed class IntLiteralSyntax : ExpressionSyntax
{
    public int Value { get; }

    public IntLiteralSyntax(in Token start)
        : base(start)
    {
        Value = start.IntValue;
    }

    public IntLiteralSyntax(in Token start, int value)
        : base(start)
    {
        Value = value;
    }

    public static implicit operator int(IntLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString();
}
