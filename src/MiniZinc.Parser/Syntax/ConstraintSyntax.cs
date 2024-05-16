namespace MiniZinc.Parser.Syntax;

public sealed record ConstraintSyntax(Token Start, SyntaxNode Expr)
    : SyntaxNode(Start),
        ILetLocal { }
