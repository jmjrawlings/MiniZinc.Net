namespace MiniZinc.Parser.Ast;

public readonly record struct FloatLit(double Value) : INode
{
    public static implicit operator double(FloatLit expr) => expr.Value;

    public override string ToString() => Value.ToString("g");
}
