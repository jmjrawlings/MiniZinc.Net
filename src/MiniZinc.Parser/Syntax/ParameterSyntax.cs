namespace MiniZinc.Parser.Syntax;

public sealed class ParameterSyntax : SyntaxNode
{
    public readonly TypeSyntax Type;
    public readonly IdentifierSyntax? Identifier;

    public ParameterSyntax(TypeSyntax type, IdentifierSyntax? identifier)
        : base(type.Start)
    {
        Type = type;
        Identifier = identifier;
    }
}
