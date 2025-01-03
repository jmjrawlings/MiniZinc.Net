namespace MiniZinc.Parser;

public sealed class IfThenSyntax : MiniZincExpr
{
    public MiniZincExpr If { get; private set; }

    public MiniZincExpr? Then { get; private set; }

    public MiniZincExpr? Else { get; set; }

    public List<(MiniZincExpr elseif, MiniZincExpr then)>? ElseIfs { get; set; }

    public IfThenSyntax(in Token start, MiniZincExpr ifCase, MiniZincExpr? thenCase)
        : base(start)
    {
        If = ifCase;
        Then = thenCase;
    }
}
