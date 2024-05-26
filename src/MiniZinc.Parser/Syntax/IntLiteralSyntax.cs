namespace MiniZinc.Parser.Syntax;

public sealed record IntLiteralSyntax(in Token Start) : SyntaxNode<int>(Start, Start.IntValue)
{
    public static implicit operator int(IntLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString();
}
