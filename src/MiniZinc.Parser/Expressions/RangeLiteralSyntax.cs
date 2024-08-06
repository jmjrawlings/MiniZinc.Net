namespace MiniZinc.Parser.Syntax;

public sealed class RangeLiteralSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax? Lower;
    public readonly bool LowerIncusive;
    public readonly ExpressionSyntax? Upper;
    public readonly bool UpperInclusive;
    public readonly TokenKind Operator;

    public RangeLiteralSyntax(
        Token start,
        TokenKind op,
        ExpressionSyntax? lower = null,
        ExpressionSyntax? upper = null
    )
        : base(start)
    {
        Operator = op;
        Lower = lower;
        LowerIncusive = op is TokenKind.CLOSED_RANGE or TokenKind.RIGHT_OPEN_RANGE;
        Upper = upper;
        UpperInclusive = op is TokenKind.CLOSED_RANGE or TokenKind.LEFT_OPEN_RANGE;
    }
}
