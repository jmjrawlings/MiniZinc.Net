namespace MiniZinc.Parser.Syntax;

public sealed record WildCardSyntax(in Token Start) : SyntaxNode(Start)
{
    public override string ToString() => "_";
}
