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

    internal const TokenKind Annotation = KEYWORD_ANNOTATION;
    internal const TokenKind Ann = KEYWORD_ANN;
    internal const TokenKind Any = KEYWORD_ANY;
    internal const TokenKind AnonEnum = KEYWORD_ANONENUM;
    internal const TokenKind Array = KEYWORD_ARRAY;
    internal const TokenKind Bool = KEYWORD_BOOL;
    internal const TokenKind Case = KEYWORD_CASE;
    internal const TokenKind Constraint = KEYWORD_CONSTRAINT;
    internal const TokenKind Default = KEYWORD_DEFAULT;
    internal const TokenKind Diff = KEYWORD_DIFF;
    internal const TokenKind Div = KEYWORD_DIV;
    internal const TokenKind Else = KEYWORD_ELSE;
    internal const TokenKind Elseif = KEYWORD_ELSEIF;
    internal const TokenKind Endif = KEYWORD_ENDIF;
    internal const TokenKind Enum = KEYWORD_ENUM;
    internal const TokenKind False = KEYWORD_FALSE;
    internal const TokenKind Float = KEYWORD_FLOAT;
    internal const TokenKind Function = KEYWORD_FUNCTION;
    internal const TokenKind If = KEYWORD_IF;
    internal const TokenKind In = KEYWORD_IN;
    internal const TokenKind Include = KEYWORD_INCLUDE;
    internal const TokenKind Int = KEYWORD_INT;
    internal const TokenKind Intersect = KEYWORD_INTERSECT;
    internal const TokenKind Let = KEYWORD_LET;
    internal const TokenKind List = KEYWORD_LIST;
    internal const TokenKind Maximize = KEYWORD_MAXIMIZE;
    internal const TokenKind Minimize = KEYWORD_MINIMIZE;
    internal const TokenKind Mod = KEYWORD_MOD;
    internal const TokenKind Not = KEYWORD_NOT;
    internal const TokenKind Of = KEYWORD_OF;
    internal const TokenKind Op = KEYWORD_OP;
    internal const TokenKind Opt = KEYWORD_OPT;
    internal const TokenKind Output = KEYWORD_OUTPUT;
    internal const TokenKind Par = KEYWORD_PAR;
    internal const TokenKind Predicate = KEYWORD_PREDICATE;
    internal const TokenKind Record = KEYWORD_RECORD;
    internal const TokenKind Satisfy = KEYWORD_SATISFY;
    internal const TokenKind Set = KEYWORD_SET;
    internal const TokenKind Solve = KEYWORD_SOLVE;
    internal const TokenKind String = KEYWORD_STRING;
    internal const TokenKind Subset = KEYWORD_SUBSET;
    internal const TokenKind Superset = KEYWORD_SUPERSET;
    internal const TokenKind Symdiff = KEYWORD_SYMDIFF;
    internal const TokenKind Test = KEYWORD_TEST;
    internal const TokenKind Then = KEYWORD_THEN;
    internal const TokenKind True = KEYWORD_TRUE;
    internal const TokenKind Tuple = KEYWORD_TUPLE;
    internal const TokenKind Type = KEYWORD_TYPE;
    internal const TokenKind Union = KEYWORD_UNION;
    internal const TokenKind Var = KEYWORD_VAR;
    internal const TokenKind Where = KEYWORD_WHERE;
    internal const TokenKind Xor = KEYWORD_XOR;

    internal static readonly Dictionary<string, TokenKind> Lookup;

    static void Add(TokenKind kind)
    {
        var key = GetString(kind);
        Lookup[key] = kind;
    }

    static Keyword()
    {
        Lookup = new Dictionary<string, TokenKind>();
        Add(Annotation);
        Add(Ann);
        Add(AnonEnum);
        Add(Any);
        Add(Array);
        Add(Bool);
        Add(Case);
        Add(Constraint);
        Add(Default);
        Add(Diff);
        Add(Div);
        Add(Else);
        Add(Elseif);
        Add(Endif);
        Add(Enum);
        Add(False);
        Add(Float);
        Add(Function);
        Add(If);
        Add(In);
        Add(Include);
        Add(Int);
        Add(Intersect);
        Add(Let);
        Add(List);
        Add(Maximize);
        Add(Minimize);
        Add(Mod);
        Add(Not);
        Add(Of);
        Add(Op);
        Add(Opt);
        Add(Output);
        Add(Par);
        Add(Predicate);
        Add(Record);
        Add(Satisfy);
        Add(Set);
        Add(Solve);
        Add(String);
        Add(Subset);
        Add(Superset);
        Add(Symdiff);
        Add(Test);
        Add(Then);
        Add(True);
        Add(Tuple);
        Add(Type);
        Add(Union);
        Add(Var);
        Add(Where);
        Add(Xor);
    }
}
