namespace MiniZinc.Parser.Syntax;

/// <summary>
/// Type Alias
/// </summary>
/// <mzn>type A = record(int: a, bool: b)</mzn>
/// <mzn>type C = 1..100</mzn>
public sealed class TypeAliasSyntax : StatementSyntax, INamedSyntax
{
    public Token Name { get; }

    public TypeSyntax Type { get; }

    public TypeAliasSyntax(in Token start, in Token name, TypeSyntax type)
        : base(in start)
    {
        Name = name;
        Type = type;
    }
}
