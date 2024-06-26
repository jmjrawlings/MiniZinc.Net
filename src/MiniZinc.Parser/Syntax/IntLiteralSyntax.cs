namespace MiniZinc.Parser.Syntax;

public sealed class IntLiteralSyntax : ExpressionSyntax<int>
{
    public IntLiteralSyntax(in Token token)
        : base(token, token.IntValue) { }

    public IntLiteralSyntax(in Token token, int value)
        : base(token, value) { }

    public static implicit operator int(IntLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString();
}
