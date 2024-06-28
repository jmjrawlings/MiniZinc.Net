namespace MiniZinc.Compiler;

using Parser;
using Parser.Syntax;

/// <summary>
/// Wraps a minizinc variable name.
/// Allows expressions to be created with
/// operator overloading.
/// </summary>
public readonly struct Variable
{
    public readonly string Name;

    public Variable(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Variables cannot be empty strings");

        // TODO - handle quotes
        Name = name;
    }

    public static BinaryOperatorSyntax operator +(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} + {b}");

    public static BinaryOperatorSyntax operator -(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} - {b}");

    public static BinaryOperatorSyntax operator *(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} * {b}");

    public static BinaryOperatorSyntax operator /(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} / {b}");

    public static BinaryOperatorSyntax operator %(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} % {b}");

    public static BinaryOperatorSyntax operator |(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} \\/ {b}");

    public static BinaryOperatorSyntax operator &(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} /\\ {b}");

    public static UnaryOperatorSyntax operator !(Variable a) =>
        Parser.ParseExpression<UnaryOperatorSyntax>($"not {a}");

    public static BinaryOperatorSyntax operator ==(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} == {b}");

    public static BinaryOperatorSyntax operator !=(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} != {b}");

    public static BinaryOperatorSyntax operator <(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} < {b}");

    public static BinaryOperatorSyntax operator >(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} > {b}");

    public static BinaryOperatorSyntax operator <=(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} <= {b}");

    public static BinaryOperatorSyntax operator >=(in Variable a, in Variable b) =>
        Parser.ParseExpression<BinaryOperatorSyntax>($"{a} >= {b}");

    public static implicit operator string(in Variable v) => v.Name;

    public static implicit operator Variable(string s) => new Variable(s);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is Variable v)
            return Name.Equals(v.Name);

        if (obj is string s)
            return Name.Equals(s);

        return false;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => Name;
}
