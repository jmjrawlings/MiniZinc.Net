namespace MiniZinc.Parser.Ast;

public sealed record ArrayType : Type
{
    public Type ValueType { get; set; }

    public List<IExpr> Dimensions { get; set; }

    public int N => Dimensions.Count;
}
