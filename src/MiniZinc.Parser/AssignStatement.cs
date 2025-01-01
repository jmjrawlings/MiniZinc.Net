namespace MiniZinc.Parser.Syntax;

public sealed class AssignStatement : Statement, ILetLocalSyntax, INamedSyntax
{
    public Token Name { get; }
    public Expr Expr { get; }

    public AssignStatement(in Token name, Expr expr)
        : base(name)
    {
        Name = name;
        Expr = expr;
    }
}
