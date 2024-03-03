namespace MiniZinc.Parser.Ast;

public sealed record ExprType : Type
{
    public IExpr Expr { get; set; }
}
