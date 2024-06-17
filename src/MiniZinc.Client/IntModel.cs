namespace MiniZinc.Client;

/// <summary>
/// A <see cref="MiniZincModel"/> that contains
/// only integer variables.  For models containing
/// floating point variables use the <see cref="FloatModel"/>
/// class.
/// </summary>
public sealed class IntModel : MiniZincModel
{
    public override bool IsFloatModel => false;

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
