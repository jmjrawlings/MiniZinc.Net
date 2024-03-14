namespace MiniZinc.Parser.Ast;

public readonly record struct NameExpr(string value) : IExpr
{
    public string Value => value;
}
