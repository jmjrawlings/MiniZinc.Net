namespace MiniZinc.Parser.Ast;

public record Expr
{
    public static FloatLit Float(double f) => new FloatLit(f);

    public static IntLit Int(int i) => new IntLit(i);

    public static BoolLit Bool(bool b) => new BoolLit(b);

    public static StringLit String(string s) => new StringLit(s);

    public static IExpr Wildcard => new WildCardExpr();

    public static IExpr Null => new NullExpr();

    public static IExpr Bracketed(IExpr expr) => new BracketedExpr { Expr = expr };

    public static IExpr Ident(string name) => new Identifer(name);
}
