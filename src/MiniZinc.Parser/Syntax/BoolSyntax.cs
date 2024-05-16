namespace MiniZinc.Parser.Syntax;

public sealed record BoolSyntax(Token Start) : SyntaxNode(Start)
{
    public readonly bool Value = Start.Kind is TokenKind.TRUE;

    public static implicit operator bool(in BoolSyntax syntax) => syntax.Value;

    public override string ToString() => Value.ToString();
}
