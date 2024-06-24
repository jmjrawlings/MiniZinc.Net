namespace MiniZinc.Parser.Syntax;

public sealed record BinaryOperatorSyntax(
    ExpressionSyntax Left,
    in Token Infix,
    Operator? Operator,
    ExpressionSyntax Right
) : ExpressionSyntax(Left.Start) { }
