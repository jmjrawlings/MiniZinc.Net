namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed record DeclarationSyntax(in Token Start, TypeSyntax Type, IdentifierSyntax Name)
    : SyntaxNode(Start),
        ILetLocal,
        INamedSyntax
{
    public SyntaxNode? Body { get; set; }

    public List<ParameterSyntax>? Parameters { get; set; }
}
