namespace MiniZinc.Parser.Ast;

public record Expr : IExpr
{
    public bool IsBracketed { get; set; }

    public static Assignment Assign(string name, INode body) => new Assignment(name, body);

    public static FloatLit Float(double f) => new FloatLit(f);

    public static IntLit Int(int i) => new IntLit(i);

    public static BoolLit Bool(bool b) => new BoolLit(b);

    public static StringLit String(string s) => new StringLit(s);

    public static IExpr Wildcard => new WildCardExpr();
    public static IExpr Empty => new EmptyExpr();

    public static IExpr Null => new NullExpr();

    public static Identifier Ident(string name) => new Identifier(name);

    public static RangeExpr Range(IExpr? lower = null, IExpr? upper = null) =>
        new RangeExpr(lower, upper);
}
