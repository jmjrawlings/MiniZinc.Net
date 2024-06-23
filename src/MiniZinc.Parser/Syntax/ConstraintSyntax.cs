namespace MiniZinc.Parser.Syntax;

public sealed record ConstraintSyntax(in Token Start, SyntaxNode Expr)
    : StatementSyntax(Start),
        ILetLocalSyntax { }
