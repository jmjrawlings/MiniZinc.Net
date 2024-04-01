namespace MiniZinc.Parser;

using System.Diagnostics;
using System.Text;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed partial class Parser
{
    private readonly IEnumerator<Token> _lexer;
    private readonly Stopwatch _watch;
    private Token _token;
    private TokenKind _kind;
    private uint _pos;
    private uint _line;
    private uint _col;
    public string? Err;
    private StringBuilder? _trace;

    public TimeSpan Elapsed => _watch.Elapsed;

    internal Parser(IEnumerator<Token> lexer)
    {
        _watch = Stopwatch.StartNew();
        _lexer = lexer;
        _pos = 0;
        _line = 1;
        _col = 1;
#if DEBUG
        _trace = new StringBuilder();
#endif
    }

    /// Progress the parser
    public bool Step()
    {
        if (!_lexer.MoveNext())
        {
            _kind = TokenKind.EOF;
            return false;
        }

        _token = _lexer.Current;
        _kind = _token.Kind;
        _pos = _token.Position;

        if (_trace is null)
            return true;

        if (_line < _token.Line)
        {
            _col = 1;
            _trace.Append('\n');
        }

        _line = _token.Line;
        while (_col < _token.Col)
        {
            _trace.Append(' ');
            _col++;
        }
        var s = _token.ToString();
        _trace.Append(s);
        _col += _token.Length;
        return true;
    }

    // // Read token from the buffer if possible
    // if (++_bufferIndex < _bufferCount)
    // {
    //     _token = _buffer[_bufferIndex];
    //     _kind = _token.Kind;
    //     _pos++;
    //     return true;
    // }
    //
    // // Fill the buffer from the source
    // _bufferIndex = 0;
    // _bufferCount = 0;
    //
    // while (_bufferCount < _buffer.Length && _tokens.MoveNext())
    //     _buffer[_bufferCount++] = _tokens.Current;
    //
    // _pos++;
    // _token = _buffer[0];
    // _kind = _token.Kind;
    // return _bufferCount > 0;


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

    private bool ParseInt(out int i)
    {
        if (_kind is TokenKind.INT_LIT)
        {
            i = _token.Int;
            Step();
            return true;
        }

        i = 0;
        return false;
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

    private bool ParseFloat(out double f)
    {
        if (_kind is TokenKind.FLOAT_LIT)
        {
            f = _token.Double;
            Step();
            return true;
        }

        f = default;
        return false;
    }

    private bool ParseIdent(out string id)
    {
        if (_kind is TokenKind.IDENT or TokenKind.QUOTED_IDENT)
        {
            id = _token.String!;
            Step();
            return true;
        }
        id = string.Empty;
        return false;
    }

    /// Record the given message as an error and return false
    private bool Error(string? msg = null)
    {
        if (Err is not null)
            return false;

        _watch.Stop();
        var trace = _trace?.ToString() ?? string.Empty;
        var message = $"""
             
             ---------------------------------------------
             {msg}
             ---------------------------------------------
             Token {_kind}
             Line {_token.Line}
             Col {_token.Col}
             Pos {_pos}
             ---------------------------------------------
             {trace}
             """;

        Err = message;
        return false;
    }
}
