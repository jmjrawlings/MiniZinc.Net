namespace MiniZinc.Parser;

public sealed class Array1dValueSyntax(List<ValueSyntax> values) : ValueSyntax(default)
{
    public IReadOnlyList<ValueSyntax> Values => values;
}