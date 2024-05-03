namespace MiniZinc.Parser.Ast;

public sealed record IntExpr(int Value) : Expr
{
    public static implicit operator int(IntExpr expr) => expr.Value;

    public static implicit operator IntExpr(int i) => new IntExpr(i);

    public override string ToString() => Value.ToString();
}
