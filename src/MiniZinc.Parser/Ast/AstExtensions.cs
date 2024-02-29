namespace MiniZinc.Parser;

public static class AstExtensions
{
    public static void Annotate(this IAnnotations x, IExpr expr)
    {
        x.Annotations ??= new List<IExpr>(4);
        x.Annotations.Add(expr);
    }
}
