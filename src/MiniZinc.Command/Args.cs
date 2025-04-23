namespace MiniZinc.Command;

using System.Collections;

public sealed class Args
{
    private List<Arg>? _args;

    public static Args Empty => new Args();

    public int Count => _args?.Count ?? 0;

    public IEnumerable<Arg> Values => _args ?? Enumerable.Empty<Arg>();

    public void Add(params string[] args)
    {
        foreach (var arg in args)
        {
            foreach (var a in Arg.Parse(arg))
            {
                _args ??= new List<Arg>();
                _args.Add(a);
            }
        }
    }

    public void Add(Args args)
    {
        foreach (var arg in args.Values)
        {
            _args ??= new List<Arg>();
            _args.Add(arg);
        }
    }

    /// <summary>
    /// Parse args from the given parameters
    /// </summary>
    public static Args Parse(params string[] strings)
    {
        var args = new Args();
        args.Add(strings);
        return args;
    }

    public override string ToString() => string.Join(" ", Values);
}
