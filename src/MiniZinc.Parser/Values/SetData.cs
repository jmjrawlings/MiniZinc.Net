namespace MiniZinc.Parser;

public abstract class SetData : DataSyntax { }

public sealed class IntSet(HashSet<int> values) : SetData
{
    public IReadOnlySet<int> Values => values;
}

public sealed class FloatSet(HashSet<decimal> values) : SetData
{
    public IReadOnlySet<decimal> Values => values;
}

public sealed class BoolSet(HashSet<bool> values) : SetData
{
    public IReadOnlySet<bool> Values => values;
}

public sealed class ValueSet(List<DataSyntax> values) : SetData
{
    public IReadOnlyList<DataSyntax> Values => values;
}
