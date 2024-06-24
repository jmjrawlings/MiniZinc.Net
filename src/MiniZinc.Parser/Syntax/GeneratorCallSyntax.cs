namespace MiniZinc.Parser.Syntax;

public sealed record GeneratorCallSyntax(
    IdentifierSyntax Name,
    SyntaxNode Expr,
    List<GeneratorSyntax> Generators
) : ExpressionSyntax(Name.Start) { }
