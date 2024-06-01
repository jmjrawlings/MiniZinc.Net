namespace MiniZinc.Client;

using Parser;
using Parser.Syntax;

public sealed class Model
{
    public readonly List<SyntaxNode> Nodes;

    public readonly Dictionary<string, object> NameSpace;

    public readonly HashSet<string> Variables;

    public List<SyntaxNode> Constraints { get; private set; }

    public SolveMethod SolveMethod { get; private set; }

    public SyntaxNode? Objective { get; private set; }

    public List<SyntaxNode> Outputs { get; private set; }

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

    public IEnumerable<string> Warnings => _warnings ?? Enumerable.Empty<string>();

    public IEnumerable<string> Errors => _errors ?? Enumerable.Empty<string>();

    private List<string>? _warnings;
    private List<string>? _errors;

    private Model(IEnumerable<SyntaxNode>? nodes = null)
    {
        _errors = null;
        _warnings = null;
        Outputs = new List<SyntaxNode>();
        Constraints = new List<SyntaxNode>();
        SolveMethod = SolveMethod.Satisfy;
        Objective = null;
        Nodes = new List<SyntaxNode>();
        Variables = new HashSet<string>();
        NameSpace = new Dictionary<string, object>();
        if (nodes is not null)
            AddNodes(nodes);
    }

    /// <summary>
    /// Compile this model from the files and strings
    /// </summary>
    public Model AddNodes(IEnumerable<SyntaxNode> nodes)
    {
        foreach (var node in nodes)
            AddNode(node);
    }

    /// <summary>
    /// Add the given syntax node to the model
    /// </summary>
    void AddNode(SyntaxNode node)
    {
        switch (node)
        {
            case ArrayAccessSyntax arrayAccessSyntax:
                break;
            case Array2dSyntax array2dSyntax:
                break;
            case Array3dSyntax array3dSyntax:
                break;
            case Array1DSyntax array1DSyntax:
                break;
            case ArraySyntax arraySyntax:
                break;
            case ArrayTypeSyntax arrayTypeSyntax:
                break;
            case AssignmentSyntax assignmentSyntax:
                break;
            case BinaryOperatorSyntax binaryOperatorSyntax:
                break;
            case BoolLiteralSyntax boolLiteralSyntax:
                break;
            case CallSyntax callSyntax:
                break;
            case CompositeTypeSyntax compositeTypeSyntax:
                break;
            case ComprehensionSyntax comprehensionSyntax:
                break;
            case ConstraintSyntax constraintSyntax:
                break;
            case DeclarationSyntax declarationSyntax:
                break;
            case EmptyLiteralSyntax emptyLiteralSyntax:
                break;
            case EnumCasesSyntax enumCasesSyntax:
                break;
            case EnumDeclarationSyntax enumDeclarationSyntax:
                break;
            case ExprType exprType:
                break;
            case FloatLiteralSyntax floatLiteralSyntax:
                break;
            case FunctionDeclarationSyntax functionDeclarationSyntax:
                break;
            case GeneratorCallSyntax generatorCallSyntax:
                break;
            case GeneratorSyntax generatorSyntax:
                break;
            case IdentifierSyntax identifierSyntax:
                break;
            case IfElseSyntax ifElseSyntax:
                break;
            case IncludeSyntax includeSyntax:
                break;
            case IndexAndNode indexAndNode:
                break;
            case IntLiteralSyntax intLiteralSyntax:
                break;
            case LetSyntax letSyntax:
                break;
            case ListTypeSyntax listTypeSyntax:
                break;
            case NameTypeSyntax nameTypeSyntax:
                break;
            case OutputSyntax outputSyntax:
                break;
            case ParameterSyntax parameterSyntax:
                break;
            case RangeLiteralSyntax rangeLiteralSyntax:
                break;
            case RecordAccessSyntax recordAccessSyntax:
                break;
            case RecordLiteralSyntax recordLiteralSyntax:
                break;
            case RecordTypeSyntax recordTypeSyntax:
                break;
            case SetLiteralSyntax setLiteralSyntax:
                break;
            case SetTypeSyntax setTypeSyntax:
                break;
            case SolveSyntax solveSyntax:
                break;
            case StringLiteralSyntax stringLiteralSyntax:
                break;
            case SyntaxNode<TODO> syntaxNode:
                break;
            case SyntaxTree syntaxTree:
                break;
            case TupleAccessSyntax tupleAccessSyntax:
                break;
            case TupleLiteralSyntax tupleLiteralSyntax:
                break;
            case TupleTypeSyntax tupleTypeSyntax:
                break;
            case TypeAliasSyntax typeAliasSyntax:
                break;
            case TypeSyntax typeSyntax:
                break;
            case UnaryOperatorSyntax unaryOperatorSyntax:
                break;
            case WildCardExpr wildCardExpr:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(node));
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
    public Model AddFile(string path)
    {
        var file = new FileInfo(path);
        return AddFile(file);
    }

    /// <summary>
    /// Add the given file to this model (.mzn or .dzn)
    /// </summary>
    public Model AddFile(FileInfo file)
    {
        if (!file.Exists)
            return Error($"File \"{file.FullName}\" does not exist");

        var result = Parser.ParseFile(file);
        if (!result.Ok)
            return Error(result.ErrorMessage!);

        return AddNodes(result.Syntax.Nodes);
    }

    /// <summary>
    /// Add the given error to this model
    /// </summary>
    Model Error(string msg)
    {
        _errors ??= new List<string>();
        _errors.Add(msg);
        return this;
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

    public Model AddString(string mzn)
    {
        var result = Parser.ParseString(mzn);
        if (!result.Ok)
            return Error(result.ErrorMessage!);
        return AddNodes(result.Syntax.Nodes);
    }

    public Model AddStrings(params string[] strings)
    {
        foreach (var mzn in strings)
            AddString(mzn);
        return this;
    }

    public Model Copy()
    {
        var mzn = ToString();
        var copy = FromString(mzn);
        return copy;
    }

    public override string ToString()
    {
        var tree = new SyntaxTree(default);
        tree.Nodes.AddRange(Nodes);
        var mzn = tree.Write();
        return mzn;
    }
}
