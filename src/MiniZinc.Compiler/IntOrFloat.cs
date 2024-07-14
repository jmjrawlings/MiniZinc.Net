namespace MiniZinc.Compiler;

/// <summary>
/// Union type that could be an int or float
/// </summary>
public readonly struct IntOrFloat
{
    public readonly bool IsFloat;
    public readonly int IntValue;
    public readonly decimal DecimalValue;

    private IntOrFloat(int i = default, decimal d = default, bool isFloat = false)
    {
        IsFloat = isFloat;
        DecimalValue = d;
        IntValue = i;
    }

    public static implicit operator IntOrFloat(int i) => new IntOrFloat(i: i);

    public static implicit operator IntOrFloat(decimal d) => new IntOrFloat(d: d);

    public static IntOrFloat Int(int i) => new IntOrFloat(i: i);

    public static IntOrFloat Float(decimal d) => new IntOrFloat(d: d, isFloat: true);
}
