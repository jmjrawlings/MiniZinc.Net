namespace MiniZinc.Parser.Ast;

public sealed record TupleType : Type
{
    public List<Type> Params { get; set; } = new();
}
