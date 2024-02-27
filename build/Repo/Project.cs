namespace MiniZinc.Build;

public sealed class Project
{
    public required string Name { get; init; }
    public required DirectoryInfo Dir { get; init; }
    public required FileInfo File { get; init; }
}
