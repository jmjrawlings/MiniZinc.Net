namespace MiniZinc.Parser.Ast;

public sealed record Comment : IItem
{
    public string String { get; set; }
}