namespace MiniZinc.Parser.Syntax;

public sealed record RecordAccessSyntax(SyntaxNode Expr, Token Field) : SyntaxNode(Expr.Start) { }
