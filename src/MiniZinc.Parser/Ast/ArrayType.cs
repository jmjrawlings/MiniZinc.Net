namespace MiniZinc.Parser.Ast;

public sealed record ArrayType : TypeInst
{
    public TypeInst Type { get; set; }

    public List<IExpr> Dimensions { get; set; }

    public int N => Dimensions.Count;
}
