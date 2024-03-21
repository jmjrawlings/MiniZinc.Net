namespace MiniZinc.Parser.Ast;

/// <summary>
/// An array or set comprehension
/// </summary>
public record CompExpr : Expr
{
    public INode Expr { get; set; }
    public List<GeneratorExpr> Generators { get; set; }
    public bool IsSet { get; set; }
}
