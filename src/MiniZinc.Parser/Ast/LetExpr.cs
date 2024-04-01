namespace MiniZinc.Parser.Ast;

public sealed record LetExpr : IExpr
{
    public List<ILetLocal>? Locals { get; set; }

    public IExpr Body { get; set; }
}
