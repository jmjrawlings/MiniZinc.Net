namespace MiniZinc.Parser;

[Flags]
internal enum LexOptions
{
    /// <summary>
    /// Default
    /// </summary>
    None,

    /// <summary>
    /// Line comments (eg: % this is a comment) will
    /// be lexed and returned in the token stream
    /// </summary>
    LexLineComments,

    /// <summary>
    /// Block comments (eg: /* block comment */) will
    /// be lexed and returned in the token stream
    /// </summary>
    LexBlockComments
}
