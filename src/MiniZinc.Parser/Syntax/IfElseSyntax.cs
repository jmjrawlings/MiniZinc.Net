namespace MiniZinc.Parser.Syntax;

public sealed record IfElseSyntax(in Token Start, SyntaxNode If, SyntaxNode Then)
    : SyntaxNode(Start)
{
    public List<(SyntaxNode elseif, SyntaxNode then)>? ElseIfs { get; set; } = new();

    public SyntaxNode? Else { get; set; }
}
