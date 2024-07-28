namespace MiniZinc.Parser;

public sealed class TupleValueSyntax(List<ValueSyntax> fields) : ValueSyntax(default)
{
    private IReadOnlyList<ValueSyntax> Fields => fields;
}
