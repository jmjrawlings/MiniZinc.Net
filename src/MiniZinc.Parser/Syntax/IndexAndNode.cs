namespace MiniZinc.Parser.Syntax;

public sealed record IndexAndNode(SyntaxNode Index, SyntaxNode Value)
    : ExpressionSyntax(Index.Start) { }
