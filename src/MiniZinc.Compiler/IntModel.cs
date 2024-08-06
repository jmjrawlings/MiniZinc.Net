namespace MiniZinc.Compiler;

/// <summary>
/// A model that contains only integer variables.
/// For models with floating point variables use <see cref="FloatModel"/>.
/// </summary>
public sealed class IntModel : MiniZincModel<IntModel>
{
    public IntModel()
        : base(allowFloats: false) { }
}
