namespace MiniZinc.Parser;

public abstract class GenExpr : MiniZincExpr
{
    public MiniZincExpr Source { get; }

    public MiniZincExpr? Where { get; }

    protected GenExpr(in Token start, MiniZincExpr source, MiniZincExpr? where)
        : base(start)
    {
        Source = source;
        Where = where;
    }
}

/// Generator expr of type `$id$ in $source$ where $cond$`
public sealed class GenYieldExpr : GenExpr
{
    public IReadOnlyList<Token> Ids { get; }

    public GenYieldExpr(
        in Token start,
        IReadOnlyList<Token> ids,
        MiniZincExpr source,
        MiniZincExpr? where
    )
        : base(start, source, where)
    {
        Ids = ids;
    }
}

/// Generator expr of type `$id$ = $expr$`
public sealed class GenAssignExpr : GenExpr
{
    public Token Id { get; }

    public GenAssignExpr(in Token id, MiniZincExpr source, MiniZincExpr? where)
        : base(id, source, where)
    {
        Id = id;
    }
}
