namespace MiniZinc.Parser.Ast;

public interface ILetLocal { }

public sealed record LetSyntax(Token Start) : SyntaxNode(Start)
{
    public List<ILetLocal>? Locals { get; set; }

    public SyntaxNode Body { get; set; }
}
