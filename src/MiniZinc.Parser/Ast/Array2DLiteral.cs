namespace MiniZinc.Parser.Ast;

public sealed record Array2DLiteral(IExpr[,] elements) : IExpr
{
    public IExpr[,] Elements => elements;
}
