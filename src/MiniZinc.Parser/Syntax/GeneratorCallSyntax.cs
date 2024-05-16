namespace MiniZinc.Parser.Syntax;

public sealed record GeneratorCallSyntax(
    Token Name,
    SyntaxNode Expr,
    List<GeneratorSyntax> Generators
) : SyntaxNode(Name) { }
