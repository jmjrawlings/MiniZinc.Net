namespace MiniZinc.Parser;

using System.Diagnostics;
using System.Text;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed partial class Parser
{
    private readonly Token[] _tokens;
    private readonly Stopwatch _watch;
    private Token _token;
    private TokenKind _kind;
    private ushort _precedence;
    private uint _pos;
    private uint _line;
    private uint _col;
    private int _i;
    public string? ErrorString { get; private set; }
    private StringBuilder? _trace;
    public bool Err => ErrorString is not null;
    public TimeSpan Elapsed => _watch.Elapsed;

    public static Parser ParseFile(string path)
    {
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        return parser;
    }

    public static Parser ParseString(string text)
    {
        var parser = new Parser(text);
        return parser;
    }

    internal Parser(string mzn)
    {
        using var lexer = Lexer.LexString(mzn);
        _watch = Stopwatch.StartNew();
        _tokens = lexer.ToArray();
        _pos = 0;
        _line = 1;
        _col = 1;
        _i = 0;
#if DEBUG
        _trace = new StringBuilder();
#endif
    }

    /// Progress the parser
    public bool Step()
    {
        if (_i >= _tokens.Length)
        {
            _kind = TokenKind.EOF;
            return false;
        }

        _token = _tokens[_i++];
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
            i = _token.IntValue;
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
        if (_kind == kind)
        {
            Step();
            result = _token.StringValue;
            return true;
        }

        result = string.Empty;
        return false;
    }

    private bool ParseFloat(out double f)
    {
        if (_kind is TokenKind.FLOAT_LIT)
        {
            f = _token.DoubleValue;
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
            id = _token.StringValue;
            Step();
            return true;
        }
        id = string.Empty;
        return false;
    }

    /// Record the given message as an error and return false
    private bool Error(string? msg = null)
    {
        if (ErrorString is not null)
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

        ErrorString = message;
        return false;
    }
}
