namespace MiniZinc.Compiler;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Parser;
using Parser.Syntax;
using static MiniZinc.Parser.Parser;

/// <summary>
/// A MiniZinc model.
/// Instantiated as either an <see cref="IntModel"/> or a <see cref="FloatModel"/>
/// </summary>
/// <remarks>
/// This class extracts useful semantic information
/// from <see cref="ModelSyntax"/> and <see cref="DataSyntax"/>
/// </remarks>
public abstract class BaseModel<T>
    where T : BaseModel<T>, new()
{
    public string Name { get; set; } = "";

    private Dictionary<string, IIdentifiedSyntax> _namespace;

    private Dictionary<string, List<DeclareStatement>>? _overloads;

    private List<ConstraintStatement>? _constraints;

    private List<OutputStatement>? _outputs;

    private List<IncludeStatement>? _includes;

    private SolveStatement? _solve;

    private List<string>? _warnings;

    private List<DirectoryInfo> _searchDirectories;

    private List<FileInfo>? _addedFiles;

    private HashSet<string>? _parsedFiles;

    private bool _allowFloats;

    private bool _containsFloats;

    public IEnumerable<string> Warnings => _warnings ?? Enumerable.Empty<string>();

    public IEnumerable<ConstraintStatement> Constraints =>
        _constraints ?? Enumerable.Empty<ConstraintStatement>();

    public IEnumerable<OutputStatement> Outputs => _outputs ?? Enumerable.Empty<OutputStatement>();

    public IReadOnlyDictionary<string, IIdentifiedSyntax> NameSpace => _namespace;

    public SolveStatement? SolveStatement => _solve;

    public SolveMethod SolveMethod => _solve?.Method ?? SolveMethod.Satisfy;

    public ExpressionSyntax? Objective => _solve?.Objective;

    public IReadOnlyList<DirectoryInfo> SearchDirectories => _searchDirectories;

    public int WarningCount => _warnings?.Count ?? 0;

    public bool HasWarnings => _warnings is null;

    protected BaseModel(bool allowFloats = true)
    {
        _warnings = null;
        _outputs = null;
        _constraints = null;
        _namespace = new Dictionary<string, IIdentifiedSyntax>();
        _searchDirectories = new List<DirectoryInfo>();
        _allowFloats = allowFloats;
    }

    /// <summary>
    /// Declare a type alias
    /// </summary>
    ///<returns>The name of the type alias</returns>
    public string AddTypeAlias(string name, string type)
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
    public Variable AddParameter(string name, string type, string? value = null) =>
        Declare(name, type, value);

    /// <summary>
    /// Declare a variable
    /// </summary>
    /// <returns>
    /// The name of the declared parameter
    /// </returns>
    public Variable AddVariable(string name, string type, string? value = null) =>
        Declare(name, $"var {type}", value);

    /// <summary>
    /// Declare an integer variable
    /// </summary>
    /// <returns>
    /// The name of the declared variable
    /// </returns>
    public Variable AddInt(string name, string? value = null) => Declare(name, $"var int", value);

    /// <summary>
    /// Declare boolean variable
    /// </summary>
    /// <returns>
    /// The name of the declared variable
    /// </returns>
    public Variable AddBool(string name, bool value) =>
        Declare(name, $"var bool", value.ToString());

    /// <summary>
    /// Declare boolean variable
    /// </summary>
    /// <returns>
    /// The name of the declared variable
    /// </returns>
    public Variable AddBool(string name, string? value = null) => Declare(name, $"var bool", value);

    /// <summary>
    /// Declare float variable
    /// </summary>
    /// <returns>
    /// The name of the declared variable
    /// </returns>
    public Variable AddFloat(string name, string? value = null) =>
        Declare(name, $"var float", value);

    /// <summary>
    /// Declare an integer variable with the given bounds
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Bounds out of order
    /// </exception>
    /// <returns>
    /// The name of the declared variable
    /// </returns>
    public Variable AddInt(string name, int lowerBound, int upperBound, string? value = null)
    {
        if (lowerBound > upperBound)
            throw new ArgumentException();

        return Declare(name, $"var {lowerBound}..{upperBound}", value);
    }

    /// <summary>
    /// Declare a parameter, variable, or type alias
    /// </summary>
    /// <returns>The name of the declared variable</returns>
    public Variable Declare(string name, string type, string? value = null)
    {
        if (value != null)
            AddString($"{type}: {name} = {value};");
        else
            AddString($"{type}: {name};");

        return new Variable(name);
    }

    /// <summary>
    /// Assign a value to an expression
    /// </summary>
    public void Assign(string name, string expr)
    {
        AddString($"{name} = {expr};");
    }

    /// <summary>
    /// Add a constraint to the model with an optional name
    /// </summary>
    /// <returns>
    /// The constraint name if one was provided
    /// </returns>
    [return: NotNullIfNotNull(nameof(name))]
    public string? AddConstraint(ExpressionSyntax expr, string? name = null)
    {
        var con = new ConstraintStatement(default, expr);
        if (name is not null)
        {
            con.Annotations ??= new List<ExpressionSyntax>();
            con.Annotations.Add(new ExpressionSyntax<string>(name));
        }
        AddSyntax(con);
        return name;
    }

    /// <summary>
    /// Add a constraint to the model
    /// </summary>
    public void AddConstraint(ConstraintStatement constraint)
    {
        AddSyntax(constraint);
    }

    /// <summary>
    /// Add a constraint with an optional name
    /// </summary>
    /// <returns>
    /// The constraint name if one was provided
    /// </returns>
    public string? AddConstraint(string expr, string? name = null)
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
    public void Minimize(string objective, params string[] annotations)
    {
        var mzn = $"solve minimize {objective}{Annotations(annotations)};";
        var stm = ParseStatement<SolveStatement>(mzn);
        SetObjective(stm);
    }

    /// <summary>
    /// Set the model objective to maximize the given expression
    /// </summary>
    /// <example>model.Maximize("a+b")</example>
    /// <example>model.Maximize("makespan", "int_search(q, first_fail, indomain_min)")</example>
    public void Maximize(string objective, params string[] annotations)
    {
        var mzn = $"solve maximize {objective}{Annotations(annotations)};";
        var stm = ParseStatement<SolveStatement>(mzn);
        SetObjective(stm);
    }

    /// <summary>
    /// Set the model objective to find a satisfactory solution
    /// </summary>
    /// <example>model.Satisfy();</example>
    /// <example>model.Satisfy("int_search(q, first_fail, indomain_min)")</example>
    public void Satisfy(params string[] annotations)
    {
        var mzn = $"solve satisfy{Annotations(annotations)};";
        var stm = ParseStatement<SolveStatement>(mzn);
        SetObjective(stm);
    }

    /// <summary>
    /// Set the objective of the model to solve with the given
    /// syntax.  This will override any existing solve statement.
    /// <seealso cref="Maximize"/>
    /// <seealso cref="Minimize"/>
    /// <seealso cref="Satisfy"/>
    /// </summary>
    public void SetObjective(SolveStatement solve)
    {
        _solve = solve;
    }

    public void AddOutput(params string[] strings)
    {
        var mzn = $"output [{string.Join(',', strings)}]";
        var output = ParseStatement<OutputStatement>(mzn);
        AddSyntax(output);
    }

    public void ClearOutputs()
    {
        _outputs?.Clear();
    }

    /// <summary>
    /// Add the given syntax node to the model
    /// </summary>
    public void AddSyntax(SyntaxNode syntax)
    {
        switch (syntax)
        {
            case SolveStatement solve when _solve is null:
                _solve = solve;
                break;

            case SolveStatement solve:
                Error($"Can not override \"{_solve}\" with \"{syntax}\"");
                break;

            case OutputStatement output:
                _outputs ??= new List<OutputStatement>();
                _outputs.Add(output);
                break;

            case ConstraintStatement node:
                _constraints ??= new List<ConstraintStatement>();
                _constraints.Add(node);
                break;

            case TypeAliasSyntax alias:
                var name = alias.Identifier.Name;
                if (_namespace.TryGetValue(name, out var old))
                    Error($"Type alias name \"{name}\" conflicts with {old})");
                _namespace.Add(name, alias);
                break;

            case AssignmentSyntax assign:
                name = assign.Name;
                var expr = assign.Expr;
                _namespace.TryGetValue(name, out old);
                switch (old)
                {
                    // Undeclared variable
                    case null:
                        _namespace[name] = assign;
                        break;

                    // Assignment to a variable that has only been declared
                    case DeclareStatement { Body: null } declare:
                        /* The easiest way to store the fully assigned variable
                         * declaration here is to complete the intial declaration
                         * using this expression by parsing the string concatentat.
                         *
                         * eg:
                         * enum Dir;
                         * Dir = {A,B,C,D};
                         *
                         * becomes:
                         * enum Dir = {A, B, C, D};
                         */
                        var mzn = $"{declare.ToString()[..^1]} = {expr.ToString()};";
                        declare = ParseStatement<DeclareStatement>(mzn);
                        _namespace[name] = declare;
                        break;

                    default:
                        Error($"Reassigned variable {name} from {old} to {expr}");
                        break;
                }
                break;

            case DeclareStatement declare:
                name = declare.Name;
                _namespace.TryGetValue(name, out old);
                switch (old)
                {
                    case null:
                        _namespace[name] = declare;
                        break;

                    case AssignmentSyntax assign when declare.Body is null:
                        declare.Body = assign.Expr;
                        _namespace[name] = declare;
                        break;

                    case DeclareStatement oldDeclare:
                        var oldType = oldDeclare.Type?.ToString();
                        var newType = declare.Type?.ToString();
                        if (oldType != newType)
                            Error(
                                $"Function {name} was already declared with an incompatible return type ({oldType} vs {newType})"
                            );

                        _overloads ??= new Dictionary<string, List<DeclareStatement>>();
                        if (!_overloads.TryGetValue(name, out var overloads))
                        {
                            overloads = new List<DeclareStatement>();
                            _overloads[name] = overloads;
                        }
                        overloads.Add(declare);
                        break;

                    default:
                        Error($"Variable {name} was already declared as {old}");
                        break;
                }

                break;

            case IncludeStatement include:
                var path = include.Path.StringValue;
                var file = FindFile(path);

                /* If we couldn't find the included file then for now
                just assume it's a stdlib file and let minizinc sort
                it out */
                if (file is null)
                {
                    Warning(FileNotFoundMessage(path));
                    _includes ??= new List<IncludeStatement>();
                    _includes.Add(include);
                }
                else
                {
                    _parsedFiles ??= new HashSet<string>();
                    if (_parsedFiles.Contains(file.FullName))
                    {
                        // TODO - should we just ignore this?
                        Error($"Detected recursive include of \"{file.FullName}\"");
                    }
                    else
                    {
                        _parsedFiles.Add(file.FullName);
                        var result = ParseModelFile(file, out var model);
                        result.EnsureOk();
                        AddModel(model);
                    }
                }
                break;
        }
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

    /// <inheritdoc cref="AddFile(string)"/>
    public void AddFile(string path)
    {
        var file = FindFile(path);
        if (file is null)
            throw new FileNotFoundException(FileNotFoundMessage(path));

        var result = ParseModelFile(file, out var model);
        result.EnsureOk();

        // Added models become a search directory
        if (file.Directory is { } dir)
            AddSearchPath(dir);
        _addedFiles ??= new List<FileInfo>();
        _addedFiles.Add(file);
        AddModel(model);
    }

    public void AddModel(ModelSyntax model)
    {
        foreach (var statement in model.Statements)
            AddSyntax(statement);
    }

    /// <inheritdoc cref="AddFile(string)"/>
    public void AddFile(FileInfo file)
    {
        AddFile(file.FullName);
    }

    [DoesNotReturn]
    void Error(string msg)
    {
        throw new Exception(msg);
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
    public void AddFiles(params string[] paths)
    {
        foreach (var path in paths)
            AddFile(path);
    }

    /// <summary>
    /// Add an include statement for the given filepath
    /// </summary>
    public void Include(string path)
    {
        var syntax = ParseStatement<IncludeStatement>($"include \"{path}\"");
        AddSyntax(syntax);
    }

    public void AddString(string mzn)
    {
        var result = ParseModelString(mzn, out var model);
        result.EnsureOk();
        AddModel(model);
    }

    public void AddStrings(params string[] strings)
    {
        foreach (var mzn in strings)
            AddString(mzn);
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

    /// <summary>
    /// Return this model as a FloatModel.
    /// </summary>
    public FloatModel AsFloatModel()
    {
        var mzn = Write();
        var model = FloatModel.FromString(mzn);
        return model;
    }

    /// <summary>
    /// Return this model as an IntModel. An exception
    /// will be thrown if there were any floating point
    /// variables in the model.
    /// </summary>
    public IntModel AsIntModel()
    {
        var mzn = Write();
        var model = IntModel.FromString(mzn);
        return model;
    }

    public static T FromString(string mzn)
    {
        var model = new T();
        model.AddString(mzn);
        return model;
    }

    public static T FromFile(string path)
    {
        var model = new T();
        model.AddFile(path);
        return model;
    }

    public static T FromFile(FileInfo file)
    {
        var model = new T();
        model.AddFile(file);
        return model;
    }

    private string FileNotFoundMessage(string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Could not find \"{path}\" in any of the search directories:");
        foreach (var dir in _searchDirectories)
        {
            sb.AppendLine($"- {dir.FullName}");
        }

        var message = sb.ToString();
        return message;
    }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);

        if (_includes is { } includes)
            foreach (var include in includes)
                writer.WriteSyntax(include);

        foreach (var syntax in _namespace.Values)
            writer.WriteSyntax((SyntaxNode)syntax);

        if (_overloads is { } overloads)
            foreach (var overload in overloads)
            foreach (var syntax in overload.Value)
                writer.WriteSyntax(syntax);

        foreach (var constraint in Constraints)
            writer.WriteSyntax(constraint);

        if (_solve is not null)
            writer.WriteSyntax(_solve);

        // foreach (var output in Outputs)
        //     writer.WriteSyntax(output);

        var mzn = writer.ToString();
        return mzn;
    }

    public override string ToString()
    {
        var mzn = Write();
        return mzn;
    }
}
