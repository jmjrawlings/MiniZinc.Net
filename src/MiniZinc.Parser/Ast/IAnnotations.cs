namespace MiniZinc.Parser.Ast;

public interface IAnnotations
{
    public List<IExpr>? Annotations { get; set; }
}