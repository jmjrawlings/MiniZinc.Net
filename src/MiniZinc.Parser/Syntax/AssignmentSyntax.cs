namespace MiniZinc.Parser.Syntax;

public sealed class AssignmentSyntax : StatementSyntax, ILetLocalSyntax, IIdentifiedSyntax
{
    public IdentifierSyntax Identifier { get; }
    public ExpressionSyntax Expr { get; }
    public string Name => Identifier.Name;

    public AssignmentSyntax(IdentifierSyntax identifier, ExpressionSyntax expr)
        : base(identifier.Start)
    {
        Identifier = identifier;
        Expr = expr;
    }
}
