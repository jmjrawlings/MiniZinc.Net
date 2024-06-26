namespace MiniZinc.Parser.Syntax;

public sealed class SetLiteralSyntax : ExpressionSyntax
{
    public SetLiteralSyntax(in Token start)
        : base(start) { }

    public List<SyntaxNode> Elements { get; } = new();
}
