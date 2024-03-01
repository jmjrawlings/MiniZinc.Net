namespace MiniZinc.Parser.Ast;

public sealed record NamedType : Type
{
    public string Name { get; set; }
    public bool IsGeneric { get; set; }
}
