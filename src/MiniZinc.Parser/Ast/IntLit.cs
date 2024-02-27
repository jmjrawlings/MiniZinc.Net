namespace MiniZinc.Parser.Ast;

public sealed record IntLit(int Value) : Expr
{
    public static implicit operator int(IntLit expr) => expr.Value;

    public static implicit operator IntLit(int i) => new IntLit(i);

    public override string ToString() => Value.ToString();
}
