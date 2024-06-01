namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed record DeclarationSyntax(in Token Start, TypeSyntax Type)
    : SyntaxNode(Start),
        ILetLocal
{
    public IdentifierSyntax Name { get; set; }

    public SyntaxNode? Body { get; set; }

    public List<ParameterSyntax>? Parameters { get; set; }
}
