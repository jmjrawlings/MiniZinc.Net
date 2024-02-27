namespace MiniZinc.Parser.Ast;

public interface IAnnotations
{
    List<Expr>? Annotations { get; }

    void Annotate(Expr ann);
}
