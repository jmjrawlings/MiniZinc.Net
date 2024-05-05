namespace MiniZinc.Parser.Ast;

public sealed record IndexAndValue(SyntaxNode Index, SyntaxNode Value) : SyntaxNode(Index.Start) { }
