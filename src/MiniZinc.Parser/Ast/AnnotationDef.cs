namespace MiniZinc.Parser.Ast;

public sealed record AnnotationDeclarationSyntax(in Token Start, SyntaxNode Body)
    : SyntaxNode(Start) { }
