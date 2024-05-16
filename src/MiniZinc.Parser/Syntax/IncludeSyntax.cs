namespace MiniZinc.Parser.Syntax;

public sealed record IncludeSyntax(Token Start, Token Path) : SyntaxNode(Start) { }
