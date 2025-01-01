namespace MiniZinc.Parser.Syntax;

using static TokenKind;

public sealed class RangeExpr : Expr
{
    public readonly Expr? Lower;
    public readonly bool LowerIncusive;
    public readonly Expr? Upper;
    public readonly bool UpperInclusive;
    public readonly TokenKind Operator;

    public RangeExpr(Token start, TokenKind op, Expr? lower = null, Expr? upper = null)
        : base(start)
    {
        Operator = op;
        Lower = lower;
        LowerIncusive = op is TOKEN_CLOSED_RANGE or TOKEN_RIGHT_OPEN_RANGE;
        Upper = upper;
        UpperInclusive = op is TOKEN_CLOSED_RANGE or TOKEN_LEFT_OPEN_RANGE;
    }
}
