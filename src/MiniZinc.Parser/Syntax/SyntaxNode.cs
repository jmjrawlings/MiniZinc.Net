namespace MiniZinc.Parser.Syntax;

public abstract class SyntaxNode
{
    public readonly Token Start;

    protected SyntaxNode(in Token start)
    {
        Start = start;
    }

    public List<SyntaxNode>? Annotations { get; set; } = null;

    public string SourceText => Write(WriteOptions.Minimal);

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteNode(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public override string ToString() => SourceText;
}
