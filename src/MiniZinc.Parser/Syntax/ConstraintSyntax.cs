namespace MiniZinc.Parser.Syntax;

public sealed record ConstraintSyntax(in Token Start, SyntaxNode Expr)
    : SyntaxNode(Start),
        ILetLocal { }
