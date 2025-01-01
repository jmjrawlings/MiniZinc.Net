namespace MiniZinc.Parser;

using static TokenKind;

internal static class Keyword
{
    const char FWD_SLASH = '/';
    const char BACK_SLASH = '\\';
    const char STAR = '*';
    const char DELIMITER = ';';
    const char EQUAL = '=';
    const char OPEN_CHEVRON = '<';
    const char CLOSE_CHEVRON = '>';
    const char UP_CHEVRON = '^';
    const char DOT = '.';
    const char PLUS = '+';
    const char DASH = '-';
    const char TILDE = '~';
    const char DOLLAR = '$';
    const char OPEN_BRACKET = '[';
    const char CLOSE_BRACKET = ']';
    const char OPEN_PAREN = '(';
    const char CLOSE_PAREN = ')';
    const char OPEN_BRACE = '{';
    const char CLOSE_BRACE = '}';
    const char PIPE = '|';
    const char PERCENT = '%';
    const char UNDERSCORE = '_';
    const char COMMA = ',';
    const char EXCLAMATION = '!';
    const char SINGLE_QUOTE = '\'';
    const char DOUBLE_QUOTE = '"';
    const char BACKTICK = '`';
    const char SPACE = ' ';
    const char COLON = ':';
    const char NEWLINE = '\n';
    const char RETURN = '\r';
    const char EOF = '\uffff';
    const string ANNOTATION = "annotation";
    const string ANN = "ann";
    const string ANY = "any";
    const string ARRAY = "array";
    const string BOOL = "bool";
    const string CASE = "case";
    const string CONSTRAINT = "constraint";
    const string DEFAULT = "default";
    const string DIFF = "diff";
    const string DIV = "div";
    const string ELSE = "else";
    const string ELSEIF = "elseif";
    const string ENDIF = "endif";
    const string ENUM = "enum";
    const string FALSE = "false";
    const string FLOAT = "float";
    const string FUNCTION = "function";
    const string IF = "if";
    const string IN = "in";
    const string INCLUDE = "include";
    const string INT = "int";
    const string INTERSECT = "intersect";
    const string LET = "let";
    const string LIST = "list";
    const string MAXIMIZE = "maximize";
    const string MINIMIZE = "minimize";
    const string MOD = "mod";
    const string NOT = "not";
    const string OF = "of";
    const string OP = "op";
    const string OPT = "opt";
    const string OUTPUT = "output";
    const string PAR = "par";
    const string PREDICATE = "predicate";
    const string RECORD = "record";
    const string SATISFY = "satisfy";
    const string SET = "set";
    const string SOLVE = "solve";
    const string STRING = "string";
    const string SUBSET = "subset";
    const string SUPERSET = "superset";
    const string SYMDIFF = "symdiff";
    const string TEST = "test";
    const string THEN = "then";
    const string TRUE = "true";
    const string TUPLE = "tuple";
    const string TYPE = "type";
    const string UNION = "union";
    const string VAR = "var";
    const string WHERE = "where";
    const string XOR = "xor";
    const string EOL = ";";

