namespace MiniZinc.Parser.Syntax;

public sealed record GeneratorSyntax(in Token Start) : ExpressionSyntax(Start)
{
    public required List<IdentifierSyntax> Names { get; set; }

    public required ExpressionSyntax From { get; set; }

    public ExpressionSyntax? Where { get; set; }
}
