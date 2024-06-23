namespace MiniZinc.Parser.Syntax;

public abstract record SyntaxNode(in Token Start)
{
    public List<SyntaxNode>? Annotations { get; set; } = null;

    public string SourceText => Write(WriteOptions.Minimal);

    public override string ToString() => SourceText;

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteNode(this);
        var mzn = writer.ToString();
        return mzn;
    }
}
