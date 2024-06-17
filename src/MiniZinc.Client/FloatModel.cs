namespace MiniZinc.Client;

/// <summary>
/// A <see cref="MiniZincModel"/> that contains
/// floating point variables.  For models that contain
/// only integer variables use the <see cref="IntModel"/>
/// class.
/// </summary>
public sealed class FloatModel : MiniZincModel
{
    public override bool IsFloatModel => true;

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
