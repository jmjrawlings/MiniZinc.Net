namespace MiniZinc.Parser.Syntax;

public sealed class BoolLiteralSyntax : ExpressionSyntax<bool>
{
    public BoolLiteralSyntax(in Token start)
        : base(start, start.BoolValue) { }

    public static implicit operator bool(in BoolLiteralSyntax literalSyntax) => literalSyntax.Value;

    public override string ToString() => Value.ToString();
}
