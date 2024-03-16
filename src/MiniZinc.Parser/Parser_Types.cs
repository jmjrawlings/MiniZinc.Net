namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    public bool ParseBaseTypeInst(out TypeInst type)
    {
        var inst = TypeFlags.Par;
        if (Skip(TokenKind.KeywordVar))
            inst = TypeFlags.Var;
        else
            Skip(TokenKind.KeywordPar);

        if (Skip(TokenKind.KeywordOpt))
            inst |= TypeFlags.Opt;

        if (Skip(TokenKind.KeywordSet))
        {
            Expect(TokenKind.KeywordOf);
            inst |= TypeFlags.Set;
        }

        if (!ParseBaseTypeTail(out type))
            return false;

        type.Flags = inst;
        return true;
    }

    /// <summary>
    /// Parse a list type
    /// </summary>
    /// <mzn>list of int</mzn>
    public bool ParseListType(out ArrayType type)
    {
        type = new ArrayType();
        if (!Skip(TokenKind.KeywordList))
            return false;

        if (!Expect(TokenKind.KeywordOf))
            return false;

        var dim = new TypeInst { Kind = TypeKind.Int, Flags = TypeFlags.Par };
        type.Dimensions.Add(dim);

        if (!ParseType(out var expr))
            return false;

        type.Type = expr;
        return true;
    }

    /// <summary>
    /// Parse an array type
    /// </summary>
    /// <mzn>array[X, 1..2} of var int</mzn>
    public bool ParseArrayType(out ArrayType arr)
    {
        arr = default;
        if (!Skip(TokenKind.KeywordArray))
            return false;

        arr = new ArrayType();

        if (!ParseExprs(out var dims, TokenKind.OpenBracket, TokenKind.CloseBrace))
            return false;
        arr.Dimensions = dims!;

        if (!Expect(TokenKind.KeywordOf))
            return false;

        if (!ParseType(out var type))
            return false;

        arr.Type = type;
        return true;
    }

    private bool ParseBaseTypeTail(out TypeInst type)
    {
        type = default;
        switch (_token.Kind)
        {
            case TokenKind.KeywordInt:
                Step();
                type = new TypeInst { Kind = TypeKind.Int };
                break;

            case TokenKind.KeywordBool:
                Step();
                type = new TypeInst { Kind = TypeKind.Bool };
                break;

            case TokenKind.KeywordFloat:
                Step();
                type = new TypeInst { Kind = TypeKind.Float };
                break;

            case TokenKind.KeywordString:
                Step();
                type = new TypeInst { Kind = TypeKind.String };
                break;

            case TokenKind.KeywordAnn:
                Step();
                type = new TypeInst { Kind = TypeKind.Annotation };
                break;

            case TokenKind.KeywordRecord:
                if (!ParseRecordType(out var rec))
                    return false;
                type = rec;
                break;

            case TokenKind.KeywordTuple:
                if (!ParseTupleType(out var tup))
                    return false;
                type = tup;
                break;

            default:
                if (!ParseExpr(out var expr))
                    return false;
                type = new ExprType { Expr = expr };
                break;
        }

        return true;
    }

    /// <summary>
    /// Parse a type name pair
    /// </summary>
    /// <mzn>int: a</mzn>
    /// <mzn>bool: ABC</mzn>
    public bool ParseTypeAndName(out Binding<TypeInst> result)
    {
        result = default;
        if (!ParseType(out var type))
            return false;

        if (!Expect(TokenKind.Colon))
            return false;

        if (!ParseString(out var name))
            return false;

        result = name.Bind(type);
        return true;
    }

    /// <summary>
    /// Parse a tuple type constructor
    /// </summary>
    /// <mzn>tuple(int, bool, tuple(int))</mzn>
    private bool ParseTupleType(out TupleTypeInst tuple)
    {
        tuple = default;
        if (!Skip(TokenKind.KeywordTuple))
            return false;

        tuple = new TupleTypeInst();
        if (!Expect(TokenKind.OpenParen))
            return false;

        while (_kind is not TokenKind.CloseParen)
        {
            if (!ParseType(out var ti))
                return false;

            tuple.Items.Add(ti);
            if (!Skip(TokenKind.Comma))
                break;
        }

        return Expect(TokenKind.CloseParen);
    }

    /// <summary>
    /// Parse a record type constructor
    /// </summary>
    /// <mzn>record(int: a, bool: b)</mzn>
    private bool ParseRecordType(out RecordTypeInst record)
    {
        record = default;
        if (!Skip(TokenKind.KeywordRecord))
            return false;
        record = new RecordTypeInst();

        if (!ParseParameters(out var fields))
            return false;
        record.Fields = fields;
        return true;
    }

    /// <summary>
    /// Parse a comma separated list of types
    /// and names between parentheses
    /// </summary>
    /// <mzn>(int: a, bool: b)</mzn>
    private bool ParseParameters(out List<Binding<TypeInst>> parameters)
    {
        parameters = null;
        if (!Expect(TokenKind.OpenParen))
            return false;

        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        next:
        if (!ParseTypeAndName(out var type))
            return false;

        parameters = new List<Binding<TypeInst>> { type };
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseParen);
    }

    private bool ParseArgs(out List<IExpr>? exprs) =>
        ParseExprs(out exprs, TokenKind.OpenParen, TokenKind.CloseParen);

    /// <summary>
    /// Parse a comma separated list of expressions
    /// between parentheses
    /// </summary>
    /// <mzn>(1, 2, false)</mzn>
    private bool ParseExprs(out List<IExpr>? exprs, TokenKind open, TokenKind close)
    {
        exprs = null;
        if (!Expect(open))
            return false;

        exprs = new List<IExpr>();
        while (_kind != close)
        {
            if (!ParseExpr(out var expr))
                return false;
            exprs.Add(expr);
            if (!Skip(TokenKind.Comma))
                break;
        }

        if (!Expect(close))
            return false;

        return true;
    }

    public bool ParseType(out TypeInst type)
    {
        type = default;
        switch (_token.Kind)
        {
            case TokenKind.KeywordArray:
            case TokenKind.KeywordList:
                if (!ParseArrayType(out var arr))
                    return false;
                type = arr;
                break;
            default:
                if (!ParseBaseTypeInst(out type))
                    return false;
                break;
        }

        return true;
    }
}