    internal static string GetString(TokenKind kind) =>
        kind switch
        {
            KEYWORD_ANNOTATION => ANNOTATION,
            KEYWORD_ANN => ANN,
            KEYWORD_ANY => ANY,
            KEYWORD_ARRAY => ARRAY,
            KEYWORD_BOOL => BOOL,
            KEYWORD_CASE => CASE,
            KEYWORD_CONSTRAINT => CONSTRAINT,
            KEYWORD_DEFAULT => DEFAULT,
            KEYWORD_DIFF => DIFF,
            KEYWORD_DIV => DIV,
            KEYWORD_ELSE => ELSE,
            KEYWORD_ELSEIF => ELSEIF,
            KEYWORD_ENDIF => ENDIF,
            KEYWORD_ENUM => ENUM,
            KEYWORD_FALSE => FALSE,
            KEYWORD_FLOAT => FLOAT,
            KEYWORD_FUNCTION => FUNCTION,
            KEYWORD_IF => IF,
            KEYWORD_IN => IN,
            KEYWORD_INCLUDE => INCLUDE,
            KEYWORD_INT => INT,
            KEYWORD_INTERSECT => INTERSECT,
            KEYWORD_LET => LET,
            KEYWORD_LIST => LIST,
            KEYWORD_MAXIMIZE => MAXIMIZE,
            KEYWORD_MINIMIZE => MINIMIZE,
            KEYWORD_MOD => MOD,
            KEYWORD_NOT => NOT,
            KEYWORD_OF => OF,
            KEYWORD_OP => OP,
            KEYWORD_OPT => OPT,
            KEYWORD_OUTPUT => OUTPUT,
            KEYWORD_PAR => PAR,
            KEYWORD_PREDICATE => PREDICATE,
            KEYWORD_RECORD => RECORD,
            KEYWORD_SATISFY => SATISFY,
            KEYWORD_SET => SET,
            KEYWORD_SOLVE => SOLVE,
            KEYWORD_STRING => STRING,
            KEYWORD_SUBSET => SUBSET,
            KEYWORD_SUPERSET => SUPERSET,
            KEYWORD_SYMDIFF => SYMDIFF,
            KEYWORD_TEST => TEST,
            KEYWORD_THEN => THEN,
            KEYWORD_TRUE => TRUE,
            KEYWORD_TUPLE => TUPLE,
            KEYWORD_TYPE => TYPE,
            KEYWORD_UNION => UNION,
            KEYWORD_VAR => VAR,
            KEYWORD_WHERE => WHERE,
            KEYWORD_XOR => XOR,
            _ => string.Empty
        };

    internal static readonly Dictionary<string, TokenKind> Lookup;

    static void Add(TokenKind kind)
    {
        var key = GetString(kind);
        Lookup[key] = kind;
    }

    static Keyword()
    {
        Lookup = new Dictionary<string, TokenKind>();
        Add(KEYWORD_ANNOTATION);
        Add(KEYWORD_ANN);
        Add(KEYWORD_ANONENUM);
        Add(KEYWORD_ANY);
        Add(KEYWORD_ARRAY);
        Add(KEYWORD_BOOL);
        Add(KEYWORD_CASE);
        Add(KEYWORD_CONSTRAINT);
        Add(KEYWORD_DEFAULT);
        Add(KEYWORD_DIFF);
        Add(KEYWORD_DIV);
        Add(KEYWORD_ELSE);
        Add(KEYWORD_ELSEIF);
        Add(KEYWORD_ENDIF);
        Add(KEYWORD_ENUM);
        Add(KEYWORD_FALSE);
        Add(KEYWORD_FLOAT);
        Add(KEYWORD_FUNCTION);
        Add(KEYWORD_IF);
        Add(KEYWORD_IN);
        Add(KEYWORD_INCLUDE);
        Add(KEYWORD_INT);
        Add(KEYWORD_INTERSECT);
        Add(KEYWORD_LET);
        Add(KEYWORD_LIST);
        Add(KEYWORD_MAXIMIZE);
        Add(KEYWORD_MINIMIZE);
        Add(KEYWORD_MOD);
        Add(KEYWORD_NOT);
        Add(KEYWORD_OF);
        Add(KEYWORD_OP);
        Add(KEYWORD_OPT);
        Add(KEYWORD_OUTPUT);
        Add(KEYWORD_PAR);
        Add(KEYWORD_PREDICATE);
        Add(KEYWORD_RECORD);
        Add(KEYWORD_SATISFY);
        Add(KEYWORD_SET);
        Add(KEYWORD_SOLVE);
        Add(KEYWORD_STRING);
        Add(KEYWORD_SUBSET);
        Add(KEYWORD_SUPERSET);
        Add(KEYWORD_SYMDIFF);
        Add(KEYWORD_TEST);
        Add(KEYWORD_THEN);
        Add(KEYWORD_TRUE);
        Add(KEYWORD_TUPLE);
        Add(KEYWORD_TYPE);
        Add(KEYWORD_UNION);
        Add(KEYWORD_VAR);
        Add(KEYWORD_WHERE);
        Add(KEYWORD_XOR);
    }
}
