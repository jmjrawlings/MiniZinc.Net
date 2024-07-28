namespace MiniZinc.Parser.Syntax;

public sealed class IdentifierSyntax : ExpressionSyntax
{
    public IdentifierSyntax(in Token token)
        : base(token) { }

    public TokenKind Kind => Start.Kind;

    public string Name => Start.StringValue;

    public override string ToString() => Start.ToString();
}
