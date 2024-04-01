namespace MiniZinc.Parser.Ast;

public readonly record struct IntLit(int Value) : IExpr
{
    public static implicit operator int(IntLit expr) => expr.Value;

    public static implicit operator IntLit(int i) => new IntLit(i);

    public override string ToString() => Value.ToString();
}
