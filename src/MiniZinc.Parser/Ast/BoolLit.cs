namespace MiniZinc.Parser.Ast;

public readonly record struct BoolLit(bool Value) : IExpr
{
    public static implicit operator bool(BoolLit lit) => lit.Value;

    public override string ToString() => Value.ToString();
}
