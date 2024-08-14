namespace MiniZinc.Parser;

using Syntax;

public sealed class BoolLiteralSyntax : ExpressionSyntax
{
    public bool Value { get; }

    public BoolLiteralSyntax(in Token start, bool value)
        : base(start)
    {
        Value = value;
    }

    public static implicit operator bool(in BoolLiteralSyntax literalSyntax) => literalSyntax.Value;

    public override string ToString() => Value.ToString();
}
