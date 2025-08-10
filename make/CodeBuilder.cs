namespace Make;

using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <summary>
/// Like a StringBuilder but for writing C# code.
/// Blocks are constructed using scopes that
/// correctly indent and dedent on use/disposal.
///
/// Each time a scope is started (eg namespace, block) it is kept
/// track of.  Any scopes that have not been disposed when ToString()
/// is called will be disposed of then.  That way we dont have to wrap
/// every single scope in a `using` block allowing for things like:
/// </summary>
public sealed class CodeBuilder
{
    // Underlying StringBuilder
    readonly StringBuilder _sb;

    // Current indentation level
    int _indent;

    // All created scopes
    private Stack<Scope>? _scopes;

    /// <summary>
    /// A disposable that performs a side effect when disposed.
    /// in our case this means closing open brackets, or
    /// dedenting.
    /// </summary>
    private sealed class Scope : IDisposable
    {
        private readonly Action _onDispose;
        public bool Disposed { get; private set; }

        public Scope(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (Disposed)
                throw new Exception("Scope was already disposed");

            _onDispose();
            Disposed = true;
        }
    }

    /// <summary>
    /// Create a new CodeBuilder
    /// </summary>
    /// <param name="indent">The initial indentation level</param>
    public CodeBuilder(int indent = 0)
    {
        _indent = indent;
        _sb = new StringBuilder();
        _scopes = new();
    }

    /// <summary>
    /// Write the given string at the current level of indentation
    /// </summary>
    public void Write(string? s)
    {
        _sb.Append('\t', _indent);
        if (s is not null)
            Append(s);
    }

    /// <summary>
    /// Write the given strings at the current level of indentation
    /// </summary>
    public void Write(params string[] strings) => WriteJoin(" ", strings);

    public void WriteJoin(string join, IEnumerable<string> strings)
    {
        using var enumerator = strings.GetEnumerator();
        if (!enumerator.MoveNext())
            return;

        while (true)
        {
            _sb.Append(enumerator.Current);
            if (!enumerator.MoveNext())
                break;
            _sb.Append(join);
        }
    }

    /// <summary>
    /// Write the given char at the current level of indentation
    /// </summary>
    public void Write(char c)
    {
        _sb.Append('\t', _indent);
        Append(c);
    }

    /// <summary>
    /// Write the given string at the current level of indentation plus a newline
    /// </summary>
    public void WriteLn(string? s)
    {
        Write(s);
        NewLine();
    }

    /// <summary>
    /// Write the given string at the current level of indentation plus a newline
    /// </summary>
    public void WriteLn(params string[] strings)
    {
        Write(strings);
        NewLine();
    }

    /// <summary>
    /// Start a `namespace` scope
    /// </summary>
    public IDisposable NameSpace(string name) => Block($"namespace {name}");

    /// <summary>
    /// Throw a new argument of the given type with the given arguments
    /// </summary>
    public void Throw(string exceptionType, params string[] args) =>
        WriteLn($"throw new {exceptionType}({string.Join(", ", args)});");

    public void Return(string? s = null)
    {
        if (s is null)
            WriteLn("return;");
        else
            WriteLn($"return {s};");
    }

    public void Attribute(string name, params string?[] args)
    {
        Write('[');
        Append(name);
        if (args.Length > 0)
        {
            Append('(');
            Append(string.Join(", ", args.Select(a => a ?? "null")));
            Append(')');
        }
        Append(']');
        NewLine();
    }

    public void New(string className, params string[] args)
    {
        Append("new");
        Space();
        Append('(');
        Append(args);
        Append(')');
        Append(';');
    }

    public void Call(string? name, params string[] args)
    {
        Write(name);
        Append('(');
        Append(string.Join(", ", args));
        Append(')');
        Append(';');
        NewLine();
    }

    public static string Quote([NotNullIfNotNull("s")] string? s)
    {
        if (s is null)
            return "null";
        string x = s.Replace("\\", "");
        x = $"\"{x}\"";
        return x;
    }

    public static string TripleQuote(string s) => $"\"\"\"{s.Replace("\n", "")}\"\"\"";

    public void Var(string? name, string? value)
    {
        Write("var ");
        Append(name);
        Append(" = ");
        Append(value ?? "null");
        Append(';');
        NewLine();
    }

    public void Break() => WriteLn("break;");

