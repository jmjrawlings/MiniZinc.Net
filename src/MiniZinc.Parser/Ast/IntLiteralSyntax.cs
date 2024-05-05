namespace MiniZinc.Parser.Ast;

public sealed record IntLiteralSyntax(Token start) : SyntaxNode(start)
{
    public int Value => start.IntValue;

    public static implicit operator int(IntLiteralSyntax expr) => expr.Value;

    public override string ToString() => Value.ToString();
}
