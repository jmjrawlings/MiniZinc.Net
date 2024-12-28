namespace MiniZinc.Parser.Syntax;

using static TokenKind;

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
        LowerIncusive = op is TOKEN_CLOSED_RANGE or TOKEN_RIGHT_OPEN_RANGE;
        Upper = upper;
        UpperInclusive = op is TOKEN_CLOSED_RANGE or TOKEN_LEFT_OPEN_RANGE;
    }
}
