namespace MiniZinc.Parser.Ast;

public interface ILetLocal { }

public sealed record LetExpr : Expr
{
    public List<ILetLocal>? Locals { get; set; }

    public Expr Body { get; set; }
}
