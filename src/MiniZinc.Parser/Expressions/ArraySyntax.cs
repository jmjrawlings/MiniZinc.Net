namespace MiniZinc.Parser.Syntax;

public class ArraySyntax : ExpressionSyntax
{
    public ArraySyntax(in Token start)
        : base(start) { }

    public List<ExpressionSyntax> Elements { get; } = new();
    public int N => Elements.Count;
}
