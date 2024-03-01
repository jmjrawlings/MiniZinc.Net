namespace MiniZinc.Parser.Ast;

public readonly struct BracketedExpr : IExpr
{
    public IExpr Expr { get; init; }
}
