namespace MiniZinc.Parser.Ast;

public sealed record IndexAndNode(SyntaxNode Index, SyntaxNode Value) : SyntaxNode(Index.Start) { }
