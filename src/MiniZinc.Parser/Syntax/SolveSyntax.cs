namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveSyntax(Token Start, SolveMethod Method, SyntaxNode? Objective)
    : SyntaxNode(Start) { }
