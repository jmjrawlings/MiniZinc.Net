namespace MiniZinc.Parser.Syntax;

public interface ILetLocalSyntax { }

public sealed record LetSyntax(in Token Start, List<ILetLocalSyntax>? Locals, SyntaxNode Body)
    : SyntaxNode(Start) { }
