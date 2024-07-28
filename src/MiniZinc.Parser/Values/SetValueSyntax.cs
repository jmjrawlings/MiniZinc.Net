namespace MiniZinc.Parser;

using Syntax;

public sealed class SetValueSyntax(List<ValueSyntax> values) : ValueSyntax(default)
{
    public IReadOnlyList<ValueSyntax> Values => values;
}
