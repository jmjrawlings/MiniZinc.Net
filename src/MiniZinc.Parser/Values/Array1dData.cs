namespace MiniZinc.Parser;

public abstract class Array1dData<T>(List<T> values) : DataSyntax
{
    public IReadOnlyList<T> Values => values;
}

public sealed class IntArray1d(List<int> list) : Array1dData<int>(list) { }

public sealed class FloatArray1d(List<decimal> list) : Array1dData<decimal>(list) { }

public sealed class BoolArray1d(List<bool> list) : Array1dData<bool>(list) { }

public sealed class StringArray1d(List<string> list) : Array1dData<string>(list) { }

public sealed class ValueArray1d(List<DataSyntax> list) : Array1dData<DataSyntax>(list) { }
