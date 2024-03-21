namespace MiniZinc.Parser.Ast;

public sealed record GeneratorExpr : INode
{
    public List<Identifier> Names { get; set; }

    public INode From { get; set; }

    public INode? Where { get; set; }
}
