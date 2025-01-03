namespace MiniZinc.Parser;

using static TokenKind;

public sealed class RangeExpr : MiniZincExpr
{
    public readonly MiniZincExpr? Lower;
    public readonly bool LowerIncusive;
    public readonly MiniZincExpr? Upper;
    public readonly bool UpperInclusive;
    public readonly TokenKind Operator;

    public RangeExpr(
        Token start,
        TokenKind op,
        MiniZincExpr? lower = null,
        MiniZincExpr? upper = null
    )
        : base(start)
    {
        Operator = op;
        Lower = lower;
        LowerIncusive = op is TOKEN_RANGE_INCLUSIVE or TOKEN_RANGE_RIGHT_EXCLUSIVE;
        Upper = upper;
        UpperInclusive = op is TOKEN_RANGE_INCLUSIVE or TOKEN_RANGE_LEFT_EXCLUSIVE;
    }
}
