namespace MiniZinc.Parser.Syntax;

public sealed record OutputSyntax(Token Start, SyntaxNode Expr) : SyntaxNode(Start) { }
