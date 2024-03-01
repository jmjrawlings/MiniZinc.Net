namespace MiniZinc.Parser.Ast;

public readonly record struct Array3DLiteral(IExpr[,,] elements) : IExpr
{
    public IExpr[,,] Elements => elements;
}
