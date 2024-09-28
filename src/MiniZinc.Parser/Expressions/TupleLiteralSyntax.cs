namespace MiniZinc.Parser.Syntax;

public sealed class TupleLiteralSyntax : ExpressionSyntax
{
    public readonly List<ExpressionSyntax> Fields;

    public TupleLiteralSyntax(in Token start)
        : base(start)
    {
        Fields = [];
    }
}
