namespace MiniZinc.Models;

/// <summary>
/// A <see cref="Model"/> that contains only integer variables.
/// For models with floating point variables use <see cref="FloatModel"/>.
/// </summary>
public sealed class IntModel : Model
{
    public IntModel()
        : base(allowFloats: false) { }

    /// <summary>
    /// Create a new model from the given filepath
    /// </summary>
    public new static IntModel FromFile(string path)
    {
        var model = new IntModel();
        model.AddFile(path);
        return model;
    }

    /// <summary>
    /// Create a new model from the given file
    /// </summary>
    public new static IntModel FromFile(FileInfo file) => FromFile(file.FullName);

    /// <summary>
    /// Create a new model from the given string
    /// </summary>
    public new static IntModel FromString(string path)
    {
        var model = new IntModel();
        model.AddString(path);
        return model;
    }
}
