namespace MiniZinc.Parser;

internal static class Keyword
{
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
        Lookup[Token.KindString(kind)] = kind;
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
