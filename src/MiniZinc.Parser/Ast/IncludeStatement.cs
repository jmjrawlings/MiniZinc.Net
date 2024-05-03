namespace MiniZinc.Parser.Ast;

public sealed record IncludeStatement : SyntaxNode
{
    public string Path { get; set; } = string.Empty;
}
