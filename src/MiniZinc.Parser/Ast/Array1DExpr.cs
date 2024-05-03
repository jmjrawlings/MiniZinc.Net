namespace MiniZinc.Parser.Ast;

public sealed record Array1DExpr : ArrayExpr
{
    public bool Indexed { get; set; }

    public override string ToString() => $"<Array of {N} items>";
}
