namespace MiniZinc.Parser;

/// <summary>
/// A reference to a value of type T.
/// It can either by the value itself, or a name
/// that refers to it
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Ref<T>
{
    public string? Name { get; }
    public T Value { get; }

    public Ref(string? name = null, T? value = default)
    {
        Name = name;
        Value = value;
    }
}

public static class Ref
{
    public static Ref<T> OfName<T>(string id) => new(name: id);

    public static Ref<T> OfValue<T>(T value) => new(value: value);
}
