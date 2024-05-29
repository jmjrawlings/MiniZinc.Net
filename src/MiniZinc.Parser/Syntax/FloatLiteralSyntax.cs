namespace MiniZinc.Parser.Syntax;

public sealed record FloatLiteralSyntax(in Token Start)
    : SyntaxNode<decimal>(Start, Start.DecimalValue)
{
    public override string ToString() => Value.ToString("g");
}
