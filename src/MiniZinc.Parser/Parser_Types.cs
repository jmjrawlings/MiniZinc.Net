namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    public bool ParseBaseType(out TypeSyntax type)
    {
        type = default!;
        var start = _token;

        if (Skip(TokenKind.ANY))
        {
            type = new TypeSyntax(start) { Kind = TypeKind.Any };
            return true;
        }

        var var = false;
        if (Skip(TokenKind.VAR))
            var = true;
        else
            Skip(TokenKind.PAR);

        var opt = Skip(TokenKind.OPT);

        if (Skip(TokenKind.SET))
        {
            if (!Expect(TokenKind.OF))
                return false;

            if (!ParseBaseType(out type))
                return false;

            type = new SetTypeSyntax(start)
            {
                Items = type,
                Var = var,
                Opt = opt,
                Kind = TypeKind.Set
            };
            return true;
        }

        if (!ParseBaseTypeTail(out type))
            return false;

        type.Var = var;
        type.Opt = opt;
        return true;
    }

    /// <summary>
    /// Parse an array type
    /// </summary>
    /// <mzn>array[X, 1..2] of var int</mzn>
    public bool ParseArrayType(out ArrayTypeSyntax arr)
    {
        arr = default!;
        var dims = new List<SyntaxNode>();

        if (!Expect(TokenKind.ARRAY, out var start))
            return false;

        if (!Expect(TokenKind.OPEN_BRACKET))
            return false;

        next:
        if (!ParseBaseType(out var expr))
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

        arr = new ArrayTypeSyntax(start)
        {
            Kind = TypeKind.Array,
            Dimensions = dims,
            Items = type
        };
        return true;
    }

    private bool ParseBaseTypeTail(out TypeSyntax type)
    {
        type = default!;
        var start = _token;
        switch (_kind)
        {
            case TokenKind.INT:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Int };
                break;

            case TokenKind.BOOL:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Bool };
                break;

            case TokenKind.FLOAT:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Float };
                break;

            case TokenKind.STRING:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.String };
                break;

            case TokenKind.ANN:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Annotation };
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

            case TokenKind.GENERIC:
                Step();
                type = new NamedType(start, _token) { Kind = TypeKind.Generic };
                break;

            case TokenKind.GENERIC_SEQ:
                Step();
                type = new NamedType(start, _token) { Kind = TypeKind.GenericSeq };
                break;

            default:
                if (!ParseExpr(out var expr))
                    return false;
                type = new ExprType(start, expr) { Kind = TypeKind.Expr };
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
    public bool ParseTypeAndName(out (Token, TypeSyntax) result)
    {
        result = default;
        if (!ParseType(out var type))
            return false;

        if (type.Kind is not TypeKind.Any)
            if (!Expect(TokenKind.COLON))
                return false;

        if (!ParseIdent(out var name))
            return false;

        result = (name, type);
        return true;
    }

    /// <summary>
    /// Parse a tuple type constructor
    /// </summary>
    /// <mzn>tuple(int, bool, tuple(int))</mzn>
    private bool ParseTupleType(out TupleTypeSyntax tuple)
    {
        tuple = null!;
        if (!Expect(TokenKind.TUPLE, out var start))
            return false;

        tuple = new TupleTypeSyntax(start) { Kind = TypeKind.Tuple };
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
    private bool ParseRecordType(out RecordTypeSyntax record)
    {
        record = default!;
        if (!Expect(TokenKind.RECORD, out var start))
            return false;

        record = new RecordTypeSyntax(start) { Kind = TypeKind.Record };

        if (!ParseParameters(out var fields))
            return false;

        record.Items = fields!;
        return true;
    }

    /// <summary>
    /// Parse a comma separated list of types
    /// and names between parentheses
    /// </summary>
    /// <mzn>(int: a, bool: b)</mzn>
    private bool ParseParameters(out List<(Token, TypeSyntax)>? parameters)
    {
        parameters = default;
        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        if (_token.Kind is TokenKind.CLOSE_PAREN)
            goto end;

        next:
        if (!ParseTypeAndName(out var type))
            return false;

        parameters = new List<(Token, TypeSyntax)> { type };
        if (Skip(TokenKind.COMMA))
            goto next;

        end:
        return Expect(TokenKind.CLOSE_PAREN);
    }

    /// <summary>
    /// Parse a comma separated list of expressions
    /// between parentheses
    /// </summary>
    /// <mzn>(1, 2, false)</mzn>
    private bool ParseExprs(out List<SyntaxNode>? exprs, TokenKind open, TokenKind close)
    {
        exprs = null;
        if (!Skip(open))
            return false;

        exprs = new List<SyntaxNode>();
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

    public bool ParseType(out TypeSyntax result)
    {
        result = default!;
        var start = _token;
        if (_kind is TokenKind.ARRAY)
        {
            if (!ParseArrayType(out var arr))
                return false;
            result = arr;
            return true;
        }

        if (_kind is TokenKind.LIST)
        {
            if (!ParseListType(out var list))
                return false;
            result = list;
            return true;
        }

        if (!ParseBaseType(out var type))
            return false;

        if (!Skip(TokenKind.PLUS_PLUS))
        {
            result = type;
            return true;
        }

        // Complex types
        // `record(a: int) ++ record(b: int)`
        var complex = new ComplexTypeSyntax(start) { Kind = TypeKind.Complex };
        complex.Types.Add(type);
        result = complex;

        while (true)
        {
            if (!ParseBaseType(out type))
                return false;
            complex.Types.Add(type);
            if (!Skip(TokenKind.PLUS_PLUS))
                break;
        }

        return true;
    }

    /// <summary>
    /// Parse a list type
    /// </summary>
    /// <mzn>list of var int</mzn>
    private bool ParseListType(out ListTypeSyntax arr)
    {
        arr = null!;

        if (!Expect(TokenKind.LIST, out var start))
            return false;

        if (!Expect(TokenKind.OF))
            return false;

        if (!ParseBaseType(out var items))
            return false;

        arr = new ListTypeSyntax(start) { Kind = TypeKind.List, Items = items };
        return true;
    }
}
