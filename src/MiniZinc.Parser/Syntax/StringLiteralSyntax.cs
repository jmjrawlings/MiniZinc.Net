namespace MiniZinc.Parser.Syntax;

public sealed class StringLiteralSyntax : ExpressionSyntax<string>
{
    public StringLiteralSyntax(in Token start)
        : base(start, start.StringValue) { }

    public static implicit operator string(StringLiteralSyntax expr) => expr.Value;
}
