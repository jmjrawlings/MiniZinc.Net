namespace MiniZinc.Parser;

public sealed class EmptyDatum : MiniZincDatum
{
    public override bool Equals(object? obj) => obj is EmptyDatum;
}