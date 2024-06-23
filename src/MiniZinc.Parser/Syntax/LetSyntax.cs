namespace MiniZinc.Parser.Syntax;

public sealed record LetSyntax(in Token Start, List<ILetLocalSyntax>? Locals, SyntaxNode Body)
    : SyntaxNode(Start) { }
