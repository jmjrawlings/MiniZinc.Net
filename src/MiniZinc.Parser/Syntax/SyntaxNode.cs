namespace MiniZinc.Parser.Syntax;

public abstract record SyntaxNode(in Token Start)
{
    public List<SyntaxNode>? Annotations { get; set; } = null;

    /// <summary>
    /// Write the given node as a minizinc model string
    /// </summary>
    public string Write(WriteOptions? options = null)
    {
        var s = Writer.WriteNode(this, options);
        return s;
    }

    public override string ToString() => Write();
}

public abstract record SyntaxNode<T>(in Token Start, T Value) : SyntaxNode(Start) { }
