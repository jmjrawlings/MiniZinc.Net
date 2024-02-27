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
            TokenKind.ANNOTATION => ANNOTATION,
            TokenKind.ANN => ANN,
            TokenKind.ANY => ANY,
            TokenKind.ARRAY => ARRAY,
            TokenKind.BOOL => BOOL,
            TokenKind.CASE => CASE,
            TokenKind.CONSTRAINT => CONSTRAINT,
            TokenKind.DEFAULT => DEFAULT,
            TokenKind.DIFF => DIFF,
            TokenKind.DIV => DIV,
            TokenKind.ELSE => ELSE,
            TokenKind.ELSEIF => ELSEIF,
            TokenKind.ENDIF => ENDIF,
            TokenKind.ENUM => ENUM,
            TokenKind.FALSE => FALSE,
            TokenKind.FLOAT => FLOAT,
            TokenKind.FUNCTION => FUNCTION,
            TokenKind.IF => IF,
            TokenKind.IN => IN,
            TokenKind.INCLUDE => INCLUDE,
            TokenKind.INT => INT,
            TokenKind.INTERSECT => INTERSECT,
            TokenKind.LET => LET,
            TokenKind.LIST => LIST,
            TokenKind.MAXIMIZE => MAXIMIZE,
            TokenKind.MINIMIZE => MINIMIZE,
            TokenKind.MOD => MOD,
            TokenKind.NOT => NOT,
            TokenKind.OF => OF,
            TokenKind.OP => OP,
            TokenKind.OPT => OPT,
            TokenKind.OUTPUT => OUTPUT,
            TokenKind.PAR => PAR,
            TokenKind.PREDICATE => PREDICATE,
            TokenKind.RECORD => RECORD,
            TokenKind.SATISFY => SATISFY,
            TokenKind.SET => SET,
            TokenKind.SOLVE => SOLVE,
            TokenKind.STRING => STRING,
            TokenKind.SUBSET => SUBSET,
            TokenKind.SUPERSET => SUPERSET,
            TokenKind.SYMDIFF => SYMDIFF,
            TokenKind.TEST => TEST,
            TokenKind.THEN => THEN,
            TokenKind.TRUE => TRUE,
            TokenKind.TUPLE => TUPLE,
            TokenKind.TYPE => TYPE,
            TokenKind.UNION => UNION,
            TokenKind.VAR => VAR,
            TokenKind.WHERE => WHERE,
            TokenKind.XOR => XOR,
            _ => string.Empty
        };

    internal const TokenKind Annotation = TokenKind.ANNOTATION;
    internal const TokenKind Ann = TokenKind.ANN;
    internal const TokenKind Any = TokenKind.ANY;
    internal const TokenKind AnonEnum = TokenKind.ANONENUM;
    internal const TokenKind Array = TokenKind.ARRAY;
    internal const TokenKind Bool = TokenKind.BOOL;
    internal const TokenKind Case = TokenKind.CASE;
    internal const TokenKind Constraint = TokenKind.CONSTRAINT;
    internal const TokenKind Default = TokenKind.DEFAULT;
    internal const TokenKind Diff = TokenKind.DIFF;
    internal const TokenKind Div = TokenKind.DIV;
    internal const TokenKind Else = TokenKind.ELSE;
    internal const TokenKind Elseif = TokenKind.ELSEIF;
    internal const TokenKind Endif = TokenKind.ENDIF;
    internal const TokenKind Enum = TokenKind.ENUM;
    internal const TokenKind False = TokenKind.FALSE;
    internal const TokenKind Float = TokenKind.FLOAT;
    internal const TokenKind Function = TokenKind.FUNCTION;
    internal const TokenKind If = TokenKind.IF;
    internal const TokenKind In = TokenKind.IN;
    internal const TokenKind Include = TokenKind.INCLUDE;
    internal const TokenKind Int = TokenKind.INT;
    internal const TokenKind Intersect = TokenKind.INTERSECT;
    internal const TokenKind Let = TokenKind.LET;
    internal const TokenKind List = TokenKind.LIST;
    internal const TokenKind Maximize = TokenKind.MAXIMIZE;
    internal const TokenKind Minimize = TokenKind.MINIMIZE;
    internal const TokenKind Mod = TokenKind.MOD;
    internal const TokenKind Not = TokenKind.NOT;
    internal const TokenKind Of = TokenKind.OF;
    internal const TokenKind Op = TokenKind.OP;
    internal const TokenKind Opt = TokenKind.OPT;
    internal const TokenKind Output = TokenKind.OUTPUT;
    internal const TokenKind Par = TokenKind.PAR;
    internal const TokenKind Predicate = TokenKind.PREDICATE;
    internal const TokenKind Record = TokenKind.RECORD;
    internal const TokenKind Satisfy = TokenKind.SATISFY;
    internal const TokenKind Set = TokenKind.SET;
    internal const TokenKind Solve = TokenKind.SOLVE;
    internal const TokenKind String = TokenKind.STRING;
    internal const TokenKind Subset = TokenKind.SUBSET;
    internal const TokenKind Superset = TokenKind.SUPERSET;
    internal const TokenKind Symdiff = TokenKind.SYMDIFF;
    internal const TokenKind Test = TokenKind.TEST;
    internal const TokenKind Then = TokenKind.THEN;
    internal const TokenKind True = TokenKind.TRUE;
    internal const TokenKind Tuple = TokenKind.TUPLE;
    internal const TokenKind Type = TokenKind.TYPE;
    internal const TokenKind Union = TokenKind.UNION;
    internal const TokenKind Var = TokenKind.VAR;
    internal const TokenKind Where = TokenKind.WHERE;
    internal const TokenKind Xor = TokenKind.XOR;

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
