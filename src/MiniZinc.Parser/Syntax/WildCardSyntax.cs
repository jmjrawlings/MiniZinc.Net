namespace MiniZinc.Parser.Syntax;

public sealed class WildCardSyntax : ExpressionSyntax
{
    public WildCardSyntax(in Token start)
        : base(start) { }

    public override string ToString() => "_";
}
