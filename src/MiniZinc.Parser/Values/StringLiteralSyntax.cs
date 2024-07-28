namespace MiniZinc.Parser;

public sealed class StringLiteralSyntax : ValueSyntax<string>
{
    public StringLiteralSyntax(in Token start)
        : base(start, start.StringValue) { }

    public static implicit operator string(StringLiteralSyntax expr) => expr.Value;
}
