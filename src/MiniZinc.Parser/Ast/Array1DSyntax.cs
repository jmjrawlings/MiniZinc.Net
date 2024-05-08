namespace MiniZinc.Parser.Ast;

public sealed record Array1DSyntax(Token start) : ArraySyntax(start)
{
    public bool Indexed { get; set; }

    public override string ToString() => $"<Array of {N} items>";
}
