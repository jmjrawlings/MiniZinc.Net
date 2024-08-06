namespace MiniZinc.Parser;

public sealed class TupleData(List<DataSyntax> fields) : DataSyntax
{
    public IReadOnlyList<DataSyntax> Fields => fields;
}
