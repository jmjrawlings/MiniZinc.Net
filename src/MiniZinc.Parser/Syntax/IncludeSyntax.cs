namespace MiniZinc.Parser.Syntax;

public sealed record IncludeSyntax(in Token Start, Token Path) : SyntaxNode(Start) { }
