namespace MiniZinc.Parser.Ast;

public static class Extensions
{
    public static void Annotate(this IAnnotations x, IExpr expr)
    {
        x.Annotations ??= new List<IExpr>(4);
        x.Annotations.Add(expr);
    }
}
