namespace MiniZinc.Parser.Ast;

public sealed record ExprType : TypeInst
{
    public IExpr Expr { get; set; }
}
