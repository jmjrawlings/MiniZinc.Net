namespace MiniZinc.Build;

public static class Projects
{
    public static Project MiniZinc => Lookup("MiniZinc.Net");
    public static Project Parser => Lookup("MiniZinc.Parser");
    public static Project Build => Lookup("Build");
    public static Project ParserTests => Lookup("ParserTests");
    public static Project ProcessTests => Lookup("ProcessTests");
    private static readonly Dictionary<string, Project> _cache = new();

    public static Project Lookup(string name)
    {
        if (_cache.TryGetValue(name, out var proj))
            return proj;

        var csproj = $"{name}.csproj";
        var file = Repo.SolutionDir.EnumerateFiles(csproj, SearchOption.AllDirectories).First();
        var dir = file.Directory!;
        proj = new Project
        {
            Name = name,
            File = file,
            Dir = dir
        };
        _cache[name] = proj;
        return proj;
    }
}
