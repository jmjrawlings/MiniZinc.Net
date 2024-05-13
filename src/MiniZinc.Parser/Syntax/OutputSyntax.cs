namespace MiniZinc.Parser.Ast;

public sealed record OutputSyntax(Token Start, SyntaxNode Expr) : SyntaxNode(Start) { }
