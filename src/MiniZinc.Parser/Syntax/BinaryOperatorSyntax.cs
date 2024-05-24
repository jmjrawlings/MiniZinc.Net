namespace MiniZinc.Parser.Syntax;

public sealed record BinaryOperatorSyntax(
    SyntaxNode Left,
    in Token Infix,
    Operator? Operator,
    SyntaxNode Right
) : SyntaxNode(Left.Start) { }
