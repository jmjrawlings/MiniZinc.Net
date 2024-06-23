namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed record DeclarationSyntax(in Token Start, TypeSyntax Type, IdentifierSyntax Identifier)
    : StatementSyntax(Start),
        ILetLocalSyntax,
        IIdentifiedSyntax
{
    public string Name => Identifier.Name;

    public SyntaxNode? Body { get; set; }

    public List<ParameterSyntax>? Parameters { get; set; }

    public IdentifierSyntax? Ann { get; init; }

    public required bool IsFunction { get; init; }

    public bool IsAnnotation => Type.Kind is TypeKind.Annotation;
}
