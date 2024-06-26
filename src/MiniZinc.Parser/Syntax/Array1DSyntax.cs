namespace MiniZinc.Parser.Syntax;

public sealed class Array1DSyntax : ArraySyntax
{
    public Array1DSyntax(in Token start)
        : base(start) { }

    public bool Indexed { get; set; }

    public override string ToString() => $"<Array of {N} items>";
}
