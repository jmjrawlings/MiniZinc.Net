namespace MiniZinc.Parser;

public sealed class TupleValueSyntax(List<(string, ValueSyntax)> fields) : ValueSyntax(default)
{
    private IReadOnlyList<(string, ValueSyntax)> Fields => fields;
}