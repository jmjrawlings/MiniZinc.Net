namespace MiniZinc.Parser.Ast;

public readonly record struct IdExpr(string value) : IExpr
{
    public string Value => value;
}
