namespace MiniZinc.Parser.Syntax;

public sealed class IfThenSyntax : ExpressionSyntax
{
    public ExpressionSyntax If { get; private set; }

    public ExpressionSyntax? Then { get; private set; }

    public ExpressionSyntax? Else { get; set; }

    public List<(ExpressionSyntax elseif, ExpressionSyntax then)>? ElseIfs { get; set; }

    public IfThenSyntax(in Token start, ExpressionSyntax ifCase, ExpressionSyntax? thenCase)
        : base(start)
    {
        If = ifCase;
        Then = thenCase;
    }
}
