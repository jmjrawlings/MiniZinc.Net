namespace MiniZinc.Parser;

public sealed class CallExpr : MiniZincExpr, INamedSyntax
{
    public Token Name { get; }

    public IReadOnlyList<MiniZincExpr>? Args { get; }

    public CallExpr(in Token name, IReadOnlyList<MiniZincExpr>? args = null)
        : base(name)
    {
        Args = args;
        Name = name;
    }
}
