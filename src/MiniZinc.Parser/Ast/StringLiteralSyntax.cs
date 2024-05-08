namespace MiniZinc.Parser.Ast;

public sealed record StringLiteralSyntax(Token Token) : SyntaxNode(Token)
{
    public string Value => Start.StringValue;

    public static implicit operator string(StringLiteralSyntax expr) => expr.Value;
}
