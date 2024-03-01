namespace MiniZinc.Parser.Ast;

public sealed record RecordType : Type
{
    public Dictionary<string, Type> Fields { get; set; } = new();
}
