namespace MiniZinc.Parser;

/// <summary>
///
/// </summary>
public sealed class Parser
{
    private readonly IEnumerator<Token> _tokens;

    private Parser(IEnumerator<Token> tokens)
    {
        _tokens = tokens;
    }
}
