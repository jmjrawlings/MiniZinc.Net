namespace MiniZinc.Parser.Syntax;

public sealed class Array1dExpr : ArrayExpr
{
    public Array1dExpr(in Token start)
        : base(start) { }

    public bool Indexed { get; set; }
}
