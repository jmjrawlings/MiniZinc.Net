namespace MiniZinc.Parser.Ast;

public sealed record BoolLit(bool Value) : Expr
{
    public static implicit operator bool(BoolLit lit) => lit.Value;

    public override string ToString() => Value.ToString();
}
