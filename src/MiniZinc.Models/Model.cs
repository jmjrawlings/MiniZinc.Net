namespace MiniZinc.Models;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Parser;
using Parser.Syntax;

/// <summary>
/// A MiniZinc model.  Instantiated as either an
/// <see cref="IntModel"/> or a <see cref="FloatModel"/>
/// </summary>
/// <remarks>
/// This class extracts useful semantic information
/// from <see cref="SyntaxTree"/>
/// </remarks>
public class Model
{
    public string Name { get; set; } = "";

    private Dictionary<string, IIdentifiedSyntax> _namespace;

    private HashSet<string>? _variables;

    private List<ConstraintSyntax>? _constraints;

    private List<OutputSyntax>? _outputs;

    private SolveSyntax? _solve;

    private List<string>? _warnings;

    private List<string>? _errors;

    private StringBuilder _sourceText;

    private List<DirectoryInfo> _searchDirectories;

    private HashSet<string>? _parsedFiles;

    private bool _allowFloats;

    private bool _isFloatModel;

    public string SourceText => _sourceText.ToString();

    public bool IsFloatModel => _isFloatModel;

    public IEnumerable<string> Warnings => _warnings ?? Enumerable.Empty<string>();

    public IEnumerable<string> Errors => _errors ?? Enumerable.Empty<string>();

    public IEnumerable<ConstraintSyntax> Constraints =>
        _constraints ?? Enumerable.Empty<ConstraintSyntax>();

    public IEnumerable<OutputSyntax> Outputs => _outputs ?? Enumerable.Empty<OutputSyntax>();

    public SolveSyntax? Solve => _solve;

    public SolveMethod Method => _solve?.Method ?? SolveMethod.Satisfy;

    public SyntaxNode? Objective => _solve?.Objective;

    public IReadOnlyList<DirectoryInfo> SearchDirectories => _searchDirectories;

    public int WarningCount => _warnings?.Count ?? 0;

    public int ErrorCount => _errors?.Count ?? 0;

    public bool HasErrors => _errors is null;

    public bool HasWarnings => _warnings is null;

    public Model(bool allowFloats = true)
    {
        _errors = null;
        _warnings = null;
        _outputs = null;
        _constraints = null;
        _variables = new HashSet<string>();
        _namespace = new Dictionary<string, IIdentifiedSyntax>();
        _sourceText = new StringBuilder();
        _searchDirectories = new List<DirectoryInfo>();
        _allowFloats = allowFloats;
    }

    /// <summary>
    /// Include the given file in the model
    /// </summary>
    /// <example>model.Include("data.dzn");</example>
    public void Include(string filepath)
    {
        AddString($"include \"{filepath}\";");
    }

    /// <inheritdoc cref="Include(string)"/>
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
    /// Declare a parameter
    /// </summary>
    /// <returns>
    /// The name of the declared parameter
    /// </returns>
    public string Par(string name, string type, string? value = null) => Declare(name, type, value);

    /// <summary>
    /// Declare a variable
    /// </summary>
    /// <returns>
    /// The name of the declared parameter
    /// </returns>
    public string Var(string name, string type, string? value = null) =>
        Declare(name, $"var {type}", value);

    /// <summary>
    /// Declare a parameter, variable, or type alias
    /// </summary>
    /// <returns>The name of the declared variable</returns>
    public string Declare(string name, string type, string? value = null)
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
    /// Add a constraint with an optional name
    /// </summary>
    /// <returns>
    /// The constraint name if one was provided
    /// </returns>
    [return: NotNullIfNotNull(nameof(name))]
    public string? Constraint(string expr, string? name = null)
    {
        if (name is null)
        {
            AddString($"constraint {expr};");
            return null;
        }
        else
        {
            AddString($"constraint {expr} :: \"{name}\"");
            return name;
        }
    }

    private static string? Annotations(params string[] args) =>
        args.Length == 0 ? null : string.Join("::", args);

    /// <summary>
    /// Set a model objective to minimize the given expression
    /// </summary>
    /// <example>model.Minimize("a+b")</example>
    /// <example>model.Minimize("makespan", "int_search(q, first_fail, indomain_min)")</example>
    public void Minimize(string expr, params string[] annotations)
    {
        AddString($"solve minimize {expr}{Annotations(annotations)};");
    }

