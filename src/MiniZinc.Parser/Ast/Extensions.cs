namespace MiniZinc.Parser.Ast;

public static class Extensions
{
    public static void Annotate(this IAnnotations x, INode expr)
    {
        x.Annotations ??= new List<INode>(4);
        x.Annotations.Add(expr);
    }
}
