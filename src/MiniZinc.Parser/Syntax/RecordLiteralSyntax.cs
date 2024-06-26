namespace MiniZinc.Parser.Syntax;

public sealed class RecordLiteralSyntax : ExpressionSyntax
{
    public RecordLiteralSyntax(in Token start)
        : base(start) { }

    public List<(IdentifierSyntax, SyntaxNode)> Fields { get; } = new();
}
