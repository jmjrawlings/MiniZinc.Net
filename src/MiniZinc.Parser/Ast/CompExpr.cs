namespace MiniZinc.Parser.Ast;

/// <summary>
/// An array or set comprehension
/// </summary>
public record CompExpr : Expr
{
    public required Node Expr { get; set; }
    public required List<GeneratorExpr> Generators { get; set; }
    public bool IsSet { get; set; }
}
