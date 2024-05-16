namespace MiniZinc.Parser.Syntax;

public sealed record RangeLiteralSyntax(
    Token Start,
    SyntaxNode? Lower = null,
    SyntaxNode? Upper = null
) : SyntaxNode(Start)
{
    public override string ToString() => $"{Lower}..{Upper}";
}
