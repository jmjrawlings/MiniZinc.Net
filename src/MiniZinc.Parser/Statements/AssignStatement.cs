namespace MiniZinc.Parser.Syntax;

public sealed class AssignStatement : StatementSyntax, ILetLocalSyntax, IIdentifiedSyntax
{
    public IdentifierSyntax Identifier { get; }
    public ExpressionSyntax Expr { get; }
    public string Name => Identifier.Name;

    public AssignStatement(IdentifierSyntax identifier, ExpressionSyntax expr)
        : base(identifier.Start)
    {
        Identifier = identifier;
        Expr = expr;
    }
}
