namespace MiniZinc.Parser.Syntax;

public abstract class Syntax : IEquatable<Syntax>
{
    /// The token at which this node started
    public readonly Token Start;

    /// Any annotations added to this node
    public List<Expr>? Annotations;

    protected Syntax(in Token start)
    {
        Start = start;
    }

    public string SourceText => this.Write(WriteOptions.Minimal);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return Equals(obj as Syntax);
    }

    public override int GetHashCode() => SourceText.GetHashCode();

    public bool Equals(Syntax? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        switch (this, other)
        {
            case (IntExpr a, IntExpr b):
                return a.Value == b.Value;
            case (BoolExpr a, BoolExpr b):
                return a.Value == b.Value;
            case (FloatExpr a, FloatExpr b):
                return a.Value == b.Value;
            case (StringExpr a, StringExpr b):
                return a.Value == b.Value;
            case (EmptyExpr, EmptyExpr):
                return true;
            case (ConstraintStatement a, ConstraintStatement b):
                return a.Expr.Equals(b.Expr);
            case (IncludeStatement a, IncludeStatement b):
                return a.Path.Equals(b.Path);
            case (AssignStatement a, AssignStatement b):
                if (!a.Name.Equals(b.Name))
                    return false;
                if (!a.Expr.Equals(b.Expr))
                    return false;
                return true;
            case (DeclareStatement a, DeclareStatement b):
                if (!a.Name.Equals(b.Name))
                    return false;
                if (!Equals(a.Type, b.Type))
                    return false;
                if (!Equals(a.Body, b.Body))
                    return false;
                return true;
            case (SolveStatement a, SolveStatement b):
                if (a.Method != b.Method)
                    return false;
                if (!Equals(a.Objective, b.Objective))
                    return false;
                return true;
            default:
                // TODO - rest
                var mzn_a = this.Write(WriteOptions.Minimal);
                var mzn_b = other.Write(WriteOptions.Minimal);
                if (mzn_a != mzn_b)
                    return false;
                return true;
        }
    }

    public override string ToString() => SourceText;
}
