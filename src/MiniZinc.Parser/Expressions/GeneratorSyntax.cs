namespace MiniZinc.Parser.Syntax;

public sealed class GeneratorSyntax : ExpressionSyntax
{
    public GeneratorSyntax(in Token start)
        : base(start) { }

    public required List<IdentifierSyntax> Names { get; set; }

    public required ExpressionSyntax From { get; set; }

    public ExpressionSyntax? Where { get; set; }
}
