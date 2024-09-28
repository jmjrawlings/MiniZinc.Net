namespace MiniZinc.Parser;

public sealed class EmptyDatum : Datum
{
    public override DatumKind Kind => DatumKind.Unknown;

    public override bool Equals(object? obj) => obj is EmptyDatum;
}
