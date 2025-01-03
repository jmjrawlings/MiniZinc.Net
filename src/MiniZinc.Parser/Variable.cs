namespace MiniZinc;

using Parser;
using static Parser.Parser;

/// <summary>
/// Wraps a minizinc variable name.
/// Allows expressions to be created with
/// operator overloading.
/// </summary>
public readonly struct Variable
{
    public readonly string Name;

    public Variable(ReadOnlySpan<char> name)
    {
        Name = name.ToString();
        if (string.IsNullOrEmpty(Name))
            throw new ArgumentException("Variables cannot be empty strings");
    }

    public static BinOpExpr operator +(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} + {b}");

    public static BinOpExpr operator -(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} - {b}");

    public static BinOpExpr operator *(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} * {b}");

    public static BinOpExpr operator /(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} / {b}");

    public static BinOpExpr operator %(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} % {b}");

    public static BinOpExpr operator |(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} \\/ {b}");

    public static BinOpExpr operator &(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} /\\ {b}");

    public static UnOpExpr operator !(Variable a) => ParseExpression<UnOpExpr>($"not {a}");

    public static BinOpExpr operator ==(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} == {b}");

    public static BinOpExpr operator !=(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} != {b}");

    public static BinOpExpr operator <(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} < {b}");

    public static BinOpExpr operator >(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} > {b}");

    public static BinOpExpr operator <=(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} <= {b}");

    public static BinOpExpr operator >=(in Variable a, in Variable b) =>
        ParseExpression<BinOpExpr>($"{a} >= {b}");

    public static implicit operator string(in Variable v) => v.Name;

    public static implicit operator Variable(string s) => new Variable(s);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is Variable v)
            if (!Name.Equals(v.Name))
                return false;

        if (!Name.Equals(obj.ToString()))
            return false;

        return true;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => Name;
}
