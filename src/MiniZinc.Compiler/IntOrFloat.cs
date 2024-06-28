namespace MiniZinc.Compiler;

/// <summary>
/// Union type that could be an int or float
/// </summary>
public readonly struct IntOrFloat
{
    public readonly bool IsFloat;
    public readonly int IntValue;
    public readonly float FloatValue;

    private IntOrFloat(int i = default, float f = default, bool isFloat = false)
    {
        IsFloat = isFloat;
        FloatValue = f;
        IntValue = i;
    }

    public static IntOrFloat Int(int i) => new IntOrFloat(i: i);

    public static IntOrFloat Float(float f) => new IntOrFloat(f: f, isFloat: true);
}
