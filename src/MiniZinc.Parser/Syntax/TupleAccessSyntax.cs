namespace MiniZinc.Parser.Ast;

public sealed record TupleAccessSyntax(SyntaxNode Expr, Token Field) : SyntaxNode(Expr.Start) { }
