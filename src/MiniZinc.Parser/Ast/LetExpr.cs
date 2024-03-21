namespace MiniZinc.Parser.Ast;

public sealed record LetExpr : INode
{
    public List<ILetLocal>? Locals { get; set; }

    public INode Body { get; set; }
}
