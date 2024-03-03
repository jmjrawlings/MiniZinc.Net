namespace MiniZinc.Parser.Ast;

public sealed record RecordType : Type
{
    public List<Binding<Type>> Fields { get; set; } = new();
}
