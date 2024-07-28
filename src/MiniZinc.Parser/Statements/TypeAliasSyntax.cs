namespace MiniZinc.Parser.Syntax;

/// <summary>
/// Type Alias
/// </summary>
/// <mzn>type A = record(int: a, bool: b)</mzn>
/// <mzn>type C = 1..100</mzn>
public sealed class TypeAliasSyntax : StatementSyntax, IIdentifiedSyntax
{
    public IdentifierSyntax Identifier { get; }

    public TypeSyntax Type { get; }

    public TypeAliasSyntax(in Token start, in IdentifierSyntax identifer, TypeSyntax type)
        : base(in start)
    {
        Identifier = identifer;
        Type = type;
    }
}
