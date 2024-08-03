namespace MiniZinc.Parser.Syntax;

public sealed class GeneratorCallSyntax : ExpressionSyntax, INamedSyntax
{
    public ExpressionSyntax Expr { get; }
    public List<GeneratorSyntax> Generators { get; }
    public Token Name { get; }

    public GeneratorCallSyntax(Token name, ExpressionSyntax expr, List<GeneratorSyntax> generators)
        : base(name)
    {
        Name = name;
        Expr = expr;
        Generators = generators;
    }
}
