namespace MiniZinc.Parser;

/// <summary>
/// A binding of a value to a name
/// </summary>
public readonly record struct Binding<T>(string Name, T Value) { }

public static class BindingExtensions
{
    public static Binding<T> Bind<T>(this string name, T value) => new Binding<T>(name, value);
}
