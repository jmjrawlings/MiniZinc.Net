namespace MiniZinc.Parser.Syntax;

public sealed record FloatLiteralSyntax(in Token Start) : SyntaxNode(Start)
{
    public double Value => Start.DoubleValue;

    public static implicit operator double(FloatLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString("g");
}
