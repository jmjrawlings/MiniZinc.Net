namespace MiniZinc.Parser;

using Annotation = IExpr;
using Annotations = List<IExpr>;
using ArrayDim = Type;
using Ident = String;

public interface IExpr { }

public record Expr<T> : IExpr
{
    public T Value { get; set; }
}

public sealed record IntExpr : Expr<int>, IExpr { }

public sealed record FloatExpr : Expr<float>, IExpr { }

public sealed record WildCardExpr : IExpr { }

public sealed record AbsentExpr : IExpr { }

public sealed record BoolExpr : Expr<bool>, IExpr { }

public sealed record StringExpr : Expr<string>, IExpr { }

public sealed record IdExpr : Expr<String>, IExpr { }

public sealed record BracketedExpr : IExpr
{
    public IExpr Expr { get; set; }
}

public sealed record LeftOpenRangeExpr : IExpr
{
    public IExpr Min { get; set; }
}

public sealed record RightOpenRangeExpr : IExpr
{
    public IExpr Max { get; set; }
}

public sealed record SetExpr : IExpr
{
    public List<IExpr> Elements { get; set; } = new();
}

public sealed record CallExpr : IExpr
{
    public Ident Name { get; set; }
    public List<IExpr> Args { get; set; } = new();
}

public sealed record RecordAccessExpr : IExpr
{
    public IExpr Expr { get; set; }
    public string Field { get; set; }
}

public sealed record TupleAccessExpr : IExpr
{
    public IExpr Expr { get; set; }
    public int Index { get; set; }
}

public sealed record ArrayAccessExpr : IExpr
{
    public IExpr Expr { get; set; }
    public List<IExpr> Slice { get; set; }
}

public sealed record BinaryOpExpr : IExpr
{
    public IExpr Left { get; set; }
    public Op Op { get; set; }
    public IExpr Right { get; set; }
}

public sealed record UnaryOpExpr : IExpr
{
    public Op UnOp { get; set; }
    public IExpr Expr { get; set; }
}

public sealed record GeneratorExpr : IExpr
{
    public List<Ident?> Yields { get; set; }

    public IExpr From { get; set; }

    public IExpr? Where { get; set; }
}

public sealed record GenCallExpr : IExpr
{
    public Ident Name { get; set; }

    public List<GeneratorExpr> From { get; set; }

    public IExpr Yields { get; set; }
}

public record CompExpr : IExpr
{
    public IExpr Yields { get; set; }
    public bool IsSet { get; set; }
    public List<GeneratorExpr> From { get; set; }
}

public sealed record ConstraintExpr : IExpr
{
    public IExpr Expr { get; set; }
    public Annotations Annotations { get; set; }
}

public sealed record TupleExpr : IExpr
{
    public List<IExpr> Exprs { get; set; } = new();
}

public sealed record RecordExpr : IExpr
{
    public Dictionary<Ident, IExpr> Exprs { get; set; } = new();
}

public sealed record ArrayExpr : IExpr
{
    public List<ArrayDim> Dims { get; set; }

    public int N => Dims.Count;

    public List<IExpr> Exprs { get; set; } = new();
}

public sealed record Array1dLiteral : IExpr
{
    public IExpr[] Elements { get; set; }
}

public sealed record Array2dLiteral : IExpr
{
    public IExpr[,] Elements { get; set; }
}

public sealed record Array3dLiteral : IExpr
{
    public IExpr[,,] Elements { get; set; }
}

public sealed record LetExpr : IExpr
{
    public List<TypeInst> Declares { get; set; } = new();

    public List<ConstraintExpr> Constraints { get; set; } = new();

    public IExpr Body { get; set; }
}

public sealed record IfThenElseExpr : IExpr
{
    public IExpr If { get; set; }

    public IExpr Then { get; set; }

    public List<(IExpr, IExpr)> ElseIf { get; set; } = new();

    public IExpr? Else { get; set; }
}

public sealed record OutputExpr : IExpr
{
    public IExpr Expr { get; set; }
    public Annotation? Annotation { get; set; }
}
