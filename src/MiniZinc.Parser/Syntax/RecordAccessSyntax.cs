namespace MiniZinc.Parser.Ast;

public sealed record RecordAccessSyntax(SyntaxNode Expr, Token Field) : SyntaxNode(Expr.Start) { }
