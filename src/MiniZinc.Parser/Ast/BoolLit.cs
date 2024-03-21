namespace MiniZinc.Parser.Ast;

public readonly record struct BoolLit(bool Value) : INode
{
    public static implicit operator bool(BoolLit lit) => lit.Value;
}
