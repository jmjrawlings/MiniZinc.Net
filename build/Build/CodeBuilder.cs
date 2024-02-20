using System.Diagnostics;

namespace Build;

using System.Text;

/// <summary>
/// Wraps a StringBuilder for use in code generation
/// </summary>
public class CodeBuilder
{
    private readonly StringBuilder _sb;
    private int _indent = 0;
    private readonly Stack<IDisposable> _context;

    public CodeBuilder()
    {
        _sb = new();
        _context = new Stack<IDisposable>();
    }

    public void Write(string? s)
    {
        _sb.Append('\t', _indent);
        if (s is not null)
            Append(s);
    }

    public void Write(char c)
    {
        _sb.Append('\t', _indent);
        Append(c);
    }

    public void WriteLn(string? s)
    {
        Write(s);
        Newline();
    }

    public void WriteLn(char s)
    {
        Write(s);
        Newline();
    }

    public void Append(string s)
    {
        _sb.Append(s);
    }

    public void Append(char c)
    {
        _sb.Append(c);
    }

    protected void Newline()
    {
        _sb.AppendLine();
    }

    public IDisposable Indent()
    {
        _indent++;
        Debug.WriteLine("Push Inc {0}", _indent);
        return Push(() =>
        {
            _indent--;
            Debug.WriteLine("Pop Inc {0}", _indent);
        });
    }

    public IDisposable Braces()
    {
        Append(" {");
        return Push(() =>
        {
            Write('}');
        });
    }

    protected IDisposable Push(Action action)
    {
        var context = new Disposable(() =>
        {
            action();
            _context.Pop();
        });
        _context.Push(context);
        return context;
    }

    protected void Pop()
    {
        var ctx = _context.Peek();
        ctx.Dispose();
    }

    /// <summary>
    /// Starts a '{'  '}' block
    /// </summary>
    public IDisposable Block(string? s = null)
    {
        if (s is not null)
            Write(s);
        Braces();
        Newline();
        Indent();
        return new Disposable(() =>
        {
            Pop();
            Pop();
            Newline();
        });
    }

    public override string ToString()
    {
        var stack = _context.ToList();
        stack.Reverse();
        foreach (var context in stack)
            context.Dispose();
        var code = _sb.ToString();
        return code;
    }
}
