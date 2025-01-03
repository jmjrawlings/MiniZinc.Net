namespace MiniZinc.Parser;

public sealed class AssignItem : MiniZincItem, ILetLocalSyntax, INamedSyntax
{
    public Token Name { get; }
    public MiniZincExpr Expr { get; }

    public AssignItem(in Token name, MiniZincExpr expr)
        : base(name)
    {
        Name = name;
        Expr = expr;
    }
}
