namespace MiniZinc.Parser.Ast;

public readonly record struct IntLit(int Value) : INode
{
    public static implicit operator int(IntLit expr) => expr.Value;
}
