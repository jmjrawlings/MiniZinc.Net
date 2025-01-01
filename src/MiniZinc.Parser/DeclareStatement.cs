namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
/// <mzn>int: a = 10</mzn>
/// <mzn>var int: a = 10</mzn>
public sealed class DeclareStatement : Statement, ILetLocalSyntax, INamedSyntax
{
    public Token Name { get; }

    public TypeSyntax? Type { get; }

    public DeclareKind Kind { get; }

    public Expr? Body { get; set; }

    /// Typed parameter list if this is a function-like declaration
    public List<ParameterSyntax>? Parameters { get; set; }

    /// Single
    public Token? Ann { get; init; }

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
