namespace MiniZinc.Parser.Syntax;

public sealed class IdentExpr : Expr
{
    public IdentExpr(in Token token)
        : base(token) { }

    public TokenKind Kind => Start.Kind;

    public Token Name => Start;

    public override string ToString() => Start.ToString();
}
