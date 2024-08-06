namespace MiniZinc.Parser;

public sealed class RecordData(Dictionary<string, DataSyntax> fields) : DataSyntax
{
    public IReadOnlyDictionary<string, DataSyntax> Fields => fields;
}
