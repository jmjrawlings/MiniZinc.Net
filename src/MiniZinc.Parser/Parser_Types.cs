namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    public bool ParseBaseTypeInst(out TypeInst type)
    {
        var inst = TypeFlags.Par;
        if (Skip(TokenKind.VAR))
            inst = TypeFlags.Var;
        else
            Skip(TokenKind.PAR);

        if (Skip(TokenKind.OPT))
            inst |= TypeFlags.Opt;

        if (Skip(TokenKind.SET))
        {
            Expect(TokenKind.OF);
            inst |= TypeFlags.Set;
        }

        if (!ParseBaseTypeTail(out type))
            return false;

        type.Flags = inst;
        return true;
    }

    /// <summary>
    /// Parse an array type
    /// </summary>
    /// <mzn>array[X, 1..2} of var int</mzn>
    public bool ParseArrayType(out ArrayTypeInst arr)
    {
        arr = default!;
        if (!Skip(TokenKind.ARRAY))
            return false;

        var dims = new List<INode>();
        arr = new ArrayTypeInst { Kind = TypeKind.Array };
        arr.Dimensions = dims;
        if (!Skip(TokenKind.OPEN_BRACKET))
            return false;
        next:
        if (!ParseBaseTypeInst(out var expr))
            return false;
        dims.Add(expr);
        if (Skip(TokenKind.COMMA))
            goto next;
        if (!Expect(TokenKind.CLOSE_BRACKET))
            return false;

        if (!Expect(TokenKind.OF))
            return false;

        if (!ParseType(out var type))
            return false;

        arr.Type = type;
        return true;
    }

    private bool ParseBaseTypeTail(out TypeInst type)
    {
        type = default!;
        switch (_token.Kind)
        {
            case TokenKind.INT:
                Step();
                type = new TypeInst { Kind = TypeKind.Int };
                break;

            case TokenKind.BOOL:
                Step();
                type = new TypeInst { Kind = TypeKind.Bool };
                break;

            case TokenKind.FLOAT:
                Step();
                type = new TypeInst { Kind = TypeKind.Float };
                break;

            case TokenKind.STRING:
                Step();
                type = new TypeInst { Kind = TypeKind.String };
                break;

            case TokenKind.ANN:
                Step();
                type = new TypeInst { Kind = TypeKind.Annotation };
                break;

            case TokenKind.RECORD:
                if (!ParseRecordType(out var rec))
                    return false;
                type = rec;
                break;

            case TokenKind.TUPLE:
                if (!ParseTupleType(out var tup))
                    return false;
                type = tup;
                break;

            case TokenKind.ANY:
                Step();
                if (!Expect(TokenKind.POLYMORPHIC))
                    return false;
                type = new TypeInst { Kind = TypeKind.Any, Name = _token.String! };
                break;

            case TokenKind.POLYMORPHIC:
                Step();
                if (!ParseIdent(out var id))
                    return false;
                type = new TypeInst { Name = id, Kind = TypeKind.Any };
                break;

            default:
                if (!ParseExpr(out var expr))
                    return false;
                type = new ExprTypeInst { Expr = expr };
                type.Kind = TypeKind.Expr;
                break;
        }

        return true;
    }

    /// <summary>
    /// Parse a type name pair
    /// </summary>
    /// <mzn>int: a</mzn>
    /// <mzn>bool: ABC</mzn>
    /// <mzn>any $$T</mzn>
    public bool ParseTypeAndName(out Binding<TypeInst> result)
    {
        result = default;
        if (!ParseType(out var type))
            return false;

        if (type.Kind is not TypeKind.Any)
            if (!Expect(TokenKind.COLON))
                return false;

        if (!ParseIdent(out var name))
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
        tuple = default!;
        if (!Skip(TokenKind.TUPLE))
            return false;

        tuple = new TupleTypeInst { Kind = TypeKind.Tuple };
        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        while (_kind is not TokenKind.CLOSE_PAREN)
        {
            if (!ParseType(out var ti))
                return false;

            tuple.Items.Add(ti);
            if (!Skip(TokenKind.COMMA))
                break;
        }

        return Expect(TokenKind.CLOSE_PAREN);
    }

    /// <summary>
    /// Parse a record type constructor
    /// </summary>
    /// <mzn>record(int: a, bool: b)</mzn>
    private bool ParseRecordType(out RecordTypeInst record)
    {
        record = default!;
        if (!Skip(TokenKind.RECORD))
            return false;
        record = new RecordTypeInst { Kind = TypeKind.Record };
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
    private bool ParseParameters(out List<Binding<TypeInst>>? parameters)
    {
        parameters = default;
        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        if (_token.Kind is TokenKind.CLOSE_PAREN)
            goto end;

        next:
        if (!ParseTypeAndName(out var type))
            return false;

        parameters = new List<Binding<TypeInst>> { type };
        if (Skip(TokenKind.COMMA))
            goto next;

        end:
        return Expect(TokenKind.CLOSE_PAREN);
    }

    // private bool ParseArgs(out List<INode>? exprs) =>
    //     ParseExprs(out exprs, TokenKind.OPEN_PAREN, TokenKind.CLOSE_PAREN);

    /// <summary>
    /// Parse a comma separated list of expressions
    /// between parentheses
    /// </summary>
    /// <mzn>(1, 2, false)</mzn>
    private bool ParseExprs(out List<INode>? exprs, TokenKind open, TokenKind close)
    {
        exprs = null;
        if (!Skip(open))
            return false;

        exprs = new List<INode>();
        while (_kind != close)
        {
            if (!ParseExpr(out var expr))
                return false;
            exprs.Add(expr);
            if (!Skip(TokenKind.COMMA))
                break;
        }

        if (!Expect(close))
            return false;

        return true;
    }

    public bool ParseType(out TypeInst type)
    {
        type = default!;
        switch (_token.Kind)
        {
            case TokenKind.ARRAY:
            case TokenKind.LIST:
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
