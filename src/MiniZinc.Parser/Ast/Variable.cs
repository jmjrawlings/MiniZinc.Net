namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record Variable : INamed, IExpr
{
    public string Name { get; set; }

    public TypeInst Type { get; set; }

    public IExpr Value { get; set; }
}
