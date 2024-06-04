namespace MiniZinc.Client;

using System.Text;
using Parser;
using Parser.Syntax;

/// <summary>
/// A MiniZinc model
/// </summary>
/// <remarks>
/// This class extracts useful semantic information
/// from <see cref="SyntaxTree"/>
/// </remarks>
public sealed class Model
{
    private Dictionary<string, INamedSyntax> _namespace;

    private HashSet<string>? _variables;

    private List<ConstraintSyntax>? _constraints;

    private List<OutputSyntax>? _outputs;

    private SolveSyntax? _solve;

    private List<string>? _warnings;

    private List<string>? _errors;

    private StringBuilder _sourceText;

    public string SourceText => _sourceText.ToString();

    public IEnumerable<string> Warnings => _warnings ?? Enumerable.Empty<string>();

    public IEnumerable<string> Errors => _errors ?? Enumerable.Empty<string>();

    public IEnumerable<ConstraintSyntax> Constraints =>
        _constraints ?? Enumerable.Empty<ConstraintSyntax>();

    public IEnumerable<OutputSyntax> Outputs => _outputs ?? Enumerable.Empty<OutputSyntax>();

    public SolveSyntax? Solve => _solve;

    public SolveMethod Method => _solve?.Method ?? SolveMethod.Satisfy;

    public SyntaxNode? Objective => _solve?.Objective;

    public int WarningCount => _warnings?.Count ?? 0;

    public int ErrorCount => _errors?.Count ?? 0;

    public bool HasErrors => _errors is null;

    public bool HasWarnings => _warnings is null;

    private Model(IEnumerable<SyntaxNode>? nodes = null)
    {
        _errors = null;
        _warnings = null;
        _outputs = null;
        _constraints = null;
        _variables = new HashSet<string>();
        _namespace = new Dictionary<string, INamedSyntax>();
        _sourceText = new StringBuilder();
        if (nodes is not null)
            foreach (var node in nodes)
                AddNode(node);
    }

    /// <summary>
    /// Include the given file in the model
    /// </summary>
    public void Include(string path)
    {
        AddString($"include \"{path}\";");
    }

    /// <summary>
    /// Add an include statement to the model
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
    /// Declare a parameter
    /// </summary>
    /// <returns>The name of the declared parameter</returns>
    public string Par(string name, string type, string? value = null) =>
        Declare(name, type, value);
    
    /// <summary>
    /// Declare a variable
    /// </summary>
    /// <returns>The name of the declared parameter</returns>
    public string Var(string name, string type, string? value = null) =>
        Declare(name, $"var {type}",value);

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
    public void Constraint(string expr, string? name = null)
    {
        if (name is null)
            AddString($"constraint {expr};");
        else
            AddString($"constraint {expr} :: \"{name}\"");
    }

    private string? Annotations(params string[] args) =>
        args.Length == 0 ? null : string.Join("::", args);

    /// <summary>
    /// Set the model objective to minimize the given expression
    /// </summary>
    public void Minimize(string expr, params string[] annotations)
    {
        AddString($"solve minimize {expr}{Annotations(annotations)};");
    }

    /// <summary>
    /// Set the model objective to maximize the given expression
    /// </summary>
    public void Maximize(string expr, params string[] annotations)
    {
        AddString($"solve maximize {expr}{Annotations(annotations)};");
    }

    /// <summary>
    /// Set the model objective to satisfy
    /// </summary>
    public void Satisfy(params string[] annotations)
    {
        AddString("solve satisfy;");
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
                    _solve = node;
                else
                    Error($"Can not override \"{_solve}\" with \"{syntaxNode}\"");
                break;

            case OutputSyntax node:
                _outputs ??= new List<OutputSyntax>();
                _outputs.Add(node);
                break;

            case ConstraintSyntax node:
                _constraints ??= new List<ConstraintSyntax>();
                _constraints.Add(node);
                break;

            case AssignmentSyntax node:
                var name = node.Name.ToString();
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
                break;

            case DeclarationSyntax node:
                name = node.Name.ToString();
                if (_namespace.TryGetValue(name, out old))
                    Error($"Variable {name} was already declared as {node.Type.SourceText}");
                else
                    _namespace[name] = node;
                break;

            case EnumDeclarationSyntax node:
                name = node.Name.ToString();
                if (_namespace.TryGetValue(name, out old))
                    Error($"Variable {name} was already declared as an Enumeration");
                else
                    _namespace[name] = node;
                break;

            case FunctionDeclarationSyntax node:
                name = node.Name.ToString();
                if (_namespace.TryGetValue(name, out old))
                    Error($"Variable {name} was already declared as a Function");
                else
                    _namespace[name] = node;
                break;

            // TODO - follow links?
            case IncludeSyntax node:
                var path = node.Path.StringValue;
                AddFile(path);
                break;

            case SyntaxTree tree:
                foreach (var node in tree.Nodes)
                    AddNode(node);
                break;
        }
    }

    /// <summary>
    /// Create an empty model
    /// </summary>
    /// <returns></returns>
    public static Model Create() => new Model();

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
    /// Create a new model from the given string
    /// </summary>
    public static Model FromString(string path)
    {
        var model = new Model();
        model.AddString(path);
        return model;
    }

    /// <summary>
    /// Add the given file to this model (.mzn or .dzn)
    /// </summary>
    public void AddFile(string path)
    {
        var file = new FileInfo(path);
        AddFile(file);
    }

    /// <summary>
    /// Add the given file to this model (.mzn or .dzn)
    /// </summary>
    public void AddFile(FileInfo file)
    {
        var result = Parser.ParseFile(file);
        result.EnsureOk();
        _sourceText.AppendLine(result.SourceText);
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
        _sourceText.AppendLine(result.SourceText);
        AddNode(result.SyntaxNode);
    }

    public Model AddStrings(params string[] strings)
    {
        foreach (var mzn in strings)
            AddString(mzn);
        return this;
    }

    public Model Clone()
    {
        var copy = FromString(SourceText);
        return copy;
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