    /// <summary>
    /// Set the model objective to maximize the given expression
    /// </summary>
    /// <example>model.Maximize("a+b")</example>
    /// <example>model.Maximize("makespan", "int_search(q, first_fail, indomain_min)")</example>
    public void Maximize(string expr, params string[] annotations)
    {
        AddString($"solve maximize {expr}{Annotations(annotations)};");
    }

    /// <summary>
    /// Set the model objective to find a satisfactory solution
    /// </summary>
    /// <example>model.Satisfy();</example>
    /// <example>model.Satisfy("int_search(q, first_fail, indomain_min)")</example>
    public void Satisfy(params string[] annotations)
    {
        AddString($"solve satisfy{Annotations(annotations)};");
    }

    void AddSourceText(string text)
    {
        _sourceText.AppendLine(text);
    }

    void AddSourceText(SyntaxNode node)
    {
        var mzn = node.Write();
        _sourceText.AppendLine(mzn);
    }

    /// <summary>
    /// Add the given syntax node to the model
    /// </summary>
    void AddNode(SyntaxNode syntaxNode)
    {
        switch (syntaxNode)
        {
            case SolveSyntax node:
                if (_solve is null)
                {
                    _solve = node;
                    AddSourceText(node);
                }
                else
                {
                    Error($"Can not override \"{_solve}\" with \"{syntaxNode}\"");
                }
                break;

            case OutputSyntax node:
                _outputs ??= new List<OutputSyntax>();
                _outputs.Add(node);
                // AddSourceText(node);
                break;

            case ConstraintSyntax node:
                _constraints ??= new List<ConstraintSyntax>();
                _constraints.Add(node);
                AddSourceText(node);
                break;

            case AssignmentSyntax node:
                var name = node.Identifier.ToString();
                var expr = node.Expr;
                if (_namespace.TryGetValue(name, out var old))
                {
                    switch (old)
                    {
                        // Unassigned variable
                        case DeclarationSyntax { Body: null } syntax:
                            var copy = syntax.Clone();
                            copy.Body = expr;
                            _namespace[name] = copy;
                            break;

                        default:
                            Error($"Reassigned variable {name} from {old} to {expr}");
                            break;
                    }
                }
                else
                {
                    _namespace[name] = node;
                }
                AddSourceText(node);
                break;

            case DeclarationSyntax node:
                name = node.Identifier.ToString();
                if (_namespace.TryGetValue(name, out old))
                    Error($"Variable {name} was already declared as {node.Type.SourceText}");
                else
                    _namespace[name] = node;

                AddSourceText(node);
                break;

            case EnumDeclarationSyntax node:
                name = node.Identifier.ToString();
                if (_namespace.TryGetValue(name, out old))
                    Error($"Variable {name} was already declared as an Enumeration");
                else
                    _namespace[name] = node;

                AddSourceText(node);
                break;

            case FunctionDeclarationSyntax node:
                name = node.Name;
                if (_namespace.TryGetValue(name, out old))
                    Error($"Function {name} was already declared as {old}");
                else
                    _namespace[name] = node;
                AddSourceText(node);
                break;

            case TypeAliasSyntax node:
                name = node.Name;
                if (_namespace.TryGetValue(name, out old))
                    Error($"TypeAlias {name} was already declared as {old}");
                else
                    _namespace[name] = node;
                AddSourceText(node);
                break;

            case IncludeSyntax node:
                var path = node.Path.StringValue;
                var file = FindFile(path);

                /* If we couldn't find the included file then for now
                just assume it's a stdlib file and let minizinc sort
                it out */
                if (file is null)
                {
                    var msg = CreateFileNotFoundMessage(path);
                    Warning(msg);
                    AddSourceText(node);
                }
                else
                {
                    _parsedFiles ??= new HashSet<string>();
                    if (_parsedFiles.Contains(file.FullName))
                    {
                        Error($"Detected recursive include of \"{file.FullName}\"");
                    }
                    else
                    {
                        _parsedFiles.Add(file.FullName);
                        var result = Parser.ParseFile(file);
                        result.EnsureOk();
                        AddNode(result.SyntaxNode);
                    }
                }
                break;

            case SyntaxTree tree:
                foreach (var node in tree.Nodes)
                    AddNode(node);
                break;
        }
    }

