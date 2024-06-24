namespace MiniZinc.Parser.Syntax;

public sealed record IfThenSyntax(in Token Start, ExpressionSyntax If, ExpressionSyntax? Then)
    : ExpressionSyntax(Start)
{
    public List<(ExpressionSyntax elseif, ExpressionSyntax then)>? ElseIfs { get; set; } = new();

    public ExpressionSyntax? Else { get; set; }
}
