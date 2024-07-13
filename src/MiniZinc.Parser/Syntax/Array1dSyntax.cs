namespace MiniZinc.Parser.Syntax;

public sealed class Array1dSyntax : ArraySyntax
{
    public Array1dSyntax(in Token start)
        : base(start) { }

    public bool Indexed { get; set; }
}
