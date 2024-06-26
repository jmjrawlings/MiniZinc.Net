namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed class DeclarationSyntax : StatementSyntax, ILetLocalSyntax, IIdentifiedSyntax
{
    public string Name => Identifier.Name;

    public SyntaxNode? Body { get; set; }

    public List<ParameterSyntax>? Parameters { get; set; }

    public IdentifierSyntax? Ann { get; init; }

    public required bool IsFunction { get; init; }

    public bool IsAnnotation => Type.Kind is TypeKind.Annotation;

    public TypeSyntax Type { get; }

    public IdentifierSyntax Identifier { get; }

    /// <summary>
    /// A variable
    /// </summary>
    /// <mzn>int: a = 10</mzn>
    /// <mzn>var int: a = 10</mzn>
    public DeclarationSyntax(in Token start, TypeSyntax type, IdentifierSyntax identifier)
        : base(start)
    {
        Type = type;
        Identifier = identifier;
    }
}
