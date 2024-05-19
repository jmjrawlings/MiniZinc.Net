namespace MiniZinc.Parser.Syntax;

public sealed record IdentifierSyntax(in Token Token) : SyntaxNode(Token)
{
    public TokenKind Kind => Token.Kind;

    public override string ToString() => Token.ToString();
}
