namespace MiniZinc.Parser;

public sealed class LetExpr : MiniZincExpr
{
    public LetExpr(in Token start, List<ILetLocalSyntax>? locals, MiniZincExpr body)
        : base(start)
    {
        Locals = locals;
        Body = body;
    }

    public List<ILetLocalSyntax>? Locals { get; }

    public MiniZincExpr Body { get; }
}
