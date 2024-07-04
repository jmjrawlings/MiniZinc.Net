namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed class DeclarationSyntax : StatementSyntax, ILetLocalSyntax, IIdentifiedSyntax
{
    public IdentifierSyntax Identifier { get; }

    public TypeSyntax? Type { get; }

    public DeclareKind Kind { get; }

    public ExpressionSyntax? Body { get; set; }

    public List<ParameterSyntax>? Parameters { get; set; }

    public IdentifierSyntax? Ann { get; init; }

    public string Name => Identifier.Name;

    public bool IsAnnotation => Type.Kind is TypeKind.Annotation;

    /// <summary>
    /// A variable
    /// </summary>
    /// <mzn>int: a = 10</mzn>
    /// <mzn>var int: a = 10</mzn>
    public DeclarationSyntax(
        in Token start,
        DeclareKind kind,
        TypeSyntax? type,
        IdentifierSyntax identifier
    )
        : base(start)
    {
        Type = type;
        Kind = kind;
        Identifier = identifier;
    }
}
