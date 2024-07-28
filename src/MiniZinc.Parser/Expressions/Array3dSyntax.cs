namespace MiniZinc.Parser.Syntax;

public sealed class Array3dSyntax : ArraySyntax
{
    public Array3dSyntax(in Token start)
        : base(start) { }

    public int I { get; set; }
    public int J { get; set; }
    public int K { get; set; }

    public override string ToString() => $"<{I}x{J}x{K} array of {N} items>";
}
