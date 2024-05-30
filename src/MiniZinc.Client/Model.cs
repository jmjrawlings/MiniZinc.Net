namespace MiniZinc.Client;

using Parser;
using Parser.Syntax;

public sealed class Model
 {
     public SolveMethod SolveMethod { get; private set; }
     
     public SyntaxNode? Objective { get; private set; }
     
     public List<SyntaxNode> Outputs { get; private set; }
     
     public List<SyntaxNode> Constraints { get; private set; }
     
     public Dictionary<string, SyntaxNode> Assignments { get; private set; }
     
     public Dictionary<string, DeclarationSyntax> Bindings { get; private set; }
     
     /// <summary>
     /// Include the given file in the model
     /// </summary>
     public void Include(string path)
     {
         AddString($"include \"{path}\";");
     }
     
     /// <summary>
     /// Include the given file in the model
     /// </summary>
     public void Include(FileInfo file)
     {
         Include(file.FullName);
     }
     
     /// <summary>
     /// Declare a type alias 
     /// </summary>
     ///<returns>The name of the type alias</returns>
     public string TypeAlias(string name, string type)
     {
         AddString($"type {name} = {type};");
         return name;
     }
     
     /// <summary>
     /// Declare a parameter, variable, or type alias 
     /// </summary>
     /// <returns>The name of the declared variable</returns>
     public string Declare(string name, string type, string? value)
     {
         if (value != null)
            AddString($"{type}: {name} = {value};");
         else
            AddString($"{type}: {name};");
         
         return name;
     }
     
     /// <summary>
     /// Assign a value to an expression 
     /// </summary>
     public void Assign(string name, string expr)
     {
         AddString($"{name} = {expr};");
     }
     
     /// <summary>
     /// Add a constraint 
     /// </summary>
     public void Constraint(string expr, params string[] annotations)
     {
         if (annotations.Length is 0)
             AddString($"constraint {expr};");
         else
             AddString($"constraint {expr} " + Annotations(annotations) + ';');
     }
     
     private string Annotations(params string[] args) => string.Join("::", args);
     
     public void Minimize(string expr, params string[] annotations)
     {
         AddString($"solve minimize {expr}{Annotations(annotations)};");
     }
     
     public void Maximize(string expr, params string[] annotations)
     {
         AddString($"solve maximize {expr}{Annotations(annotations)};");
     }
     
     public void Satisfy(params string[] annotations)
     {
         AddString("solve satisfy;");
     }
     
     /// <summary>
    /// MiniZinc model (.mzn) or data (.dzn) files
    /// </summary>
    /// <example>"model.mzn"</example>
    /// <example>"data.dzn"</example>
    public IEnumerable<string> Files => _modelFiles ?? Enumerable.Empty<string>();
    
    /// <summary>
    /// MiniZinc models as strings
    /// </summary>
    /// <example>var 0..100: a;</example>
    public IEnumerable<string> Strings => _modelStrings ?? Enumerable.Empty<string>();
    
    public IEnumerable<string> Warnings => _warnings ?? Enumerable.Empty<string>();
    
    public IEnumerable<string> Errors => _errors ?? Enumerable.Empty<string>();
    
    private List<string>? _modelFiles;
    private List<string>? _modelStrings;
    private List<string>? _warnings;
    private List<string>? _errors;
    
    private Model(IEnumerable<string>? files = null, IEnumerable<string>? strings = null)
    {
        _modelFiles = files?.ToList();
        _modelStrings = strings?.ToList();
        _errors = null;
        _warnings = null;
        Outputs = new List<SyntaxNode>();
        Constraints = new List<SyntaxNode>();
        SolveMethod = SolveMethod.Satisfy;
        Objective = null;
        Compile();
    }
    
    /// <summary>
    /// Compile this model from the files and strings 
    /// </summary>
    void Compile()
    {
        SolveMethod = SolveMethod.Satisfy;
        Objective = null;
        Outputs.Clear();
        Constraints.Clear();
    }
    
    public static Model Create() => new Model();

    public static Model FromFile(string path)
    {
        var model = new Model();
        model.AddFile(path);
        return model;
    }

    public static Model FromString(string path)
    {
        var model = new Model();
        model.AddString(path);
        return model;
    }

    public Model AddFile(string path)
    {
        _modelFiles ??= new List<string>();
        _modelFiles.Add(path);
        return this;
    }

    public Model AddFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths)
            AddFile(path);
        return this;
    }

    public Model AddString(string mzn)
    {
        _modelStrings ??= new List<string>();
        _modelStrings.Add(mzn);
        return this;
    }

    public Model AddStrings(params string[] strings)
    {
        foreach (var mzn in strings)
            AddString(mzn);
        return this;
    }

    public Model Copy()
    {
        var model = new Model(files: _modelFiles, strings: _modelStrings);
        return model;
    }

    public override string ToString()
    {
        switch (_modelFiles, _modelStrings)
        {
            case (null, null):
                return "Empty Model";
            case (var files, null):
                if (files.Count is 1)
                    return $"Model \"{files[0]}\"";
                else
                    return $"Model with {files.Count} files";
            case (null, var strings):
                return $"Model with {strings.Count} strings";
            case var (files, strings):
                return $"Model with {files.Count} files and {strings.Count} strings";
        }
    }
}