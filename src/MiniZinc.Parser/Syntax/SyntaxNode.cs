﻿namespace MiniZinc.Parser.Syntax;

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

    public string SourceText => Write(WriteOptions.Minimal);

    /// <summary>
    /// Write this node as a minizinc string
    /// </summary>
    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteNode(this);
        var mzn = writer.ToString();
        return mzn;
    }

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
            case (WildCardSyntax, WildCardSyntax):
                return true;
            case (EmptyLiteralSyntax, EmptyLiteralSyntax):
                return true;
            case (ConstraintSyntax a, ConstraintSyntax b):
                return a.Expr.Equals(b.Expr);
            case (IncludeSyntax a, IncludeSyntax b):
                return a.Path.Equals(b.Path);
            case (AssignmentSyntax a, AssignmentSyntax b):
                if (!a.Identifier.Equals(b.Identifier))
                    return false;
                if (!a.Expr.Equals(b.Expr))
                    return false;
                return true;
            case (DeclarationSyntax a, DeclarationSyntax b):
                if (!a.Identifier.Equals(b.Identifier))
                    return false;
                if (!a.Type.Equals(b.Type))
                    return false;
                if (!object.Equals(a.Body, b.Body))
                    return false;
                return true;
            case (SolveSyntax a, SolveSyntax b):
                if (a.Method != b.Method)
                    return false;
                if (!object.Equals(a.Objective, b.Objective))
                    return false;
                return true;
            case (TypeAliasSyntax a, TypeAliasSyntax b):
                if (!a.Identifier.Equals(b.Identifier))
                    return false;
                if (!a.Type.Equals(b.Type))
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
