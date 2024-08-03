namespace MiniZinc.Parser.Syntax;

public sealed class GeneratorSyntax : ExpressionSyntax
{
    public GeneratorSyntax(in Token start, List<Token> names)
        : base(start)
    {
        Names = names;
    }

    public List<Token> Names { get; }

    public required ExpressionSyntax From { get; set; }

    public ExpressionSyntax? Where { get; set; }
}
