namespace MiniZinc.Parser;

using System.Diagnostics;
using System.Text;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed partial class Parser
{
    private readonly IEnumerator<Token> _tokens;
    private readonly Queue<Token> _cache;
    private readonly Stopwatch _watch;
    private Token _token;
    private TokenKind _kind;
    private int _bufferIndex;
    private int _bufferCount;
    private Token[] _buffer;
    private int _pos;
    public string? Err;
    public TimeSpan Elapsed => _watch.Elapsed;

    internal Parser(IEnumerator<Token> tokens)
    {
        _watch = Stopwatch.StartNew();
        _tokens = tokens;
        _buffer = new Token[1024];
        _bufferIndex = -1;
        _bufferCount = 0;
        _pos = 0;
    }

    /// Progress the parser
    public bool Step()
    {
        // Read token from the buffer if possible
        if (++_bufferIndex < _bufferCount)
        {
            _token = _buffer[_bufferIndex];
            _kind = _token.Kind;
            _pos++;
            Debug.Write(_token.ToString());
            return true;
        }

        // Fill the buffer from the source
        _bufferIndex = 0;
        _bufferCount = 0;

        while (_bufferCount < _buffer.Length && _tokens.MoveNext())
            _buffer[_bufferCount++] = _tokens.Current;

        _pos++;
        _token = _buffer[0];
        _kind = _token.Kind;
        Debug.Write(_token.ToString());
        return _bufferCount > 0;
    }

    /// Progress the parser if the current token is of the given kind
    private bool Skip(TokenKind kind)
    {
        if (_token.Kind != kind)
            return false;
        Step();
        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind)
    {
        if (_kind != kind)
        {
            Error($"Expected a {kind} but encountered a {_token.Kind}");
            return false;
        }

        Step();
        return true;
    }

    /// <summary>
    /// Read the current tokens string into the given variable
    /// </summary>
    private bool ParseString(out string result, TokenKind kind = TokenKind.STRING_LIT)
    {
        if (_kind == kind && _token.String is { } s)
        {
            Step();
            result = s;
            return true;
        }

        result = string.Empty;
        return false;
    }

    private bool ParseIdent(out string name) => ParseString(out name, TokenKind.IDENT);

    /// Record the given message as an error and return false
    private bool Error(string? msg = null)
    {
        if (Err is not null)
            return false;

        _watch.Stop();
        var sb = new StringBuilder();
        for (int i = 0; i <= _bufferIndex; i++)
        {
            var token = _buffer[i];
            if (token.Kind <= TokenKind.XOR)
            {
                sb.Append(' ');
                sb.Append(token.ToString());
                sb.Append(' ');
            }
            else
            {
                sb.Append(token.ToString());
            }

            if (token.Kind is TokenKind.EOL)
                sb.AppendLine();
        }

        var trace = sb.ToString();

        var message = $"""
             An error occured after {Elapsed}
             Position {_pos}
             Token {_kind}
             Line {_token.Line}
             Column {_token.Col})
             ---------------------------------------------
             {msg}
             ---------------------------------------------
             {trace}
             """;

        Err = message;
        return false;
    }
}
