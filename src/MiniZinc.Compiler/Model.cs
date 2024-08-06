namespace MiniZinc.Compiler;

/// <summary>
/// A model that could contain floating point variables.
/// For models known to contain only integer variables use the <see cref="IntModel"/>
/// class or call <see cref="ToIntModel()"/>
/// For models known to contain floating point variables use the <see cref="FloatModel"/>
/// class or call <see cref="ToFloatModel()"/>.
/// </summary>
public sealed class Model : MiniZincModel<Model>
{
    public Model()
        : base(allowFloats: true) { }
}
