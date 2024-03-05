namespace MiniZinc.Parser.Ast;

public static class Expr
{
    public static FloatLit Float(double f) => new FloatLit(f);

    public static IntLit Int(int i) => new IntLit(i);

    public static BoolLit Bool(bool b) => new BoolLit(b);

    public static StringLit String(string s) => new StringLit(s);

    public static IExpr Null => new NullExpr();

    public static IExpr Bracketed(IExpr expr) => new BracketedExpr { Expr = expr };

    public static UnaryOpExpr Unary(Operator op, IExpr expr) =>
        new UnaryOpExpr { Expr = expr, Op = op };

    public static BinaryOpExpr Binary(IExpr left, Operator op, IExpr right) =>
        new BinaryOpExpr
        {
            Left = left,
            Right = right,
            Op = op
        };

    public static RangeExpr Range(IExpr? lower = null, IExpr? upper = null) =>
        new RangeExpr { Lower = lower ?? Null, Upper = upper ?? Null };

    public static T? CheckNull<T>(this T item)
        where T : IExpr
    {
        return item switch
        {
            NullExpr => default,
            _ => item
        };
    }
}
