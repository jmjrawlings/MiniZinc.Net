namespace MiniZinc.Parser.Syntax;

public sealed record IdentifierSyntax(in Token Token) : SyntaxNode(Token)
{
    public string Name => Token.StringValue;

    public TokenKind Kind => Token.Kind;

    public override string ToString() => Token.StringValue;
}
