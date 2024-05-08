namespace MiniZinc.Parser.Ast;

public sealed record UnaryOperatorSyntax(Token Start, Operator Operator, SyntaxNode Expr)
    : SyntaxNode(Start) { }
