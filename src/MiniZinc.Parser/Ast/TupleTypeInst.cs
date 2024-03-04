namespace MiniZinc.Parser.Ast;

public sealed record TupleTypeInst : TypeInst
{
    public List<TypeInst> Items { get; set; } = new();
}
