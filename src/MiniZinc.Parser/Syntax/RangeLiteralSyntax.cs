namespace MiniZinc.Parser.Syntax;

public sealed class RangeLiteralSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax? Lower;
    public readonly bool LowerIncusive;
    public readonly ExpressionSyntax? Upper;
    public readonly bool UpperInclusive;

    public RangeLiteralSyntax(
        Token start,
        ExpressionSyntax? lower = null,
        bool lowerInclusive = true,
        ExpressionSyntax? upper = null,
        bool upperInclusive = true
    )
        : base(start)
    {
        Lower = lower;
        LowerIncusive = lowerInclusive;
        Upper = upper;
        UpperInclusive = upperInclusive;
    }
}
