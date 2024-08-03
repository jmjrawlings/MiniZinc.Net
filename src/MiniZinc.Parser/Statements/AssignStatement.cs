namespace MiniZinc.Parser.Syntax;

public sealed class AssignStatement : StatementSyntax, ILetLocalSyntax, INamedSyntax
{
    public Token Name { get; }
    public ExpressionSyntax Expr { get; }

    public AssignStatement(in Token name, ExpressionSyntax expr)
        : base(name)
    {
        Name = name;
        Expr = expr;
    }
}
