namespace MiniZinc.Command;

public static class Args
{
    /// <summary>
    /// Parse args from the given parameters
    /// </summary>
    public static Arg[] Parse(params string[]? args)
    {
        if (args is null)
            return [];

        var arguments = string.Join(' ', args);
        var array = Arg.Parse(arguments).ToArray();
        return array;
    }

    /// <summary>
    /// Concatenate two args
    /// </summary>
    public static Arg[] Concat(Arg[] arr1, Arg[] arr2)
    {
        var arr3 = new Arg[arr1.Length + arr2.Length];
        arr1.CopyTo(arr3, 0);
        arr2.CopyTo(arr3, arr1.Length);
        return arr3;
    }
}
