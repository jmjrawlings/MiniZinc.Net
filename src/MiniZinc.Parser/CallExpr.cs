namespace MiniZinc.Parser.Syntax;

public sealed class CallExpr : Expr, INamedSyntax
{
    public Token Name { get; }

    public IReadOnlyList<Expr>? Args { get; }

    public CallExpr(in Token name, IReadOnlyList<Expr>? args = null)
        : base(name)
    {
        Args = args;
        Name = name;
    }
}
