namespace MiniZinc.Parser.Syntax;

public sealed record WildCardSyntax(in Token Start) : ExpressionSyntax(Start)
{
    public override string ToString() => "_";
}
