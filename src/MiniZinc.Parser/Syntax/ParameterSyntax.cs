namespace MiniZinc.Parser.Syntax;

public sealed record ParameterSyntax(TypeSyntax Type, IdentifierSyntax? Name) : SyntaxNode(Type) { }
