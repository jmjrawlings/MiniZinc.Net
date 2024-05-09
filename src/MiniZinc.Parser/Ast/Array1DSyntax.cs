﻿namespace MiniZinc.Parser.Ast;

public sealed record Array1DSyntax(Token Start) : ArraySyntax(Start)
{
    public bool Indexed { get; set; }

    public override string ToString() => $"<Array of {N} items>";
}
