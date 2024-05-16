namespace MiniZinc.Parser.Syntax;

public sealed record FloatLiteralSyntax(Token start) : SyntaxNode(start)
{
    public double Value => start.DoubleValue;

    public static implicit operator double(FloatLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString("g");
}
