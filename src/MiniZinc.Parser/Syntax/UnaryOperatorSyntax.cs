namespace MiniZinc.Parser.Syntax;

public sealed record UnaryOperatorSyntax(Token Start, Operator Operator, SyntaxNode Expr)
    : SyntaxNode(Start) { }
