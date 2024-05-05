namespace MiniZinc.Parser.Ast;

public sealed record IfElseSyntax(Token Start, SyntaxNode If, SyntaxNode Then) : SyntaxNode(Start)
{
    public List<(SyntaxNode @elseif, SyntaxNode @then)>? ElseIfs { get; set; } = new();

    public SyntaxNode? Else { get; set; }
}
