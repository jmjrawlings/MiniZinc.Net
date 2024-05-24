namespace MiniZinc.Parser.Syntax;

public interface ILetLocal { }

public sealed record LetSyntax(in Token Start, List<ILetLocal>? Locals, SyntaxNode Body)
    : SyntaxNode(Start) { }
