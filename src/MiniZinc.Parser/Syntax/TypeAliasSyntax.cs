namespace MiniZinc.Parser.Syntax;

public sealed record TypeAliasSyntax(in Token Start, IdentifierSyntax Name, TypeSyntax Type)
    : SyntaxNode(in Start) { }
