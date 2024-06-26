namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An expression lives inside statements and other expressions.
/// </summary>
public abstract class ExpressionSyntax : SyntaxNode
{
    /// <summary>
    /// An expression lives inside statements and other expressions.
    /// </summary>
    protected ExpressionSyntax(Token start)
        : base(start) { }
}

public class ExpressionSyntax<T> : ExpressionSyntax
{
    private readonly T _value;

    protected ExpressionSyntax(in Token start, T value)
        : base(start)
    {
        _value = value;
    }

    public ExpressionSyntax(T value)
        : base(default)
    {
        _value = value;
    }

    public T Value => _value;
}
