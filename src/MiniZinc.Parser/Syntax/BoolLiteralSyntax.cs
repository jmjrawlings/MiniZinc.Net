namespace MiniZinc.Parser.Syntax;

public sealed record BoolLiteralSyntax(in Token Start) : SyntaxNode(Start)
{
    public readonly bool Value = Start.BoolValue;
    
    public static implicit operator bool(in BoolLiteralSyntax literalSyntax) => literalSyntax.Value;
    
    public override string ToString() => Value.ToString();
}
