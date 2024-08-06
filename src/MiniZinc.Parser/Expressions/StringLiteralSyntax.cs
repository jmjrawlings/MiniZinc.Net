namespace MiniZinc.Parser;

using Syntax;

public sealed class StringLiteralSyntax : ExpressionSyntax
{
    public string Value { get; }

    public StringLiteralSyntax(in Token start)
        : base(start)
    {
        Value = start.StringValue;
    }

    public StringLiteralSyntax(in Token start, string value)
        : base(start)
    {
        Value = value;
    }

    public static implicit operator string(StringLiteralSyntax expr) => expr.Value;
}
