namespace MiniZinc.Parser.Syntax;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveSyntax(in Token Start, SolveMethod Method, SyntaxNode? Objective)
    : StatementSyntax(Start) { }
