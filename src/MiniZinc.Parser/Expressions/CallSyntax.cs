namespace MiniZinc.Parser.Syntax;

public sealed class CallSyntax : ExpressionSyntax, INamedSyntax
{
    public Token Name { get; }

    public IReadOnlyList<ExpressionSyntax>? Args { get; }

    public CallSyntax(in Token name, IReadOnlyList<ExpressionSyntax>? args = null)
        : base(name)
    {
        Args = args;
        Name = name;
    }
}
