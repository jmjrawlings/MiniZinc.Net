namespace MiniZinc.Parser.Syntax;

public sealed class GeneratorCallSyntax : ExpressionSyntax, IIdentifiedSyntax
{
    public IdentifierSyntax Identifier { get; }
    public ExpressionSyntax Expr { get; }
    public List<GeneratorSyntax> Generators { get; }
    public string Name => Identifier.Name;

    public GeneratorCallSyntax(
        IdentifierSyntax identifier,
        ExpressionSyntax expr,
        List<GeneratorSyntax> generators
    )
        : base(identifier.Start)
    {
        Identifier = identifier;
        Expr = expr;
        Generators = generators;
    }
}
