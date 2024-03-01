namespace MiniZinc.Parser.Ast;

public sealed record IncludeStatement : IStatement
{
    public string Path { get; set; }
    public FileInfo? File { get; set; }
}
