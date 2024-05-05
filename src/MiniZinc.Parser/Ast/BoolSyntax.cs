namespace MiniZinc.Parser.Ast;

public sealed record BoolSyntax(Token Start) : SyntaxNode(Start)
{
    public readonly bool Value = Start.Kind is TokenKind.TRUE;

    public static implicit operator bool(BoolSyntax syntax) => syntax.Value;

    public override string ToString() => Value.ToString();
}
