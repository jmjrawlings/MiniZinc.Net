namespace MiniZinc;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Parser;
using Parser.Syntax;
using static Parser.Parser;
using static Parser.SolveMethod;

/// <summary>
/// A MiniZinc model.
/// </summary>
/// <remarks>
/// This class extracts useful semantic information
/// from <see cref="ModelSyntax"/> and <see cref="MiniZincData"/>
/// </remarks>
public sealed class MiniZincModel
{
    public string Name { get; set; } = "";

    private Dictionary<string, INamedSyntax> _namespace;

    private Dictionary<string, List<DeclareStatement>>? _overloads;

    private List<ConstraintStatement>? _constraints;

    private List<OutputStatement>? _outputs;

    private List<IncludeStatement>? _includes;

    private SolveStatement? _solve;

    private List<string>? _warnings;

    private List<DirectoryInfo> _searchDirectories;

    private List<FileInfo>? _addedFiles;

    private HashSet<string>? _parsedFiles;

    // private bool _containsFloats;

    public IEnumerable<string> Warnings => _warnings ?? Enumerable.Empty<string>();

    public IEnumerable<ConstraintStatement> Constraints =>
        _constraints ?? Enumerable.Empty<ConstraintStatement>();

    public IEnumerable<OutputStatement> Outputs => _outputs ?? Enumerable.Empty<OutputStatement>();

    public IReadOnlyDictionary<string, INamedSyntax> NameSpace => _namespace;

    public SolveStatement? SolveStatement => _solve;

    public SolveMethod SolveMethod => _solve?.Method ?? SOLVE_SATISFY;

    public ExpressionSyntax? Objective => _solve?.Objective;

    public IReadOnlyList<DirectoryInfo> SearchDirectories => _searchDirectories;

    public int WarningCount => _warnings?.Count ?? 0;

    public bool HasWarnings => _warnings is null;

    public MiniZincModel()
    {
        _warnings = null;
        _outputs = null;
        _constraints = null;
        _includes = null;
        _namespace = [];
        _searchDirectories = [];
        _overloads = null;
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
    public Variable AddBool(ReadOnlySpan<char> name, bool value) =>
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
    public Variable Declare(ReadOnlySpan<char> name, string type, string? value = null)
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
            con.Annotations ??= [];
            con.Annotations.Add(new StringLiteralSyntax(default, name));
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
    public SolveStatement Minimize(ReadOnlySpan<char> objective, params string[] annotations)
    {
        var mzn = $"solve minimize {objective}{Annotations(annotations)};";
        var obj = ParseStatement<SolveStatement>(mzn);
        SetObjective(obj);
        return obj;
    }

    /// <summary>
    /// Set the model objective to maximize the given expression
    /// </summary>
    /// <example>model.Maximize("a+b")</example>
    /// <example>model.Maximize("makespan", "int_search(q, first_fail, indomain_min)")</example>
    public SolveStatement Maximize(ReadOnlySpan<char> objective, params string[] annotations)
    {
        var mzn = $"solve maximize {objective}{Annotations(annotations)};";
        var obj = ParseStatement<SolveStatement>(mzn);
        SetObjective(obj);
        return obj;
    }

    /// <summary>
    /// Set the model objective to find a satisfactory solution
    /// </summary>
    /// <example>model.Satisfy();</example>
    /// <example>model.Satisfy("int_search(q, first_fail, indomain_min)")</example>
    public SolveStatement Satisfy(params string[] annotations)
    {
        var mzn = $"solve satisfy{Annotations(annotations)};";
        var obj = ParseStatement<SolveStatement>(mzn);
        SetObjective(obj);
        return obj;
    }

    /// <summary>
    /// Set the objective of the model to solve with the given
    /// syntax.  This will override any existing solve statement.
    /// <seealso cref="Maximize"/>
    /// <seealso cref="Minimize"/>
    /// <seealso cref="Satisfy"/>
    /// </summary>
    private void SetObjective(SolveStatement solve)
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
            case SolveStatement solve:
                if (_solve is not null)
                    Warning($"Existing solve statement `{_solve}` was overwritten with `{solve}`");
                _solve = solve;
                break;

            case OutputStatement output:
                _outputs ??= [];
                _outputs.Add(output);
                break;

            case ConstraintStatement node:
                _constraints ??= [];
                _constraints.Add(node);
                break;

            case TypeAliasSyntax alias:
                var name = alias.Name.StringValue;
                if (_namespace.TryGetValue(name, out var old))
                    Error($"Type alias name \"{name}\" conflicts with {old})");
                _namespace.Add(name, alias);
                break;

            case AssignStatement assign:
                name = assign.Name.StringValue;
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
                        var mzn = $"{declare.ToString()[..^1]} = {expr};";
                        declare = ParseStatement<DeclareStatement>(mzn);
                        _namespace[name] = declare;
                        break;

                    default:
                        Error($"Reassigned variable {name} from {old} to {expr}");
                        break;
                }
                break;

            case DeclareStatement declare:
                name = declare.Name.StringValue;
                _namespace.TryGetValue(name, out old);
                switch (old)
                {
                    case null:
                        _namespace[name] = declare;
                        break;

                    case AssignStatement assign when declare.Body is null:
                        declare.Body = assign.Expr;
                        _namespace[name] = declare;
                        break;

                    case DeclareStatement oldDeclare:
                        var oldType = oldDeclare.Type?.ToString();
                        var newType = declare.Type?.ToString();
                        if (oldType != newType)
                            Warning(
                                $"Function {name} overloaded with a differnt return type ({oldType} vs {newType})"
                            );

                        _overloads ??= new Dictionary<string, List<DeclareStatement>>();
                        if (!_overloads.TryGetValue(name, out var overloads))
                        {
                            overloads = [];
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
                    _includes ??= [];
                    _includes.Add(include);
                }
                else
                {
                    _parsedFiles ??= [];
                    if (!_parsedFiles.Add(file.FullName))
                    {
                        // TODO - should we just ignore this?
                        Error($"Detected recursive include of \"{file.FullName}\"");
                    }
                    else
                    {
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
        _addedFiles ??= [];
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
        _warnings ??= [];
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

    public static MiniZincModel FromString(string mzn)
    {
        var model = new MiniZincModel();
        model.AddString(mzn);
        return model;
    }

    public static MiniZincModel FromFile(string path)
    {
        var model = new MiniZincModel();
        model.AddFile(path);
        return model;
    }

    public static MiniZincModel FromFile(FileInfo file)
    {
        var model = new MiniZincModel();
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
                writer.WriteStatement(include);

        foreach (var syntax in _namespace.Values)
            writer.WriteStatement((StatementSyntax)syntax);

        if (_overloads is { } dict)
            foreach (var name in dict.Keys)
            {
                var overloads = dict[name];
                foreach (var overload in overloads)
                    writer.WriteStatement(overload);
            }

        foreach (var constraint in Constraints)
            writer.WriteStatement(constraint);

        if (_solve is not null)
            writer.WriteStatement(_solve);

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
