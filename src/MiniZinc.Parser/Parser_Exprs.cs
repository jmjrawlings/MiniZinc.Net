namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    /// <summary>
    /// Parse an Expression
    /// </summary>
    /// <mzn>a + b + 100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1] * arr[2]</mzn>
    public bool ParseExprAtom(out IExpr expr)
    {
        expr = Expr.Null;

        switch (_kind)
        {
            case TokenKind.PLUS:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOpExpr(Operator.Plus, expr);
                break;

            case TokenKind.MINUS:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOpExpr(Operator.Minus, expr);
                break;

            case TokenKind.NOT:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOpExpr(Operator.Not, expr);
                break;

            case TokenKind.DOT_DOT:
                Step();
                // Used for array indices eg `a[1..2, ..]`
                if (_kind is TokenKind.COMMA or TokenKind.CLOSE_BRACKET)
                {
                    expr = new RangeExpr();
                }
                else if (ParseExpr(out expr))
                    expr = new RangeExpr(Upper: expr);
                else
                    return Error();
                break;

            case TokenKind.UNDERSCORE:
                Step();
                expr = new WildCardExpr();
                break;

            case TokenKind.TRUE:
                Step();
                expr = Expr.Bool(true);
                break;

            case TokenKind.FALSE:
                Step();
                expr = Expr.Bool(false);
                break;

            case TokenKind.INT_LIT:
                expr = Expr.Int(_token.Int);
                Step();
                break;

            case TokenKind.FLOAT_LIT:
                expr = Expr.Float(_token.Double);
                Step();
                break;

            case TokenKind.STRING_LIT:
                expr = Expr.String(_token.String!);
                Step();
                break;

            case TokenKind.OPEN_PAREN:
                if (!ParseParenExpr(out expr))
                    return false;
                break;

            case TokenKind.OPEN_BRACE:
                if (!ParseBraceExpr(out expr))
                    return false;
                break;

            case TokenKind.OPEN_BRACKET:
                if (!ParseBracketExpr(out expr))
                    return false;
                break;

            case TokenKind.IF:
                if (!ParseIfElseExpr(out var ite))
                    return false;
                expr = ite;
                break;

            case TokenKind.LET:
                if (!ParseLetExpr(out var let))
                    return false;
                expr = let;
                break;

            case TokenKind.IDENT:
            case TokenKind.QUOTED_IDENT:
                if (!ParseIdentExpr(out expr))
                    return false;
                break;

            case TokenKind.EMPTY:
                Step();
                expr = Expr.Empty;
                return true;

            default:
                return false;
        }

        if (!ParseExprAtomTail(expr, out expr))
            return false;

        // ParseAnnotations()


        return Okay;
    }

    public bool ParseExprAtomTail(IExpr expr, out IExpr result)
    {
        result = expr;
        while (true)
        {
            if (Skip(TokenKind.OPEN_BRACKET))
            {
                // Array access eg: `a[1,2]`
                var access = new List<IExpr>();
                while (_kind is not TokenKind.CLOSE_BRACKET)
                {
                    if (!ParseExpr(out var index))
                        return false;
                    access.Add(index);
                    if (!Skip(TokenKind.COMMA))
                        break;
                }

                result = new ArrayAccessExpr(expr, access);
                if (!Expect(TokenKind.CLOSE_BRACKET))
                    return false;
            }
            else if (Skip(TokenKind.DOT))
            {
                // Tuple access: `a.1`
                if (ParseInt(out int i))
                    result = new TupleAccess(expr, i);
                // Record access: `a.name`
                else if (ParseIdent(out var field))
                    result = new RecordAccess(expr, field);
                // A float token indicates chained tuple access
                // eg `item.1.2`
                // todo - handle in lexer?
                else if (ParseFloat(out var f))
                {
                    var s = f.ToString("F1");
                    var t = s.Split(".");
                    i = int.Parse(t[0]);
                    int j = int.Parse(t[1]);
                    result = new TupleAccess(new TupleAccess(expr, i), j);
                }
                else
                    return Error("Expected a tuple field (eg .1) or record field (eg .name)");
            }
            else
            {
                break;
            }
        }
        return true;
    }

    private bool Okay => Err is null;

    /// <summary>
    /// Parse an Expression
    /// </summary>
    /// <mzn>a + b + 100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1] * arr[2]</mzn>
    public bool ParseExpr(out IExpr expr)
    {
        if (!ParseExprAtom(out expr))
            return false;

        while (ParseBinOp(out var op, out var id))
        {
            if (!ParseExpr(out var right))
            {
                // Postfix range operator `1..`
                if (op is Operator.Range)
                {
                    expr = Expr.Range(expr);
                    return Okay;
                }

                return false;
            }

            if (op is Operator.Range)
                expr = new RangeExpr(expr, right);
            else
                expr = new BinaryOpExpr(expr, op, id, right);
        }
        return true;
    }

    public bool ParseBinOp(out Operator op, out string id)
    {
        id = string.Empty;
        op = Operator.Add;

        switch (_kind)
        {
            case TokenKind.DOUBLE_ARROW:
                op = Operator.Equivalent;
                break;

            case TokenKind.RIGHT_ARROW:
                op = Operator.Implies;
                break;

            case TokenKind.LEFT_ARROW:
                op = Operator.ImpliedBy;
                break;

            case TokenKind.DOWN_WEDGE:
                op = Operator.Or;
                break;

            case TokenKind.XOR:
                op = Operator.Xor;
                break;

            case TokenKind.UP_WEDGE:
                op = Operator.And;
                break;

            case TokenKind.LESS_THAN:
                op = Operator.LessThan;
                break;

            case TokenKind.GREATER_THAN:
                op = Operator.GreaterThan;
                break;

            case TokenKind.LESS_THAN_EQUAL:
                op = Operator.LessThanEqual;
                break;

            case TokenKind.GREATER_THAN_EQUAL:
                op = Operator.GreaterThanEqual;
                break;

            case TokenKind.EQUAL:
                op = Operator.Equal;
                break;

            case TokenKind.NOT_EQUAL:
                op = Operator.NotEqual;
                break;

            case TokenKind.TILDE_EQUALS:
                op = Operator.TildeEqual;
                break;

            case TokenKind.IN:
                op = Operator.In;
                break;

            case TokenKind.SUBSET:
                op = Operator.Subset;
                break;

            case TokenKind.SUPERSET:
                op = Operator.Superset;
                break;

            case TokenKind.UNION:
                op = Operator.Union;
                break;

            case TokenKind.DIFF:
                op = Operator.Diff;
                break;

            case TokenKind.SYMDIFF:
                op = Operator.SymDiff;
                break;

            case TokenKind.DOT_DOT:
                op = Operator.Range;
                break;

            case TokenKind.INTERSECT:
                op = Operator.Intersect;
                break;

            case TokenKind.PLUS_PLUS:
                op = Operator.Concat;
                break;

            case TokenKind.DEFAULT:
                op = Operator.Default;
                break;

            case TokenKind.PLUS:
                op = Operator.Plus;
                break;

            case TokenKind.MINUS:
                op = Operator.Minus;
                break;

            case TokenKind.STAR:
                op = Operator.Multiply;
                break;

            case TokenKind.FWDSLASH:
                op = Operator.Divide;
                break;

            case TokenKind.DIV:
                op = Operator.Div;
                break;

            case TokenKind.MOD:
                op = Operator.Mod;
                break;

            case TokenKind.EXP:
                op = Operator.Exponent;
                break;

            case TokenKind.TILDE_PLUS:
                op = Operator.TildeAdd;
                break;

            case TokenKind.TILDE_MINUS:
                op = Operator.TildeSubtract;
                break;

            case TokenKind.TILDE_STAR:
                op = Operator.TildeMultiply;
                break;

            case TokenKind.QUOTED_OP:
            case TokenKind.QUOTED_IDENT:
                id = _token.String!;
                break;
            default:
                return false;
        }

        Step();
        return true;
    }

    /// <summary>
    /// Parses an expression that begins with an identifier.
    /// This could be one of:
    ///  - identifier
    ///  - function call
    ///  - generator call
    /// </summary>
    /// <mzn>hello</mzn>
    ///<mzn>something()</mzn>
    ///<mzn>sum(xd)</mzn>
    ///<mzn>forall(i in 1..3)(xd[i] > 0);</mzn>
    ///<mzn>forall(i,j in 1..3)(xd[i] > 0);</mzn>
    ///<mzn>forall(i in 1..3, j in 1..3 where i > j)(xd[i]);</mzn>
    public bool ParseIdentExpr(out IExpr result)
    {
        result = Expr.Null;

        if (!ParseIdent(out var name))
            return false;

        // Simple identifier
        if (!Skip(TokenKind.OPEN_PAREN))
        {
            result = Expr.Ident(name);
            return true;
        }

        // Function call without arguments
        if (Skip(TokenKind.CLOSE_PAREN))
        {
            result = new CallExpr { Name = name };
            return true;
        }

        /* This part is tricky because we need to determine
         * whether the call is a standard function call
         * like `max([1,2,3])` or a generator call like
         * `forall(x in 1..3 where x > 1)(xs[x])`
         *
         * Since something like `x in 1..3` could be either
         * a generator tail expr or a boolean expr we can't
         * correctly identify call versus gen-call until
         * later on.
         *
         * Backtracking would make this trivial but I think that's
         * more trouble than it's worth.
         */
        var exprs = new List<INode>();
        bool maybeGen = true;
        bool isGen = false;
        int i;

        next:
        if (!ParseExpr(out var expr))
            return false;

        switch (expr)
        {
            // Identifier are valid in either call or gencall
            case Identifier id:
                exprs.Add(expr);
                break;

            // `in` expressions involving identifiers could mean a gencall
            case BinaryOpExpr { Op: Operator.In, Left: Identifier id, Right: { } from }
                when maybeGen:
                if (!Skip(TokenKind.WHERE))
                {
                    exprs.Add(expr);
                    break;
                }

                // The where indicates this definitely is a gencall
                isGen = true;
                if (!ParseExpr(out var where))
                    return false;

                expr = new GeneratorExpr { From = from, Where = where };
                exprs.Add(id);
                exprs.Add(expr);
                break;

            default:
                if (isGen)
                    return Error($"Unexpected {expr} while parsing a gen-call");
                if (maybeGen)
                    maybeGen = false;
                exprs.Add(expr);
                break;
        }

        if (Skip(TokenKind.COMMA))
        {
            if (isGen)
                goto next;

            if (_kind is not TokenKind.CLOSE_PAREN)
                goto next;
        }

        if (!Expect(TokenKind.CLOSE_PAREN))
            return false;

        // For sure it's just a call
        if (!maybeGen)
        {
            result = new CallExpr { Name = name, Args = exprs };
            return true;
        }

        // Could be a gencall if followed by (
        if (!isGen && _kind is not TokenKind.OPEN_PAREN)
        {
            result = new CallExpr { Name = name, Args = exprs };
            return true;
        }

        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        if (!ParseExpr(out var yields))
            return false;

        if (!Expect(TokenKind.CLOSE_PAREN))
            return false;

        var generators = new List<GeneratorExpr>();
        var gencall = new GenCallExpr
        {
            Name = name,
            Generators = generators,
            Expr = yields
        };

        List<Identifier>? idents = null;
        for (i = 0; i < exprs.Count; i++)
        {
            switch (exprs[i])
            {
                // Identifiers must be collected as they are part of generators
                case Identifier id:
                    idents ??= new List<Identifier>();
                    idents.Add(id);
                    break;
                // Already created generators get added
                case GeneratorExpr g:
                    g.Names = idents!;
                    idents = null;
                    generators.Add(g);
                    break;
                // Binops are now known to be generators
                case BinaryOpExpr binop:
                    idents ??= new List<Identifier>();
                    idents.Add((Identifier)binop.Left);
                    var gen = new GeneratorExpr();
                    gen.Names = idents;
                    gen.From = binop.Right;
                    generators.Add(gen);
                    idents = null;
                    break;
            }
        }
        result = gencall;
        return true;
    }

    private bool ParseGenerators(List<GeneratorExpr> generators)
    {
        begin:
        var gen = new GeneratorExpr { Names = new List<Identifier>() };
        while (true)
        {
            Identifier name;
            if (Skip(TokenKind.UNDERSCORE))
                name = Identifier.Anonymous;
            else if (ParseIdent(out var id))
                name = Expr.Ident(id);
            else
                return Error("Expected identifier in generator names");

            gen.Names.Add(name);
            if (Skip(TokenKind.COMMA))
                continue;

            break;
        }

        if (!Expect(TokenKind.IN))
            return false;

        if (!ParseExpr(out var @from))
            return false;

        gen.From = @from;

        if (Skip(TokenKind.WHERE))
            if (!ParseExpr(out var @where))
                return false;
            else
                gen.Where = @where;

        generators.Add(gen);

        if (Skip(TokenKind.COMMA))
            goto begin;

        return true;
    }

    /// <summary>
    /// Parse anything that starts with a '[', which will
    /// be an array or array comprehension
    /// </summary>
    /// <mzn>[1,2,3]</mzn>
    /// <mzn>[ x | x in [a,b,c]]</mzn>
    public bool ParseBracketExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Skip(TokenKind.OPEN_BRACKET))
            return false;

        if (_kind is TokenKind.CLOSE_BRACKET)
        {
            result = new Array1DLit();
            return Expect(TokenKind.CLOSE_BRACKET);
        }

        if (Skip(TokenKind.PIPE))
        {
            if (!Parse2dArrayLiteral(out var arr2d))
                return false;
            result = arr2d;
            return true;
        }

        if (!Parse1dArrayLiteral(out result))
            return false;
        return true;
    }

    /*
     * 1D array literals.
     *
     * `[1, 2, 3, a, 5]`
     *
     * These could be of a simple form like:
     * Or an indexed form like
     * `[A:0, B:1, C:2]`
     *
     * Or a composite form like
     * `[0: A, B, C, D]`
     */
    bool Parse1dArrayLiteral(out IExpr result)
    {
        result = default!;
        IExpr index;
        IExpr element;

        // Parse the first element
        if (!ParseExpr(out var value))
            return false;

        // Determine if its an indexed array
        bool indexed = Skip(TokenKind.COLON);
        if (indexed)
        {
            index = value;
            if (!ParseExpr(out value))
                return false;
            element = new IndexAndValue(index, value);
        }
        else
        {
            element = value;
        }

        // Array comprehension
        if (Skip(TokenKind.PIPE))
        {
            var comp = new CompExpr
            {
                Expr = value,
                IsSet = false,
                Generators = new List<GeneratorExpr>()
            };
            result = comp;
            if (!ParseGenerators(comp.Generators))
                return false;
            return Expect(TokenKind.CLOSE_BRACKET);
        }

        // 1D Array literal
        var arr1d = new Array1DLit();
        result = arr1d;
        arr1d.Elements.Add(element);

        while (true)
        {
            if (!Skip(TokenKind.COMMA))
                return Expect(TokenKind.CLOSE_BRACKET);

            if (Skip(TokenKind.CLOSE_BRACKET))
                return true;

            if (indexed)
            {
                if (!ParseExpr(out index))
                    return false;
                if (!Skip(TokenKind.COLON))
                {
                    arr1d.Elements.Add(index);
                    indexed = false;
                    continue;
                }

                if (!ParseExpr(out value))
                    return false;
                element = new IndexAndValue(index, value);
            }
            else if (!ParseExpr(out value))
                return false;
            else
                element = value;

            arr1d.Elements.Add(element);
        }
    }

    /* 2D array literal
     * Simple:
     *  `[| 1, 2, 3 | 4, 5, 6 |]`
     *
     * Only column index:
     *  [| A: B: C:
     *   | 0, 0, 0
     *   | 1, 1, 1
     *   | 2, 2, 2 |];
     *
     * Only row index:
     * [| A: 0, 0, 0
     *  | B: 1, 1, 1
     *  | C: 2, 2, 2 |];
     *
     * Row and column index:
     * [|    A: B: C:
     *  | A: 0, 0, 0
     *  | B: 1, 1, 1
     *  | C: 2, 2, 2 |];
     */
    private bool Parse2dArrayLiteral(out Array2DLit arr)
    {
        arr = new Array2DLit();
        int j = 1;

        if (Skip(TokenKind.PIPE))
            return Expect(TokenKind.CLOSE_BRACKET);

        if (!ParseExpr(out var value))
            return false;

        if (!Skip(TokenKind.COLON))
        {
            // If first value is not an index skip the rest of the check
            arr.Elements.Add(value);
            Skip(TokenKind.COMMA);
            goto parse_row_values;
        }

        arr.Indices.Add(value);

        if (Skip(TokenKind.PIPE))
        {
            arr.ColIndexed = true;
            goto parse_row_values;
        }

        arr.ColIndexed = true;
        arr.RowIndexed = true;

        while (_kind is not TokenKind.PIPE)
        {
            j++;

            if (!ParseExpr(out value))
                return false;

            if (Skip(TokenKind.COLON))
            {
                if (!arr.ColIndexed)
                    return Error("Invalid : in row indexed array literal");

                arr.RowIndexed = false;
                arr.Indices.Add(value);
                continue;
            }

            arr.ColIndexed = false;
            arr.Elements.Add(value);

            if (!Skip(TokenKind.COMMA))
                break;
        }

        arr.Rows = 1;
        arr.Cols = j;

        if (!Expect(TokenKind.PIPE))
            return false;

        if (Skip(TokenKind.CLOSE_BRACKET))
            return true;

        /* Use the second row if necessary to detect dual
         * indexing */
        if (!arr.RowIndexed)
        {
            if (!ParseExpr(out value))
                return false;

            if (Skip(TokenKind.COLON))
            {
                arr.RowIndexed = true;
                arr.Indices.Add(value);
            }
            else
            {
                arr.Elements.Add(value);
                Skip(TokenKind.COMMA);
            }
            goto parse_row_values;
        }

        parse_row_index:
        if (!ParseExpr(out value))
            return false;

        if (!Expect(TokenKind.COLON))
            return false;

        arr.Indices.Add(value);

        parse_row_values:
        arr.Rows++;
        while (_kind is not TokenKind.PIPE)
        {
            if (!ParseExpr(out value))
                return false;

            arr.Elements.Add(value);

            if (!Skip(TokenKind.COMMA))
                break;
        }

        if (!Expect(TokenKind.PIPE))
            return false;

        if (Skip(TokenKind.CLOSE_BRACKET))
            return true;

        if (arr.RowIndexed)
            goto parse_row_index;
        else
            goto parse_row_values;
    }

    /// <summary>
    /// Parse anything that starts with a '{', this could be a
    /// set literal or set comprehension
    /// </summary>
    /// <mzn>{1,2,3}</mzn>
    /// <mzn>{ x | x in [1,2,3]}</mzn>
    private bool ParseBraceExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Skip(TokenKind.OPEN_BRACE))
            return false;

        // Empty Set
        if (_kind is TokenKind.CLOSE_BRACE)
        {
            result = new SetLit();
            return Expect(TokenKind.CLOSE_BRACE);
        }

        // Parse first expression
        if (!ParseExpr(out var element))
            return false;

        // Set comprehension
        if (Skip(TokenKind.PIPE))
        {
            var comp = new CompExpr
            {
                Expr = element,
                IsSet = true,
                Generators = new List<GeneratorExpr>()
            };
            if (!ParseGenerators(comp.Generators))
                return false;

            result = comp;
            return Expect(TokenKind.CLOSE_BRACE);
        }

        // Set literal
        var set = new SetLit();
        result = set;
        set.Elements.Add(element);
        Skip(TokenKind.COMMA);
        while (_kind is not TokenKind.CLOSE_BRACE)
        {
            if (!ParseExpr(out var item))
                return false;

            set.Elements.Add(item);
            if (!Skip(TokenKind.COMMA))
                break;
        }
        return Expect(TokenKind.CLOSE_BRACE);
    }

    public bool ParseLetExpr(out LetExpr let)
    {
        let = default!;

        if (!Expect(TokenKind.LET))
            return false;

        if (!Expect(TokenKind.OPEN_BRACE))
            return false;

        let = new LetExpr();
        while (_kind is not TokenKind.CLOSE_BRACE)
        {
            if (ParseConstraintItem(out var cons))
            {
                let.Locals ??= new List<ILetLocal>();
                let.Locals.Add(cons);
            }
            else if (ParseDeclareOrAssignItem(out var var, out var assign))
            {
                let.Locals ??= new List<ILetLocal>();
                if (var is not null)
                    let.Locals.Add(var);
                else
                    let.Locals.Add(assign!);
            }
            else if (Err is not null)
                return Error();

            if (Skip(TokenKind.EOL) || Skip(TokenKind.COMMA))
                continue;
            break;
        }

        if (!Expect(TokenKind.CLOSE_BRACE))
            return false;

        if (!Expect(TokenKind.IN))
            return false;

        if (!ParseExpr(out var body))
            return false;

        let.Body = body;
        return true;
    }

    private bool ParseIfThenCase(out IExpr @if, out IExpr @then, TokenKind ifKeyword)
    {
        @if = default!;
        @then = default!;
        if (!Skip(ifKeyword))
            return false;

        if (!ParseExpr(out @if))
            return false;

        if (!Skip(TokenKind.THEN))
            return false;

        if (!ParseExpr(out @then))
            return false;

        return true;
    }

    /// <summary>
    /// Parse an if-then-else expression
    /// </summary>
    /// <mzn>if x > 0 then y > 0 else true endif</mzn>
    /// <mzn>if z then 100 else 200 endif</mzn>
    private bool ParseIfElseExpr(out IfThenElseExpr ite)
    {
        ite = null!;
        if (!ParseIfThenCase(out var @if, out var @then, TokenKind.IF))
            return false;

        ite = new IfThenElseExpr();
        ite.If = @if;
        ite.Then = @then;

        while (ParseIfThenCase(out @if, out @then, TokenKind.ELSEIF))
        {
            ite.ElseIfs ??= new List<(IExpr elseif, IExpr then)>();
            ite.ElseIfs.Add((@if, @then));
        }

        if (Skip(TokenKind.ELSE))
        {
            if (!ParseExpr(out var @else))
                return false;

            ite.Else = @else;
        }

        if (!Expect(TokenKind.ENDIF))
            return false;

        return true;
    }

    /// <summary>
    /// Parse anything starting with a '(' eg:
    /// - (1)
    /// - (2,)
    /// - (a: 100, b:200)
    /// </summary>
    /// <returns></returns>
    private bool ParseParenExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        // Bracketed expr
        if (Skip(TokenKind.CLOSE_PAREN))
        {
            result = expr;
            return true;
        }

        // Record expr
        if (Skip(TokenKind.COLON))
        {
            if (expr is not Identifier id)
                return Error("Must be identifier");

            var name = id.s;
            var record = new RecordExpr();
            result = record;
            if (!ParseExpr(out expr))
                return false;

            var field = name.Bind(expr);
            record.Fields.Add(field);

            record_field:
            if (!Skip(TokenKind.COMMA))
                return Expect(TokenKind.CLOSE_PAREN);

            if (Skip(TokenKind.CLOSE_PAREN))
                return true;

            if (!ParseIdent(out name))
                return false;

            if (!Expect(TokenKind.COLON))
                return false;

            if (!ParseExpr(out expr))
                return false;

            field = name.Bind(expr);
            record.Fields.Add(field);
            goto record_field;
        }

        // Else must be a tuple
        var tuple = new TupleExpr();
        result = tuple;
        tuple.Fields.Add(expr);
        if (!Expect(TokenKind.COMMA))
            return false;
        while (_kind is not TokenKind.CLOSE_PAREN)
        {
            if (!ParseExpr(out expr))
                return false;
            tuple.Fields.Add(expr);
            if (!Skip(TokenKind.COMMA))
                break;
        }

        return Expect(TokenKind.CLOSE_PAREN);
    }

    public bool ParseAnnotations(IAnnotations target)
    {
        while (Skip(TokenKind.COLON_COLON))
        {
            IExpr? ann;
            if (Skip(TokenKind.OUTPUT))
                ann = Expr.Ident("output");
            else if (ParseAnnotation(out ann)) { }
            else
                return Error();

            target.Annotate(ann);
        }

        return true;
    }

    public bool ParseStringAnnotations(IAnnotations target)
    {
        while (Skip(TokenKind.COLON_COLON))
        {
            if (!ParseString(out var ann))
                return false;
            target.Annotate(Expr.String(ann));
        }

        return true;
    }

    public bool ParseAnnotation(out IExpr expr) => ParseExprAtom(out expr);
}
