namespace MiniZinc.Parser.Syntax;

public static class SyntaxNodeExtensions
{
    /// <summary>
    /// Write this node as a minizinc model string
    /// </summary>
    public static string Write(this SyntaxNode node, WriteOptions? options = null)
    {
        var s = Writer.WriteNode(node, options);
        return s;
    }

    /// <summary>
    /// Create a deep clone of this syntax node
    /// </summary>
    public static T Clone<T>(this T node)
        where T : SyntaxNode
    {
        var mzn = Writer.WriteNode(node, WriteOptions.Minimal);
        var parser = new Parser(mzn);
        // TODO - optimised function for parsing a single node to avoid list allocation?
        if (!parser.ParseTree(out var tree))
            throw new Exception();
        var copy = tree.Nodes[0];
        if (copy is not T t)
            throw new Exception();
        return t;
    }
}