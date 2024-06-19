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
public readonly record struct Binding<T>(string Name, T Value) : IBinding<T>, ILetLocalSyntax { }

public static class BindingExtensions
{
    public static Binding<T> Bind<T>(this string name, T value) => new Binding<T>(name, value);
}
