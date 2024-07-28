namespace MiniZinc.Parser;

public sealed class RecordValueSyntax(Dictionary<string, ValueSyntax> map) : ValueSyntax(default)
{
    private IReadOnlyDictionary<string, ValueSyntax> Values => map;
}