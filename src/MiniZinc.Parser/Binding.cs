namespace MiniZinc.Parser;

using Syntax;

public interface IBinding<out T>
{
    string Name { get; }
    T Value { get; }
}

/// <summary>
/// A binding of a value to a name
/// </summary>
public readonly record struct Binding<T> : IBinding<T>, ILetLocalSyntax
{
    /// <summary>
    /// A binding of a value to a name
    /// </summary>
    public Binding(string Name, T Value)
    {
        this.Name = Name;
        this.Value = Value;
    }

    public string Name { get; init; }
    public T Value { get; init; }

    public void Deconstruct(out string Name, out T Value)
    {
        Name = this.Name;
        Value = this.Value;
    }
}

public static class BindingExtensions
{
    public static Binding<T> Bind<T>(this string name, T value) => new Binding<T>(name, value);
}
