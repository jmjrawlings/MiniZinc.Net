namespace MiniZinc.Compiler;

/// <summary>
/// A <see cref="Model"/> that supports floating point variables.
/// For models known to contain only integer variables use the <see cref="IntModel"/>
/// class.
/// </summary>
public sealed class FloatModel : Model
{
    public FloatModel()
        : base(allowFloats: true) { }

    /// <summary>
    /// Create a new model from the given filepath
    /// </summary>
    public new static FloatModel FromFile(string path)
    {
        var model = new FloatModel();
        model.AddFile(path);
        return model;
    }

    /// <summary>
    /// Create a new model from the given file
    /// </summary>
    public new static FloatModel FromFile(FileInfo file) => FromFile(file.FullName);

    /// <summary>
    /// Create a new model from the given string
    /// </summary>
    public new static FloatModel FromString(string path)
    {
        var model = new FloatModel();
        model.AddString(path);
        return model;
    }
}
