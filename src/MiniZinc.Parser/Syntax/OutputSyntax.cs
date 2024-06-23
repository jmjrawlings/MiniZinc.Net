namespace MiniZinc.Parser.Syntax;

public sealed record OutputSyntax(in Token Start, SyntaxNode Expr) : StatementSyntax(Start) { }
