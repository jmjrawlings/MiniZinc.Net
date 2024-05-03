namespace MiniZinc.Parser.Ast;

public sealed record Array3dExpr : ArrayExpr
{
    public int I { get; set; }
    public int J { get; set; }
    public int K { get; set; }

    public override string ToString() => $"<{I}x{J}x{K} array of {N} items>";
}
