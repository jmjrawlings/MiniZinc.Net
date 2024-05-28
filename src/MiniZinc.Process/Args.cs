namespace MiniZinc.Process;

using System.Collections;

public sealed class Args : IEnumerable<Arg>
{
    private readonly List<Arg> _args;

    public Args()
    {
        _args = new();
    }

    public Args(params string[] args)
        : this()
    {
        foreach (var a in args)
        foreach (var arg in Arg.Parse(a))
            _args.Add(arg);
    }

    public Args(params object[] args)
        : this()
    {
        foreach (var a in args)
        foreach (var arg in Arg.Parse(a.ToString()!))
            _args.Add(arg);
    }

    public void Add(Arg other)
    {
        _args.Add(other);
    }

    public void Add(Args other)
    {
        foreach (var arg in other)
            Add(arg);
    }

    public void Add(string s)
    {
        foreach (var arg in Arg.Parse(s))
            Add(arg);
    }

    public void Add(params string[] args)
    {
        foreach (var a in args)
        foreach (var arg in Arg.Parse(a))
            Add(arg);
    }

    public void Add(params object[] args)
    {
        foreach (var a in args)
        foreach (var arg in Arg.Parse(a.ToString()!))
            Add(arg);
    }

    public IEnumerator<Arg> GetEnumerator()
    {
        return _args.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        var s = string.Join(' ', _args);
        return s;
    }
}
