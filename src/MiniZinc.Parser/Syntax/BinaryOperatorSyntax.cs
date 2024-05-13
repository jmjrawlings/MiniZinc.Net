namespace MiniZinc.Parser.Ast;

public sealed record BinaryOperatorSyntax
    (SyntaxNode Left, Token Infix, Operator? Operator, SyntaxNode Right) : SyntaxNode(Left.Start)
{
}

