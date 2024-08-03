namespace MiniZinc.Parser.Syntax;

public abstract class SyntaxNode : IEquatable<SyntaxNode>
{
    /// <summary>
    /// The token at whic this node started
    /// </summary>
    public readonly Token Start;

    public List<ExpressionSyntax>? Annotations { get; set; } = null;

    protected SyntaxNode(in Token start)
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
        return Equals(obj as SyntaxNode);
    }

    public bool Equals(SyntaxNode? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        switch (this, other)
        {
            case (IntLiteralSyntax a, IntLiteralSyntax b):
                return a.Value == b.Value;
            case (BoolLiteralSyntax a, BoolLiteralSyntax b):
                return a.Value == b.Value;
            case (FloatLiteralSyntax a, FloatLiteralSyntax b):
                return a.Value == b.Value;
            case (StringLiteralSyntax a, StringLiteralSyntax b):
                return a.Value == b.Value;
            case (EmptyLiteralSyntax, EmptyLiteralSyntax):
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
                if (!a.Type.Equals(b.Type))
                    return false;
                if (!object.Equals(a.Body, b.Body))
                    return false;
                return true;
            case (SolveStatement a, SolveStatement b):
                if (a.Method != b.Method)
                    return false;
                if (!object.Equals(a.Objective, b.Objective))
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
