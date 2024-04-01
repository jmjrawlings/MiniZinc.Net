namespace MiniZinc.Parser.Ast;

public interface IAnnotations
{
    public IEnumerable<IExpr> Annotations { get; }
    public void Annotate(IExpr expr);
}

public struct Annotated<T> : IAnnotations
{
    public readonly T Value;
    private readonly List<IExpr> _annotations;
    public IEnumerable<IExpr> Annotations => _annotations;

    public Annotated(T value)
    {
        Value = value;
        _annotations = new List<IExpr>();
    }

    public void Annotate(IExpr expr)
    {
        _annotations.Add(expr);
    }
}