    public void Assign(string name, string value)
    {
        Write(name);
        Space();
        Append('=');
        Space();
        Append(value);
        Append(';');
        NewLine();
    }

    public void Declare(string? type, string? name)
    {
        Write(type);
        Spaces();
        Append(name);
        Append(';');
        NewLine();
    }

    public void Declare(string? type, string? name, string? value)
    {
        Write(type);
        Spaces();
        Append(name);
        Append(" = ");
        Append(value ?? "null");
        Append(';');
        NewLine();
    }

    public IDisposable If(string expr) => Block($"if ({expr})");

    public IDisposable ElseIf(string expr) => Block($"else if ({expr})");

    public IDisposable Else() => Block($"else");

    public IDisposable ForEach(string expr) => Block($"foreach ({expr})");

    public IDisposable While(string expr) => Block($"while ({expr})");

    public void Append(char c)
    {
        _sb.Append(c);
    }

    public void Append(string? s)
    {
        _sb.Append(s);
    }

    public void AppendLn(string? s)
    {
        _sb.AppendLine(s);
    }

    public void AppendJoin(string join, IEnumerable<string> strings)
    {
        using var enumerator = strings.GetEnumerator();
        if (!enumerator.MoveNext())
            return;

        while (true)
        {
            _sb.Append(enumerator.Current);
            if (!enumerator.MoveNext())
                break;
            _sb.Append(join);
        }
    }

    public void Append(params string[] strings)
    {
        AppendJoin(" ", strings);
    }

    /// <summary>
    /// Add 'using' namespace declarations
    /// </summary>
    public void Using(params string[] namespaces)
    {
        foreach (var nameSpace in namespaces)
        {
            WriteLn($"using {nameSpace};");
        }
    }

    public void NewLine()
    {
        _sb.AppendLine();
    }

    /// <summary>
    /// Write a summary xml block
    /// </summary>
    /// <param name="lines"></param>
    public void Summary(params string[] lines)
    {
        WriteLn("/// <summary>");
        foreach (var line in lines)
        {
            foreach (
                var str in line.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries)
            )
            {
                Write("/// ");
                Append(str);
                NewLine();
            }
        }

        WriteLn("/// </summary>");
    }

    public IDisposable Indent()
    {
        _indent++;
        return PushScope(() =>
        {
            _indent--;
        });
    }

    /// <summary>
    /// Create a new scope with the given disposable action
    /// </summary>
    IDisposable PushScope(Action action)
    {
        var scope = new Scope(action);
        _scopes ??= new Stack<Scope>();
        _scopes.Push(scope);
        return scope;
    }

    /// <summary>
    /// Define a function with the given signature and arguments
    /// </summary>
    /// <example>using (Function("bool? Okay", "string a", "int b"))</example>
    public IDisposable Function(string signature, params string[] args)
    {
        Write(signature);
        Append('(');
        AppendJoin(", ", args);
        Append(')');
        NewLine();
        return Block();
    }

    /// <summary>
    /// Starts a '{'  '}' block with the given tokens beforehand
    /// </summary>
    /// <example>using (Scope("public","sealed","class",className))</example>
    public IDisposable Block(params string[] tokens)
    {
        var preamble = string.Join(" ", tokens);
        return Block(preamble);
    }

    /// <summary>
    /// Starts a '{'  '}' block
    /// </summary>
    public IDisposable Block(string? s = null)
    {
        if (s is not null)
        {
            Write(s);
            NewLine();
            Write('{');
            NewLine();
        }
        else
        {
            Write('{');
            NewLine();
        }

        _indent++;
        return PushScope(() =>
        {
            _indent--;
            Write('}');
            NewLine();
            NewLine();
        });
    }

    public void Spaces(int n = 1)
    {
        _sb.Append(' ', n);
    }

    public void Space()
    {
        _sb.Append(' ');
    }

    public IDisposable BlockComment()
    {
        WriteLn("/*");
        return PushScope(() =>
        {
            NewLine();
            WriteLn("*/");
        });
    }

    public override string ToString()
    {
        // Close out all remaining scopes
        if (_scopes is not null)
        {
            while (_scopes.Count > 0)
            {
                var scope = _scopes.Pop();
                if (!scope.Disposed)
                    scope.Dispose();
            }
        }

        var code = _sb.ToString();
        return code;
    }
}
