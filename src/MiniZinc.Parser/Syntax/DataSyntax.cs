namespace MiniZinc.Parser.Syntax;

/// <summary>
/// Result of parsing a minizinc data from a file (.dzn) or string
/// </summary>
public sealed class DataSyntax
{
    public readonly IReadOnlyList<AssignmentSyntax> Assignments;

    public DataSyntax(IReadOnlyList<AssignmentSyntax> assignments)
    {
        Assignments = assignments;
    }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteData(this);
        var mzn = writer.ToString();
        return mzn;
    }
}
