namespace MiniZinc.Parser.Ast;

public sealed record VariableDeclareItem : INamed, IExpr
{
    public string Name { get; set; }
    public TypeInst Type { get; set; }
    public IExpr? Value;
}
