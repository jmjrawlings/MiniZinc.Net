namespace MiniZinc.Parser.Ast;

public sealed record IncludeStatement : Node
{
    public string Path { get; set; } = string.Empty;
}
