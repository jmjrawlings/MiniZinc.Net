namespace MiniZinc.Parser.Syntax;

public static class SyntaxExtensions
{
    /// <summary>
    /// Write this node as a minizinc string
    /// </summary>
    public static string Write(this SyntaxNode node, WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteSyntax(node);
        var mzn = writer.ToString();
        return mzn;
    }

    /// <summary>
    /// Create a deep clone of this syntax node
    /// </summary>
    public static T Clone<T>(this T node)
        where T : SyntaxNode
    {
        var mzn = node.Write(WriteOptions.Minimal);
        var parser = new Parser(mzn);
        // TODO - optimised function for parsing a single node to avoid list allocation?
        if (!parser.ParseStatement(out var statement))
            throw new Exception();
        if (statement is not T t)
            throw new Exception();
        return t;
    }
}
