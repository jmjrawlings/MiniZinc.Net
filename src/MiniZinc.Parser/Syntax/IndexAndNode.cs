namespace MiniZinc.Parser.Syntax;

public sealed class IndexAndNode : ExpressionSyntax
{
    public readonly SyntaxNode Index;
    public readonly SyntaxNode Value;

    public IndexAndNode(SyntaxNode index, SyntaxNode value)
        : base(index.Start)
    {
        Index = index;
        Value = value;
    }
}
