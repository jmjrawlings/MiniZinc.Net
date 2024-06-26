namespace MiniZinc.Parser.Syntax;

public sealed class RangeLiteralSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax? Lower;
    public readonly ExpressionSyntax? Upper;

    public RangeLiteralSyntax(
        Token start,
        ExpressionSyntax? lower = null,
        ExpressionSyntax? upper = null
    )
        : base(start)
    {
        Lower = lower;
        Upper = upper;
    }

    public override string ToString() => $"{Lower}..{Upper}";
}
