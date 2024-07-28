namespace MiniZinc.Parser;

public sealed class RecordValueSyntax(Dictionary<string, ValueSyntax> fields) : ValueSyntax(default)
{
    public IReadOnlyDictionary<string, ValueSyntax> Fields => fields;
}
