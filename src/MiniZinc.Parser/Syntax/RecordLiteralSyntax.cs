namespace MiniZinc.Parser.Syntax;

public sealed class RecordLiteralSyntax : ExpressionSyntax
{
    public RecordLiteralSyntax(in Token start)
        : base(start) { }

    public List<(IdentifierSyntax, ExpressionSyntax)> Fields { get; } = new();
}
