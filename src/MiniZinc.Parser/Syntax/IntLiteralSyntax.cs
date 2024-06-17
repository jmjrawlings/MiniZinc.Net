namespace MiniZinc.Parser.Syntax;

public sealed record IntLiteralSyntax : SyntaxNode<int>
{
    public IntLiteralSyntax(in Token Start)
        : base(Start, Start.IntValue) { }

    public IntLiteralSyntax(in Token Start, int Value)
        : base(Start, Value) { }

    public static implicit operator int(IntLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString();
}
