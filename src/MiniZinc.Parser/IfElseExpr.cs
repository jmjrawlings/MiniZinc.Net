namespace MiniZinc.Parser.Syntax;

public sealed class IfThenSyntax : Expr
{
    public Expr If { get; private set; }

    public Expr? Then { get; private set; }

    public Expr? Else { get; set; }

    public List<(Expr elseif, Expr then)>? ElseIfs { get; set; }

    public IfThenSyntax(in Token start, Expr ifCase, Expr? thenCase)
        : base(start)
    {
        If = ifCase;
        Then = thenCase;
    }
}
