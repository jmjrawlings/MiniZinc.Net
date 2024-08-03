namespace MiniZinc.Parser.Syntax;

public sealed class RecordLiteralSyntax : ExpressionSyntax
{
    public RecordLiteralSyntax(in Token start)
        : base(start) { }

    public List<(Token, ExpressionSyntax)> Fields { get; } = new();
}
