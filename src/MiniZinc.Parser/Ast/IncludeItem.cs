namespace MiniZinc.Parser.Ast;

public sealed record IncludeItem : IItem
{
    public string Path { get; set; }
    public FileInfo? File { get; set; }
}
