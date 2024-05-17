namespace MiniZinc.Parser.Syntax;

public sealed record StringLiteralSyntax(in Token Token) : SyntaxNode(Token)
{
    public string Value => Token.StringValue;

    public static implicit operator string(StringLiteralSyntax expr) => expr.Value;
}
