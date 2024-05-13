namespace MiniZinc.Parser.Ast;

public sealed record IncludeSyntax(Token Start, Token Path) : SyntaxNode(Start) { }
