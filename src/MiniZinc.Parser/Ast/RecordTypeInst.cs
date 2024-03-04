namespace MiniZinc.Parser.Ast;

public sealed record RecordTypeInst : TypeInst
{
    public List<Binding<TypeInst>> Fields { get; set; } = new();
}
