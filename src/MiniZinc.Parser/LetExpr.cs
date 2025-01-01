namespace MiniZinc.Parser.Syntax;

public sealed class LetExpr : Expr
{
    public LetExpr(in Token start, List<ILetLocalSyntax>? locals, Expr body)
        : base(start)
    {
        Locals = locals;
        Body = body;
    }

    public List<ILetLocalSyntax>? Locals { get; }

    public Expr Body { get; }
}
