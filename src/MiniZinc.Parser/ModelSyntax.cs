namespace MiniZinc.Parser;

using Syntax;

/// <summary>
/// Result of parsing a minizinc model from a file (.mzn) or string
/// </summary>
public sealed class ModelSyntax
{
    public readonly IReadOnlyList<Statement> Statements;

    public ModelSyntax(List<Statement> statements)
    {
        Statements = statements;
    }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteModel(this);
        var mzn = writer.ToString();
        return mzn;
    }
}
