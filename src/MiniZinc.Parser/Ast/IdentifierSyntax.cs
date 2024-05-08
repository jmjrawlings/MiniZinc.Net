namespace MiniZinc.Parser.Ast;

public sealed record IdentifierSyntax(in Token Token) : SyntaxNode(Token)
{
    public string Name => Token.StringValue;

    public override string ToString() => Token.StringValue;
}
