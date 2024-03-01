namespace MiniZinc.Parser.Ast;

public sealed record TupleType : Type
{
    public List<Type> Items { get; set; } = new();
}
