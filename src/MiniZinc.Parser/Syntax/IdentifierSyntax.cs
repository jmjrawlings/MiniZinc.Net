namespace MiniZinc.Parser.Syntax;

public sealed record IdentifierSyntax(in Token Token) : ExpressionSyntax(Token)
{
    public TokenKind Kind => Token.Kind;

    public string Name => Token.StringValue;

    public override string ToString() => Token.ToString();
}
