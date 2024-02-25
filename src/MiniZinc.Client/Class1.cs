namespace MiniZinc.Client;

/// <summary>
/// MiniZinc Client
/// </summary>
public class MiniZincClient
{
    public readonly string MiniZincPath;

    public MiniZincClient(string miniZincPath)
    {
        MiniZincPath = miniZincPath;
    }
}
