namespace MiniZinc.Parser.Ast;

/// <summary>
/// An array or set comprehension
/// </summary>
public record CompExpr : Expr
{
    public required SyntaxNode Expr { get; set; }
    public required List<GeneratorExpr> Generators { get; set; }
    public bool IsSet { get; set; }
}
