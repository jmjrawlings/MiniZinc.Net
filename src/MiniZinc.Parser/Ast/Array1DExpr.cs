namespace MiniZinc.Parser.Ast;

public record ArrayLit : Expr
{
    public List<Expr> Elements { get; set; } = new();
    public int N => Elements.Count;
}

public sealed record Array1DLit : ArrayLit
{
    public bool Indexed { get; set; }

    public override string ToString() => $"<Array of {N} items>";
}