    /// <summary>
    /// Create a new model from the given filepath
    /// </summary>
    public static Model FromFile(string path)
    {
        var model = new Model();
        model.AddFile(path);
        return model;
    }

    /// <summary>
    /// Create a new model from the given file
    /// </summary>
    public static Model FromFile(FileInfo file) => FromFile(file.FullName);

    /// <summary>
    /// Create a new model from the given string
    /// </summary>
    public static Model FromString(string path)
    {
        var model = new Model();
        model.AddString(path);
        return model;
    }

    /// <summary>
    /// Find the file at it's given location, or search through
    /// the registered search directories.
    /// </summary>
    /// <param name="path"></param>
    FileInfo? FindFile(string path)
    {
        var file = new FileInfo(path);
        if (file.Exists)
            return file;

        int i = 0;
        while (i < _searchDirectories.Count)
        {
            var dir = _searchDirectories[i++];
            file = new FileInfo(Path.Join(dir.FullName, path));
            if (file.Exists)
                return file;
        }

        return null;
    }

    private string CreateFileNotFoundMessage(string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Could not find \"{path}\" in any of the search directories:");
        foreach (var dir in _searchDirectories)
        {
            sb.AppendLine($"- {dir.FullName}");
        }

        var error = sb.ToString();
        return error;
    }

    /// <summary>
    /// Add the given file to this model (.mzn or .dzn)
    /// </summary>
    /// <example>model.AddFile("~/models/model.mzn");</example>
    /// <example>model.AddFile("C://models//scenario1.dzn");</example>
    public void AddFile(FileInfo file) => AddFile(file.FullName);

    /// <inheritdoc cref="AddFile(System.IO.FileInfo)"/>
    public void AddFile(string path)
    {
        // TODO - use sets for search paths?
        var file = FindFile(path);
        if (file is null)
        {
            var message = CreateFileNotFoundMessage(path);
            throw new FileNotFoundException(message, path);
        }
        var result = Parser.ParseFile(file);
        result.EnsureOk();

        // Added models become a search directory
        if (file.Directory is { } dir)
            AddSearchPath(dir);
        AddNode(result.SyntaxNode);
    }

    /// <summary>
    /// Add the given error to this model
    /// </summary>
    void Error(string msg)
    {
        _errors ??= new List<string>();
        _errors.Add(msg);
    }

    /// <summary>
    /// Add the given warning to this model
    /// </summary>
    void Warning(string msg)
    {
        _warnings ??= new List<string>();
        _warnings.Add(msg);
    }

    /// <summary>
    /// Add the given files to this model (.mzn or .dzn)
    /// </summary>
    public Model AddFiles(params string[] paths)
    {
        foreach (var path in paths)
            AddFile(path);
        return this;
    }

    public void AddString(string mzn)
    {
        var result = Parser.ParseString(mzn);
        result.EnsureOk();
        AddNode(result.SyntaxNode);
    }

    public Model AddStrings(params string[] strings)
    {
        foreach (var mzn in strings)
            AddString(mzn);
        return this;
    }

    /// <summary>
    /// Add the directory as a place to search for models
    /// referenced by the minizinc `include` statement.
    /// </summary>
    public void AddSearchPath(DirectoryInfo directory)
    {
        _searchDirectories.Add(directory);
    }

    /// <inheritdoc cref="AddSearchPath(System.IO.DirectoryInfo)"/>
    public void AddSearchPath(string directory) => AddSearchPath(new DirectoryInfo(directory));

    public Model Clone()
    {
        var copy = FromString(SourceText);
        return copy;
    }

    /// <summary>
    /// Return this model as a FloatModel.
    /// </summary>
    public FloatModel AsFloatModel()
    {
        var model = FloatModel.FromString(SourceText);
        return model;
    }

    /// <summary>
    /// Return this model as an IntModel. An exception
    /// will be thrown if there were any floating point
    /// variables in the model.
    /// </summary>
    public IntModel AsIntModel()
    {
        var model = IntModel.FromString(SourceText);
        return model;
    }

    public void EnsureOk()
    {
        if (_errors is null)
            return;

        var msg = new StringBuilder();
        msg.AppendLine($"The mode has {_errors.Count} errors:");
        foreach (var err in _errors)
        {
            msg.AppendLine("-------------------------------------");
            msg.AppendLine(err);
            msg.AppendLine();
        }
    }

    public override string ToString()
    {
        return SourceText;
    }
}
