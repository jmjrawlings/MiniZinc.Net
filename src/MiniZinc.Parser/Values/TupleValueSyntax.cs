namespace MiniZinc.Parser;

public sealed class TupleValueSyntax(List<ValueSyntax> fields) : ValueSyntax(default)
{
    public IReadOnlyList<ValueSyntax> Fields => fields;
}
