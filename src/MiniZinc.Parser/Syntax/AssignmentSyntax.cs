namespace MiniZinc.Parser.Syntax;

public sealed record AssignmentSyntax(IdentifierSyntax Name, SyntaxNode Expr) : SyntaxNode(Name.Start), ILetLocal
{
    
}
