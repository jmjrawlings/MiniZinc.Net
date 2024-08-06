namespace MiniZinc.Parser;

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
            TokenKind.KEYWORD_ANNOTATION => ANNOTATION,
            TokenKind.KEYWORD_ANN => ANN,
            TokenKind.KEYWORD_ANY => ANY,
            TokenKind.KEYWORD_ARRAY => ARRAY,
            TokenKind.KEYWORD_BOOL => BOOL,
            TokenKind.KEYWORD_CASE => CASE,
            TokenKind.KEYWORD_CONSTRAINT => CONSTRAINT,
            TokenKind.KEYWORD_DEFAULT => DEFAULT,
            TokenKind.KEYWORD_DIFF => DIFF,
            TokenKind.KEYWORD_DIV => DIV,
            TokenKind.KEYWORD_ELSE => ELSE,
            TokenKind.KEYWORD_ELSEIF => ELSEIF,
            TokenKind.KEYWORD_ENDIF => ENDIF,
            TokenKind.KEYWORD_ENUM => ENUM,
            TokenKind.KEYWORD_FALSE => FALSE,
            TokenKind.KEYWORD_FLOAT => FLOAT,
            TokenKind.KEYWORD_FUNCTION => FUNCTION,
            TokenKind.KEYWORD_IF => IF,
            TokenKind.KEYWORD_IN => IN,
            TokenKind.KEYWORD_INCLUDE => INCLUDE,
            TokenKind.KEYWORD_INT => INT,
            TokenKind.KEYWORD_INTERSECT => INTERSECT,
            TokenKind.KEYWORD_LET => LET,
            TokenKind.KEYWORD_LIST => LIST,
            TokenKind.KEYWORD_MAXIMIZE => MAXIMIZE,
            TokenKind.KEYWORD_MINIMIZE => MINIMIZE,
            TokenKind.KEYWORD_MOD => MOD,
            TokenKind.KEYWORD_NOT => NOT,
            TokenKind.KEYWORD_OF => OF,
            TokenKind.KEYWORD_OP => OP,
            TokenKind.KEYWORD_OPT => OPT,
            TokenKind.KEYWORD_OUTPUT => OUTPUT,
            TokenKind.KEYWORD_PAR => PAR,
            TokenKind.KEYWORD_PREDICATE => PREDICATE,
            TokenKind.KEYWORD_RECORD => RECORD,
            TokenKind.KEYWORD_SATISFY => SATISFY,
            TokenKind.KEYWORD_SET => SET,
            TokenKind.KEYWORD_SOLVE => SOLVE,
            TokenKind.KEYWORD_STRING => STRING,
            TokenKind.KEYWORD_SUBSET => SUBSET,
            TokenKind.KEYWORD_SUPERSET => SUPERSET,
            TokenKind.KEYWORD_SYMDIFF => SYMDIFF,
            TokenKind.KEYWORD_TEST => TEST,
            TokenKind.KEYWORD_THEN => THEN,
            TokenKind.KEYWORD_TRUE => TRUE,
            TokenKind.KEYWORD_TUPLE => TUPLE,
            TokenKind.KEYWORD_TYPE => TYPE,
            TokenKind.KEYWORD_UNION => UNION,
            TokenKind.KEYWORD_VAR => VAR,
            TokenKind.KEYWORD_WHERE => WHERE,
            TokenKind.KEYWORD_XOR => XOR,
            _ => string.Empty
        };

    internal const TokenKind Annotation = TokenKind.KEYWORD_ANNOTATION;
    internal const TokenKind Ann = TokenKind.KEYWORD_ANN;
    internal const TokenKind Any = TokenKind.KEYWORD_ANY;
    internal const TokenKind AnonEnum = TokenKind.KEYWORD_ANONENUM;
    internal const TokenKind Array = TokenKind.KEYWORD_ARRAY;
    internal const TokenKind Bool = TokenKind.KEYWORD_BOOL;
    internal const TokenKind Case = TokenKind.KEYWORD_CASE;
    internal const TokenKind Constraint = TokenKind.KEYWORD_CONSTRAINT;
    internal const TokenKind Default = TokenKind.KEYWORD_DEFAULT;
    internal const TokenKind Diff = TokenKind.KEYWORD_DIFF;
    internal const TokenKind Div = TokenKind.KEYWORD_DIV;
    internal const TokenKind Else = TokenKind.KEYWORD_ELSE;
    internal const TokenKind Elseif = TokenKind.KEYWORD_ELSEIF;
    internal const TokenKind Endif = TokenKind.KEYWORD_ENDIF;
    internal const TokenKind Enum = TokenKind.KEYWORD_ENUM;
    internal const TokenKind False = TokenKind.KEYWORD_FALSE;
    internal const TokenKind Float = TokenKind.KEYWORD_FLOAT;
    internal const TokenKind Function = TokenKind.KEYWORD_FUNCTION;
    internal const TokenKind If = TokenKind.KEYWORD_IF;
    internal const TokenKind In = TokenKind.KEYWORD_IN;
    internal const TokenKind Include = TokenKind.KEYWORD_INCLUDE;
    internal const TokenKind Int = TokenKind.KEYWORD_INT;
    internal const TokenKind Intersect = TokenKind.KEYWORD_INTERSECT;
    internal const TokenKind Let = TokenKind.KEYWORD_LET;
    internal const TokenKind List = TokenKind.KEYWORD_LIST;
    internal const TokenKind Maximize = TokenKind.KEYWORD_MAXIMIZE;
    internal const TokenKind Minimize = TokenKind.KEYWORD_MINIMIZE;
    internal const TokenKind Mod = TokenKind.KEYWORD_MOD;
    internal const TokenKind Not = TokenKind.KEYWORD_NOT;
    internal const TokenKind Of = TokenKind.KEYWORD_OF;
    internal const TokenKind Op = TokenKind.KEYWORD_OP;
    internal const TokenKind Opt = TokenKind.KEYWORD_OPT;
    internal const TokenKind Output = TokenKind.KEYWORD_OUTPUT;
    internal const TokenKind Par = TokenKind.KEYWORD_PAR;
    internal const TokenKind Predicate = TokenKind.KEYWORD_PREDICATE;
    internal const TokenKind Record = TokenKind.KEYWORD_RECORD;
    internal const TokenKind Satisfy = TokenKind.KEYWORD_SATISFY;
    internal const TokenKind Set = TokenKind.KEYWORD_SET;
    internal const TokenKind Solve = TokenKind.KEYWORD_SOLVE;
    internal const TokenKind String = TokenKind.KEYWORD_STRING;
    internal const TokenKind Subset = TokenKind.KEYWORD_SUBSET;
    internal const TokenKind Superset = TokenKind.KEYWORD_SUPERSET;
    internal const TokenKind Symdiff = TokenKind.KEYWORD_SYMDIFF;
    internal const TokenKind Test = TokenKind.KEYWORD_TEST;
    internal const TokenKind Then = TokenKind.KEYWORD_THEN;
    internal const TokenKind True = TokenKind.KEYWORD_TRUE;
    internal const TokenKind Tuple = TokenKind.KEYWORD_TUPLE;
    internal const TokenKind Type = TokenKind.KEYWORD_TYPE;
    internal const TokenKind Union = TokenKind.KEYWORD_UNION;
    internal const TokenKind Var = TokenKind.KEYWORD_VAR;
    internal const TokenKind Where = TokenKind.KEYWORD_WHERE;
    internal const TokenKind Xor = TokenKind.KEYWORD_XOR;

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
