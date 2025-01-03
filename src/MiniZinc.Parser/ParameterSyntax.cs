namespace MiniZinc.Parser;

public sealed class ParameterSyntax : MiniZincSyntax, INamedSyntax
{
    public Token Name { get; }

    public TypeSyntax Type { get; }

    public ParameterSyntax(Token start, in TypeSyntax type, in Token name)
        : base(start)
    {
        Type = type;
        Name = name;
    }
}
