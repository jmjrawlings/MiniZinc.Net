namespace MiniZinc.Parser.Ast;

public interface ILetLocal { }

public sealed record LetSyntax(Token Start, List<ILetLocal>? Locals, SyntaxNode Body)
    : SyntaxNode(Start) { }
