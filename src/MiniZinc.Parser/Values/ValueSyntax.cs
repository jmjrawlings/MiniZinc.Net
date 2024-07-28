namespace MiniZinc.Parser;

using Syntax;

public abstract class ValueSyntax : ExpressionSyntax
{
    protected ValueSyntax(Token start)
        : base(start) { }
}

public class ValueSyntax<T> : ValueSyntax
{
    private readonly T _value;

    protected ValueSyntax(in Token start, T value)
        : base(start)
    {
        _value = value;
    }

    public ValueSyntax(T value)
        : base(default)
    {
        _value = value;
    }

    public T Value => _value;
}
