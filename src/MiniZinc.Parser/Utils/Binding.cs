namespace MiniZinc.Parser;

using MiniZinc.Parser.Ast;

public interface IBinding<out T>
{
    string Name { get; }
    T Value { get; }
}

/// <summary>
/// A binding of a value to a name
/// </summary>
public readonly record struct Binding<T>(string Name, T Value) : IBinding<T>, ILetLocal { }



public static class BindingExtensions
{
    public static Binding<T> Bind<T>(this string name, T value) => new Binding<T>(name, value);

    public static Binding<T> ToBinding<T>(this T item)
        where T : INamed
    {
        var bnd = new Binding<T>(item.Name, item);
        return bnd;
    }
}
