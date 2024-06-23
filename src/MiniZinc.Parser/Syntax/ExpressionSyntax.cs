namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An expression lives inside statements and other expressions.
/// </summary>
public abstract record ExpressionSyntax(in Token Start) : SyntaxNode(Start) { }

public abstract record ExpressionSyntax<T>(in Token Start, T Value) : ExpressionSyntax(Start) { }
