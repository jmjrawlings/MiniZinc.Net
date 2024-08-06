namespace MiniZinc.Compiler;

/// <summary>
/// A <see cref="MiniZincModel{T}"/> that supports floating point variables.
/// For models known to contain only integer variables use the <see cref="IntModel"/>
/// class.
/// </summary>
public sealed class FloatModel : MiniZincModel<FloatModel>
{
    public FloatModel()
        : base(allowFloats: true) { }
}
