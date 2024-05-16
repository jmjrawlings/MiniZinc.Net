namespace MiniZinc.Parser.Syntax;

public sealed record IndexAndNode(SyntaxNode Index, SyntaxNode Value) : SyntaxNode(Index.Start) { }
