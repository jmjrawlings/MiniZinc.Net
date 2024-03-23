namespace MiniZinc.Parser.Ast;

public record Expr : IAnnotations
{
    public bool IsBracketed { get; set; }

    public static Assignment Assign(string name, INode body) => new Assignment(name, body);

    public static FloatLit Float(double f) => new FloatLit(f);

    public static IntLit Int(int i) => new IntLit(i);

    public static BoolLit Bool(bool b) => new BoolLit(b);

    public static StringLit String(string s) => new StringLit(s);

    public static INode Wildcard => new WildCardExpr();
    public static INode Empty => new EmptyExpr();

    public static INode Null => new NullExpr();

    public static Identifier Ident(string name) => new Identifier(name);

    public List<INode>? Annotations { get; set; }
}
