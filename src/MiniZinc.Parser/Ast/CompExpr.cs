namespace MiniZinc.Parser.Ast;

/// <summary>
/// An array or set comprehension
/// </summary>
public record CompExpr : Expr
{
    public Node Expr { get; set; }
    public List<GeneratorExpr> Generators { get; set; }
    public bool IsSet { get; set; }
}
