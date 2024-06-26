namespace MiniZinc.Parser.Syntax;

public sealed class TupleLiteralSyntax : ExpressionSyntax
{
    public readonly List<SyntaxNode> Fields;

    public TupleLiteralSyntax(in Token start)
        : base(start)
    {
        Fields = new List<SyntaxNode>();
    }
}
