namespace MiniZinc.Parser.Ast;

public readonly record struct StringLit(string s) : INode
{
    public static implicit operator string(StringLit expr) => expr.s;
}

public readonly record struct Identifier(string s) : INode
{
    public static implicit operator string(Identifier expr) => expr.s;

    public override string ToString() => s;
}
