namespace MiniZinc.Parser.Syntax;

/// <summary>
/// Result of parsing a minizinc data from a file (.dzn) or string
/// </summary>
public sealed class DataSyntax : IEquatable<DataSyntax>
{
    public readonly IReadOnlyList<AssignmentSyntax> Assignments;
    public readonly IReadOnlyDictionary<string, ExpressionSyntax> Variables;

    public DataSyntax(
        IReadOnlyList<AssignmentSyntax> assignments,
        IReadOnlyDictionary<string, ExpressionSyntax> variables
    )
    {
        Assignments = assignments;
        Variables = variables;
    }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteData(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public bool Equals(DataSyntax? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        foreach (var assign in Assignments)
        {
            var name = assign.Name;
            var expr = assign.Expr;
            if (!other.Variables.TryGetValue(name, out var b))
                return false;
            if (!expr.Equals(b))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        return Equals(obj as DataSyntax);
    }
}
