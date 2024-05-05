﻿namespace MiniZinc.Parser.Ast;

public record ArrayExpr(Token start) : SyntaxNode(start)
{
    public List<SyntaxNode> Elements { get; set; } = new();
    public int N => Elements.Count;
}
