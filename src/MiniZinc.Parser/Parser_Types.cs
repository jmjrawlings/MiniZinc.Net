namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    private bool ParseParameters(out List<Binding<TypeInst>>? parameters)
    {
        parameters = null;
        if (!Expect(TokenKind.OpenParen))
            return false;

        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        next:
        if (!ParseTypeInstAndId(out var type))
            return false;

        parameters = new List<Binding<TypeInst>> { type };
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseParen);
    }

    public bool ParseArguments(out List<IExpr>? list)
    {
        list = null;
        Expect(TokenKind.OpenParen);
        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        next:
        list = new List<IExpr>();
        if (!ParseExpr(out var expr))
            return false;

        list.Add(expr);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseParen);
    }

    public bool ParseTypeInst(out TypeInst type)
    {
        type = default;
        switch (_token.Kind)
        {
            case TokenKind.KeywordArray:
            case TokenKind.KeywordList:
                if (!ParseArrayTypeInst(out var arr))
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

        if (!ParseBaseTypeInstTail(out type))
            return false;

        type.Flags = inst;
        return true;
    }

    public bool ParseArrayTypeInst(out ArrayType type)
    {
        type = new ArrayType();
        if (Skip(TokenKind.KeywordList))
        {
            var dim = new TypeInst { Kind = TypeKind.Int, Flags = TypeFlags.Par };
            type.Dimensions.Add(dim);
        }
        else
        {
            Expect(TokenKind.KeywordArray);
            Expect(TokenKind.OpenBracket);
            if (_token.Kind is TokenKind.CloseBracket)
                goto end;

            dim:
            var dim = ParseTypeInst();
            type.Dimensions.Add(dim);
            if (Skip(TokenKind.Comma))
                goto dim;

            end:
            Expect(TokenKind.CloseBracket);
        }
        Expect(TokenKind.KeywordOf);
        type.ValueType = ParseBaseTypeInst();
        return type;
    }

    private bool ParseBaseTypeInstTail(out TypeInst type)
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

    public bool ParseTypeInstAndId(out Binding<TypeInst> binding)
    {
        binding = default;
        if (!ParseTypeInst(out var type))
            return false;

        if (!Expect(TokenKind.Colon))
            return false;

        if (!ReadString(out var name))
            return false;

        binding = name.Bind(type);
        return true;
    }

    /// <summary>
    /// Parse a tuple type constructor
    /// </summary>
    /// <mzn>tuple(int, bool, tuple(int))<mzn>
    private bool ParseTupleType(out TupleTypeInst tuple)
    {
        tuple = new TupleTypeInst();
        if (!Skip(TokenKind.KeywordTuple))
            return false;

        if (!Expect(TokenKind.OpenParen))
            return false;

        next:
        if (_kind is TokenKind.CloseParen)
            goto end;

        if (!ParseTypeInst(out var ti))
            return false;

        tuple.Items.Add(ti);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseBracket);
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
        next:
        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        if (!ParseTypeInstAndId(out var field))
            return false;

        record.Fields.Add(field);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseBracket);
    }
}
