namespace MiniZinc.Parser.Ast;

public sealed record Comment : IStatement
{
    public string String { get; set; }
}