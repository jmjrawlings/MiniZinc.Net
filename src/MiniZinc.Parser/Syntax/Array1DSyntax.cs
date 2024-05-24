namespace MiniZinc.Parser.Syntax;

public sealed record Array1DSyntax(in Token Start) : ArraySyntax(Start)
{
    public bool Indexed { get; set; }

    public override string ToString() => $"<Array of {N} items>";
}
