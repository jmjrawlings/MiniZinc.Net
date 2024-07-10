namespace MiniZinc.Compiler;

/// <summary>
/// A <see cref="BaseModel"/> that supports floating point variables.
/// For models known to contain only integer variables use the <see cref="IntModel"/>
/// class.
/// </summary>
public sealed class FloatModel : BaseModel<FloatModel>
{
    public FloatModel()
        : base(allowFloats: true) { }
}
