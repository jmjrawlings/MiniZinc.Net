namespace MiniZinc.Parser.Syntax;

public sealed record UnaryOperatorSyntax(in Token Start, Operator Operator, SyntaxNode Expr)
    : ExpressionSyntax(Start) { }
