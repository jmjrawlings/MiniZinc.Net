namespace MiniZinc.Parser;

internal static class Keyword
{
    internal const TokenKind Annotation = TokenKind.KeywordAnnotation;
    internal const TokenKind Ann = TokenKind.KeywordAnn;
    internal const TokenKind Any = TokenKind.KeywordAny;
    internal const TokenKind Array = TokenKind.KeywordArray;
    internal const TokenKind Bool = TokenKind.KeywordBool;
    internal const TokenKind Case = TokenKind.KeywordCase;
    internal const TokenKind Constraint = TokenKind.KeywordConstraint;
    internal const TokenKind Default = TokenKind.KeywordDefault;
    internal const TokenKind Diff = TokenKind.KeywordDiff;
    internal const TokenKind Div = TokenKind.KeywordDiv;
    internal const TokenKind Else = TokenKind.KeywordElse;
    internal const TokenKind Elseif = TokenKind.KeywordElseif;
    internal const TokenKind Endif = TokenKind.KeywordEndif;
    internal const TokenKind Enum = TokenKind.KeywordEnum;
    internal const TokenKind False = TokenKind.KeywordFalse;
    internal const TokenKind Float = TokenKind.KeywordFloat;
    internal const TokenKind Function = TokenKind.KeywordFunction;
    internal const TokenKind If = TokenKind.KeywordIf;
    internal const TokenKind In = TokenKind.KeywordIn;
    internal const TokenKind Include = TokenKind.KeywordInclude;
    internal const TokenKind Int = TokenKind.KeywordInt;
    internal const TokenKind Intersect = TokenKind.KeywordIntersect;
    internal const TokenKind Let = TokenKind.KeywordLet;
    internal const TokenKind List = TokenKind.KeywordList;
    internal const TokenKind Maximize = TokenKind.KeywordMaximize;
    internal const TokenKind Minimize = TokenKind.KeywordMinimize;
    internal const TokenKind Mod = TokenKind.KeywordMod;
    internal const TokenKind Not = TokenKind.KeywordNot;
    internal const TokenKind Of = TokenKind.KeywordOf;
    internal const TokenKind Op = TokenKind.KeywordOp;
    internal const TokenKind Opt = TokenKind.KeywordOpt;
    internal const TokenKind Output = TokenKind.KeywordOutput;
    internal const TokenKind Par = TokenKind.KeywordPar;
    internal const TokenKind Predicate = TokenKind.KeywordPredicate;
    internal const TokenKind Record = TokenKind.KeywordRecord;
    internal const TokenKind Satisfy = TokenKind.KeywordSatisfy;
    internal const TokenKind Set = TokenKind.KeywordSet;
    internal const TokenKind Solve = TokenKind.KeywordSolve;
    internal const TokenKind String = TokenKind.KeywordString;
    internal const TokenKind Subset = TokenKind.KeywordSubset;
    internal const TokenKind Superset = TokenKind.KeywordSuperset;
    internal const TokenKind Symdiff = TokenKind.KeywordSymdiff;
    internal const TokenKind Test = TokenKind.KeywordTest;
    internal const TokenKind Then = TokenKind.KeywordThen;
    internal const TokenKind True = TokenKind.KeywordTrue;
    internal const TokenKind Tuple = TokenKind.KeywordTuple;
    internal const TokenKind Type = TokenKind.KeywordType;
    internal const TokenKind Union = TokenKind.KeywordUnion;
    internal const TokenKind Var = TokenKind.KeywordVar;
    internal const TokenKind Where = TokenKind.KeywordWhere;
    internal const TokenKind Xor = TokenKind.KeywordXor;

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
