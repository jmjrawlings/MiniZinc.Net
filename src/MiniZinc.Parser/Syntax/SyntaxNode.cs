namespace MiniZinc.Parser.Syntax;

public abstract record SyntaxNode(in Token Start)
{
    public List<SyntaxNode>? Annotations { get; set; } = null;

    public string SourceText => this.Write(WriteOptions.Minimal);

    public override string ToString() => SourceText;
}

public abstract record SyntaxNode<T>(in Token Start, T Value) : SyntaxNode(Start) { }
