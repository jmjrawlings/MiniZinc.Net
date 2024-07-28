namespace MiniZinc.Parser;

public sealed class BoolLiteralSyntax : ValueSyntax<bool>
{
    public BoolLiteralSyntax(in Token start)
        : base(start, start.BoolValue) { }

    public static implicit operator bool(in BoolLiteralSyntax literalSyntax) => literalSyntax.Value;

    public override string ToString() => Value.ToString();
}
