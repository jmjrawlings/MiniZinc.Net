namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A statement lives at the top level of a MiniZinc model as
/// opposed to expressions.
/// </summary>
public abstract record StatementSyntax(in Token Start) : SyntaxNode(Start) { }
