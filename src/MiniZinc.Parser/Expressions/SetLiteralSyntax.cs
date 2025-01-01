namespace MiniZinc.Parser.Syntax;

public sealed class SetLiteralSyntax : ExpressionSyntax
{
    public IReadOnlyList<ExpressionSyntax>? Elements { get; }

    public SetLiteralSyntax(in Token start, IReadOnlyList<ExpressionSyntax>? elements = null)
        : base(start)
    {
        Elements = elements;
    }
}
