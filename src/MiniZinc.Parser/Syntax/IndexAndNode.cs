namespace MiniZinc.Parser.Syntax;

public sealed class IndexAndNode : ExpressionSyntax
{
    public readonly ExpressionSyntax Index;
    public readonly ExpressionSyntax Value;

    public IndexAndNode(ExpressionSyntax index, ExpressionSyntax value)
        : base(index.Start)
    {
        Index = index;
        Value = value;
    }
}
