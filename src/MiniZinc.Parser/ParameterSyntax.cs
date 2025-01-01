namespace MiniZinc.Parser.Syntax;

public sealed class ParameterSyntax : Syntax, INamedSyntax
{
    public Token Name { get; }

    public TypeSyntax Type { get; }

    public ParameterSyntax(in Token start, in TypeSyntax type, in Token name)
        : base(in start)
    {
        Type = type;
        Name = name;
    }
}
