namespace MiniZinc.Parser;

using Syntax;

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

    /// <summary>
    /// Get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    /// <exception cref="Exception">The variable does not exists or was not of the expected type</exception>
    public static T Get<T>(this IReadOnlyDictionary<string, Datum> dict, string id)
        where T : Datum
    {
        var data = dict[id];
        var value = (T)data;
        return value;
    }

    /// Try to get the solution assigned to the given variable
    public static Datum? TryGet(this IReadOnlyDictionary<string, Datum> dict, string id) =>
        dict.GetValueOrDefault(id);

    /// Try to get the solution assigned to the given variable
    public static T? TryGet<T>(this IReadOnlyDictionary<string, Datum> dict, string id)
        where T : Datum
    {
        if (!dict.TryGetValue(id, out var data))
            return null;

        T value = (T)data;
        return value;
    }
}
