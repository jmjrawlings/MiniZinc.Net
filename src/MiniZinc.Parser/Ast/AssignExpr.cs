namespace MiniZinc.Parser.Ast;

/// <summary>
/// a = 3;
/// </summary>
public readonly record struct AssignExpr : IItem, INamed
{
    public AssignExpr(string name, IExpr expr)
    {
        Name = name;
        Expr = expr;
    }

    public string Name { get; }
    public IExpr Expr { get; }
}
