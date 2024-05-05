namespace MiniZinc.Parser.Ast;

public sealed record GeneratorCallSyntax(
    Token Name,
    SyntaxNode Expr,
    List<GeneratorSyntax> Generators
) : SyntaxNode(Name) { }
