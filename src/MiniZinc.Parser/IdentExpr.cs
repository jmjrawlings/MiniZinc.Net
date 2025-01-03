namespace MiniZinc.Parser;

public sealed class IdentExpr : MiniZincExpr
{
    public IdentExpr(in Token token)
        : base(token) { }

    public TokenKind Kind => Start.Kind;

    public Token Name => Start;

    public override string ToString() => Start.ToString();
}
