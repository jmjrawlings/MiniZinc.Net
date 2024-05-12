namespace MiniZinc.Parser.Ast;

public sealed record ConstraintSyntax(Token Start, SyntaxNode Expr)
    : SyntaxNode(Start),
        ILetLocal { }
