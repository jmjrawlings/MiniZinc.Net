namespace MiniZinc.Parser.Syntax;

public sealed record StringLiteralSyntax(in Token Start)
    : ExpressionSyntax<string>(Start, Start.StringValue)
{
    public static implicit operator string(StringLiteralSyntax expr) => expr.Value;
}
