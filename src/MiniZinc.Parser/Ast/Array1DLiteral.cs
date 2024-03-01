namespace MiniZinc.Parser.Ast;

public sealed record Array1DLiteral(IExpr[] elements) : IExpr
{
    public IExpr[] Elements => elements;
}
