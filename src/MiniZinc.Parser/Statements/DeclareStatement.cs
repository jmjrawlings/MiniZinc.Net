namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed class DeclareStatement : StatementSyntax, ILetLocalSyntax, INamedSyntax
{
    public Token Name { get; }

    public TypeSyntax? Type { get; }

    public DeclareKind Kind { get; }

    public ExpressionSyntax? Body { get; set; }

    /// <summary>
    /// Typed parameter list if this is a function-like declaration
    /// </summary>
    public List<(Token, TypeSyntax)>? Parameters { get; set; }

    public Token? Ann { get; init; }

    public bool IsAnnotation => Type.Kind is TypeKind.Annotation;

    /// <summary>
    /// A variable
    /// </summary>
    /// <mzn>int: a = 10</mzn>
    /// <mzn>var int: a = 10</mzn>
    public DeclareStatement(in Token start, DeclareKind kind, TypeSyntax? type, Token name)
        : base(start)
    {
        Type = type;
        Kind = kind;
        Name = name;
    }
}
