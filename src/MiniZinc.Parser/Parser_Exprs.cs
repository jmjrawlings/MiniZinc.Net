﻿namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    /// <summary>
    /// Parse an expression prefix / atom
    /// </summary>
    /// <mzn>100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1]</mzn>
    /// <mzn>record.field</mzn>
    public bool ParseExprAtom(out SyntaxNode expr)
    {
        expr = null!;
        var token = Step();

        switch (token.Kind)
        {
            case TokenKind.INT_LIT:
                expr = new IntLiteralSyntax(token);
                break;

            case TokenKind.FLOAT_LIT:
                expr = new FloatLiteralSyntax(token);
                break;

            case TokenKind.TRUE:
                expr = new BoolSyntax(token);
                break;

            case TokenKind.FALSE:
                expr = new BoolSyntax(token);
                break;

            case TokenKind.STRING_LIT:
                expr = new StringLiteralSyntax(token);
                break;

            case TokenKind.PLUS:
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Positive, expr);
                break;

            case TokenKind.MINUS:
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Negative, expr);
                break;

            case TokenKind.NOT:
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Not, expr);
                break;

            case TokenKind.DOT_DOT:
                if (ParseExpr(out var right))
                    expr = new RangeLiteralSyntax(token, Upper: expr);
                else if (Err)
                    return false;
                else
                    expr = new RangeLiteralSyntax(token);
                break;

            case TokenKind.UNDERSCORE:
                expr = new WildCardExpr(token);
                break;

            case TokenKind.OPEN_PAREN:
                if (!ParseParenExpr(token, out expr))
                    return false;
                break;

            case TokenKind.OPEN_BRACE:
                if (!ParseBraceExpr(token, out expr))
                    return false;
                break;

            case TokenKind.OPEN_BRACKET:
                if (!ParseBracketExpr(token, out expr))
                    return false;
                break;

            case TokenKind.IF:
                if (!ParseIfElseExpr(token, out var ite))
                    return false;
                expr = ite;
                break;

            case TokenKind.LET:
                if (!ParseLetExpr(token, out var let))
                    return false;
                expr = let;
                break;

            case TokenKind.IDENT:
            case TokenKind.QUOTED_IDENT:
                if (!ParseIdentExpr(token, ref expr))
                    return false;
                break;

            case TokenKind.EMPTY:
                expr = new EmptyExpr(token);
                return true;

            default:
                return false;
        }

        if (!ParseExprAtomTail(ref expr))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        expr.Annotations = anns;
        return true;
    }

    /// <summary>
    /// Parse the tail of an expression atom
    /// </summary>
    /// <returns>True if no errors were encountered</returns>
    public bool ParseExprAtomTail(ref SyntaxNode expr)
    {
        while (true)
        {
            if (Skip(TokenKind.OPEN_BRACKET))
            {
                // Array access eg: `a[1,2]`
                var access = new List<SyntaxNode>();
                while (_kind is not TokenKind.CLOSE_BRACKET)
                {
                    if (!ParseExpr(out var index))
                        return false;
                    access.Add(index);
                    if (!Skip(TokenKind.COMMA))
                        break;
                }

                expr = new ArrayAccessExpr(expr, access);
                if (!Expect(TokenKind.CLOSE_BRACKET))
                    return false;
            }
            else if (Skip(TokenKind.DOT))
            {
                // Tuple access: `a.1`
                if (ParseInt(out var i))
                    expr = new TupleAccessSyntax(expr, i);
                // Record access: `a.name`
                else if (ParseIdent(out var field))
                    expr = new RecordAccessSyntax(expr, field);
                // A float token indicates chained tuple access
                // eg `item.1.2`
                // todo - handle in lexer?
                else if (ParseFloat(out var f))
                {
                    var s = f.ToString("F1");
                    var t = s.Split(".");
                    // i = int.Parse(t[0]);
                    // int j = int.Parse(t[1]);
                    expr = new TupleAccessSyntax(new TupleAccessSyntax(expr, i), default);
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

    private bool Okay => ErrorString is null;

    /// <summary>
    /// Parse an Expression
    /// </summary>
    /// <mzn>a + b + 100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1] * arr[2]</mzn>
    /// TODO - Operator precedence parsing
    public bool ParseExpr(out SyntaxNode expr, ushort minPrecedence = ushort.MaxValue)
    {
        if (!ParseExprAtom(out expr))
            return false;

        while (true)
        {
            _precedence = Precedence(_kind);
            if (_precedence is 0)
                break;

            if (_precedence > minPrecedence)
                break;

            if (!ParseInfixExpr(ref expr))
                return false;
        }

        return true;
    }

    private bool ParseBinopExpr(ref SyntaxNode left, Operator op)
    {
        Step();
        if (!ParseExpr(out var right, _precedence))
            return false;
        left = new BinaryOperatorSyntax(left, op, right);
        return true;
    }

    public bool ParseInfixExpr(ref SyntaxNode left) =>
        _kind switch
        {
            TokenKind.DOUBLE_ARROW => ParseBinopExpr(ref left, Operator.Equivalent),
            TokenKind.RIGHT_ARROW => ParseBinopExpr(ref left, Operator.Implies),
            TokenKind.LEFT_ARROW => ParseBinopExpr(ref left, Operator.ImpliedBy),
            TokenKind.DOWN_WEDGE => ParseBinopExpr(ref left, Operator.Or),
            TokenKind.XOR => ParseBinopExpr(ref left, Operator.Xor),
            TokenKind.UP_WEDGE => ParseBinopExpr(ref left, Operator.And),
            TokenKind.LESS_THAN => ParseBinopExpr(ref left, Operator.LessThan),
            TokenKind.GREATER_THAN => ParseBinopExpr(ref left, Operator.GreaterThan),
            TokenKind.LESS_THAN_EQUAL => ParseBinopExpr(ref left, Operator.LessThan),
            TokenKind.GREATER_THAN_EQUAL => ParseBinopExpr(ref left, Operator.GreaterThanEqual),
            TokenKind.EQUAL => ParseBinopExpr(ref left, Operator.Equal),
            TokenKind.NOT_EQUAL => ParseBinopExpr(ref left, Operator.NotEqual),
            TokenKind.IN => ParseBinopExpr(ref left, Operator.In),
            TokenKind.SUBSET => ParseBinopExpr(ref left, Operator.Subset),
            TokenKind.SUPERSET => ParseBinopExpr(ref left, Operator.Superset),
            TokenKind.UNION => ParseBinopExpr(ref left, Operator.Union),
            TokenKind.DIFF => ParseBinopExpr(ref left, Operator.Diff),
            TokenKind.SYMDIFF => ParseBinopExpr(ref left, Operator.SymDiff),
            TokenKind.DOT_DOT => ParseDotDotExpr(ref left),
            TokenKind.PLUS => ParseBinopExpr(ref left, Operator.Add),
            TokenKind.MINUS => ParseBinopExpr(ref left, Operator.Subtract),
            TokenKind.STAR => ParseBinopExpr(ref left, Operator.Multiply),
            TokenKind.DIV => ParseBinopExpr(ref left, Operator.Div),
            TokenKind.MOD => ParseBinopExpr(ref left, Operator.Mod),
            TokenKind.FWDSLASH => ParseBinopExpr(ref left, Operator.Divide),
            TokenKind.INTERSECT => ParseBinopExpr(ref left, Operator.Intersect),
            TokenKind.EXP => ParseBinopExpr(ref left, Operator.Exponent),
            TokenKind.PLUS_PLUS => ParseBinopExpr(ref left, Operator.Concat),
            TokenKind.DEFAULT => ParseBinopExpr(ref left, Operator.Default),
            TokenKind.QUOTED_OP => ParseQuotedBinopExpr(ref left),
            TokenKind.QUOTED_IDENT => ParseQuotedBinopExpr(ref left),
            TokenKind.TILDE_PLUS => ParseBinopExpr(ref left, Operator.TildeAdd),
            TokenKind.TILDE_MINUS => ParseBinopExpr(ref left, Operator.TildeSubtract),
            TokenKind.TILDE_STAR => ParseBinopExpr(ref left, Operator.TildeMultiply),
            TokenKind.TILDE_EQUALS => ParseBinopExpr(ref left, Operator.TildeEqual),
            _ => Error("Expected binary operator")
        };

    private bool ParseQuotedBinopExpr(ref SyntaxNode left)
    {
        throw new NotImplementedException();
    }

    private bool ParseDotDotExpr(ref SyntaxNode left)
    {
        Expect(TokenKind.DOT_DOT);
        if (ParseExpr(out var right, _precedence))
            left = new RangeLiteralSyntax(left.Start, left, right);
        else if (Err)
            return false;
        else
            left = new RangeLiteralSyntax(left.Start, left);
        return true;
    }

    private static ushort Precedence(in TokenKind kind) =>
        kind switch
        {
            TokenKind.DOUBLE_ARROW => 1200,
            TokenKind.RIGHT_ARROW => 1100,
            TokenKind.LEFT_ARROW => 1100,
            TokenKind.DOWN_WEDGE => 1000,
            TokenKind.XOR => 1000,
            TokenKind.UP_WEDGE => 900,
            TokenKind.LESS_THAN => 800,
            TokenKind.GREATER_THAN => 800,
            TokenKind.LESS_THAN_EQUAL => 800,
            TokenKind.GREATER_THAN_EQUAL => 800,
            TokenKind.EQUAL => 800,
            TokenKind.NOT_EQUAL => 800,
            TokenKind.IN => 700,
            TokenKind.SUBSET => 700,
            TokenKind.SUPERSET => 700,
            TokenKind.UNION => 700,
            TokenKind.DIFF => 700,
            TokenKind.SYMDIFF => 700,
            TokenKind.DOT_DOT => 500,
            TokenKind.PLUS => 400,
            TokenKind.MINUS => 400,
            TokenKind.STAR => 300,
            TokenKind.DIV => 300,
            TokenKind.MOD => 300,
            TokenKind.FWDSLASH => 300,
            TokenKind.INTERSECT => 300,
            TokenKind.EXP => 200,
            TokenKind.PLUS_PLUS => 100,
            TokenKind.DEFAULT => 70,
            TokenKind.QUOTED_OP => 50,
            TokenKind.QUOTED_IDENT => 50,
            TokenKind.TILDE_PLUS => 10,
            TokenKind.TILDE_MINUS => 10,
            TokenKind.TILDE_STAR => 10,
            TokenKind.TILDE_EQUALS => 10,
            _ => 0
        };

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
    public bool ParseIdentExpr(in Token name, ref SyntaxNode result)
    {
        // Simple identifier
        if (!Skip(TokenKind.OPEN_PAREN))
        {
            result = new IdentifierSyntax(name);
            return true;
        }

        // Function call without arguments
        if (Skip(TokenKind.CLOSE_PAREN))
        {
            result = new CallSyntax(name);
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
        var exprs = new List<SyntaxNode>();
        bool maybeGen = true;
        bool isGen = false;
        int i;

        next:
        if (!ParseExpr(out var expr))
            return false;

        switch (expr)
        {
            // Identifier are valid in either call or gencall
            case IdentifierSyntax id:
                exprs.Add(expr);
                break;

            // `in` expressions involving identifiers could mean a gencall
            case BinaryOperatorSyntax
            {
                Op: Operator.In,
                Left: IdentifierSyntax id,
                Right: { } from
            } when maybeGen:
                if (!Skip(TokenKind.WHERE))
                {
                    exprs.Add(expr);
                    break;
                }

                // The where indicates this definitely is a gencall
                isGen = true;
                if (!ParseExpr(out var where))
                    return false;

                expr = new GeneratorSyntax(name)
                {
                    From = from,
                    Where = where,
                    Names = new() { id }
                };
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
            result = new CallSyntax(name) { Args = exprs };
            return true;
        }

        // Could be a gencall if followed by (
        if (!isGen && _kind is not TokenKind.OPEN_PAREN)
        {
            result = new CallSyntax(name) { Args = exprs };
            return true;
        }

        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        if (!ParseExpr(out var yields))
            return false;

        if (!Expect(TokenKind.CLOSE_PAREN))
            return false;

        var generators = new List<GeneratorSyntax>();
        var gencall = new GeneratorCallSyntax(name, yields, generators);

        List<IdentifierSyntax>? idents = null;
        for (i = 0; i < exprs.Count; i++)
        {
            switch (exprs[i])
            {
                // Identifiers must be collected as they are part of generators
                case IdentifierSyntax id:
                    idents ??= new List<IdentifierSyntax>();
                    idents.Add(id);
                    break;
                // Already created generators get added
                case GeneratorSyntax g:
                    g.Names.AddRange(idents!);
                    idents = null;
                    generators.Add(g);
                    break;
                // Binops are now known to be generators
                case BinaryOperatorSyntax binop:
                    idents ??= new List<IdentifierSyntax>();
                    idents.Add((IdentifierSyntax)binop.Left);
                    var gen = new GeneratorSyntax(default) { Names = idents, From = binop.Right };
                    generators.Add(gen);
                    idents = null;
                    break;
            }
        }
        result = gencall;
        return true;
    }

    private bool ParseGenerators(List<GeneratorSyntax> generators)
    {
        var start = _token;
        begin:
        var names = new List<IdentifierSyntax>();
        while (true)
        {
            IdentifierSyntax name;
            if (Skip(TokenKind.UNDERSCORE))
                name = new IdentifierSyntax(_token);
            else if (ParseIdent(out var id))
                name = new IdentifierSyntax(id);
            else
                return Error("Expected identifier in generator names");

            names.Add(name);
            if (Skip(TokenKind.COMMA))
                continue;

            break;
        }

        if (!Expect(TokenKind.IN))
            return false;

        if (!ParseExpr(out var @from))
            return false;

        var gen = new GeneratorSyntax(start) { Names = names, From = @from };

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
    public bool ParseBracketExpr(in Token start, out SyntaxNode result)
    {
        result = null!;

        if (_kind is TokenKind.CLOSE_BRACKET)
        {
            result = new Array1DExpr(start);
            return Expect(TokenKind.CLOSE_BRACKET);
        }

        if (Skip(TokenKind.PIPE))
        {
            if (_kind is TokenKind.PIPE)
            {
                if (!Parse3dArrayLiteral(start, out var arr3d))
                    return false;
                result = arr3d;
                return true;
            }
            else
            {
                if (!Parse2dArrayLiteral(start, out var arr2d))
                    return false;
                result = arr2d;
                return true;
            }
        }

        if (!Parse1dArrayLiteral(start, out result))
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
    bool Parse1dArrayLiteral(in Token start, out SyntaxNode result)
    {
        result = default!;
        SyntaxNode index;
        SyntaxNode element;

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
            var comp = new ComprehensionSyntax(start, value)
            {
                IsSet = false,
                Generators = new List<GeneratorSyntax>()
            };
            result = comp;
            if (!ParseGenerators(comp.Generators))
                return false;
            return Expect(TokenKind.CLOSE_BRACKET);
        }

        // 1D Array literal
        var arr1d = new Array1DExpr(start);
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
    private bool Parse2dArrayLiteral(in Token start, out Array2dExpr arr)
    {
        arr = new Array2dExpr(start);
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

        arr.I = 1;
        arr.J = j;

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
        arr.I++;
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

    /* 3D array literal
     * (1 x 2 x 2)
     *  `[| | 1,2|3,4| |]`
     *
     * (3, 1, 1)
     *  `[| |1|,|2|,|3| |]`
     *
     * (2,2,2)
     * `[| |1,1|1,1|, |2,2|2,2|, |3,3|3,3| |]`
     *
     */
    private bool Parse3dArrayLiteral(in Token start, out Array3dExpr arr)
    {
        arr = new Array3dExpr(start);

        if (!Expect(TokenKind.PIPE))
            return false;

        // Check for empty literal
        // `[| | | |]`
        if (Skip(TokenKind.PIPE))
        {
            Expect(TokenKind.PIPE);
            Expect(TokenKind.CLOSE_BRACKET);
            return Okay;
        }

        int i = 0;

        table:
        i++;
        int j = 0;

        row:
        j++;
        int k = 0;

        while (_kind is not TokenKind.PIPE)
        {
            if (!ParseExpr(out var expr))
                return false;
            k++;
            arr.Elements.Add(expr);
            if (!Skip(TokenKind.COMMA))
                break;
        }
        if (!Expect(TokenKind.PIPE))
            return false;

        if (Skip(TokenKind.COMMA))
        {
            if (!Expect(TokenKind.PIPE))
                return false;
            goto table;
        }

        if (!Skip(TokenKind.PIPE))
            goto row;

        arr.I = i;
        arr.J = j;
        arr.K = k;
        if (!Expect(TokenKind.CLOSE_BRACKET))
            return false;
        return true;
    }

    /// <summary>
    /// Parse anything that starts with a '{', this could be a
    /// set literal or set comprehension
    /// </summary>
    /// <mzn>{1,2,3}</mzn>
    /// <mzn>{ x | x in [1,2,3]}</mzn>
    private bool ParseBraceExpr(in Token start, out SyntaxNode result)
    {
        result = null!;

        // Empty Set
        if (_kind is TokenKind.CLOSE_BRACE)
        {
            result = new SetLiteralSyntax(start);
            return Expect(TokenKind.CLOSE_BRACE);
        }

        // Parse first expression
        if (!ParseExpr(out var element))
            return false;

        // Set comprehension
        if (Skip(TokenKind.PIPE))
        {
            var comp = new ComprehensionSyntax(start, element)
            {
                IsSet = true,
                Generators = new List<GeneratorSyntax>()
            };
            if (!ParseGenerators(comp.Generators))
                return false;

            result = comp;
            return Expect(TokenKind.CLOSE_BRACE);
        }

        // Set literal
        var set = new SetLiteralSyntax(start);
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

    public bool ParseLetExpr(in Token start, out LetSyntax let)
    {
        let = null!;

        if (!Expect(TokenKind.OPEN_BRACE))
            return false;

        let = new LetSyntax(start);

        while (_kind is not TokenKind.CLOSE_BRACE)
        {
            Step();
            if (!ParseLetLocal(out var local))
                return false;

            let.Locals ??= new List<ILetLocal>();
            let.Locals.Add(local);

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

    private bool ParseLetLocal(out ILetLocal result)
    {
        result = null!;
        if (_kind is TokenKind.CONSTRAINT)
        {
            if (ParseConstraintItem(out var con))
            {
                result = con;
                return true;
            }

            return false;
        }

        if (!ParseDeclareOrAssignItem(out var var, out var assign))
            return false;

        if (var is not null)
            result = var;
        else
            result = assign!;

        return true;
    }

    private bool ParseIfThenCase(out SyntaxNode @if, out SyntaxNode @then, TokenKind ifKeyword)
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
    private bool ParseIfElseExpr(in Token start, out SyntaxNode node)
    {
        node = null!;
        if (!ParseIfThenCase(out var ifCase, out var thenCase, TokenKind.IF))
            return false;

        var ite = new IfElseSyntax(start, ifCase, thenCase);
        node = ite;

        while (ParseIfThenCase(out ifCase, out thenCase, TokenKind.ELSEIF))
        {
            ite.ElseIfs ??= new List<(SyntaxNode elseif, SyntaxNode then)>();
            ite.ElseIfs.Add((ifCase, thenCase));
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
    private bool ParseParenExpr(in Token start, out SyntaxNode result)
    {
        result = null!;
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
            if (expr is not IdentifierSyntax id)
                return Expected("Identifier");

            var record = new RecordLiteralSyntax(start);
            result = record;
            if (!ParseExpr(out expr))
                return false;

            var name = id.Token;
            var field = (name, expr);
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

            field = (name, expr);
            record.Fields.Add(field);
            goto record_field;
        }

        // Else must be a tuple
        var tuple = new TupleLiteralSyntax(start);
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

    /// <summary>
    /// Parse annotations
    /// </summary>
    /// <returns>True if no error was encountered</returns>
    public bool ParseAnnotations(out List<SyntaxNode>? annotations)
    {
        annotations = null;
        while (Skip(TokenKind.COLON_COLON))
        {
            SyntaxNode? ann;
            var token = Step();
            // int : x :: output = 3;
            if (token.Kind is TokenKind.OUTPUT)
                ann = new IdentifierSyntax(token);
            else if (!ParseExprAtom(out ann))
            {
                Expected("Annotation");
                return false;
            }

            annotations ??= new List<SyntaxNode>();
            annotations.Add(ann);
        }
        return true;
    }

    /// <summary>
    /// Parse string literal annotations
    /// </summary>
    /// <mzn>constraint a > 2 :: "xd"</mzn>
    /// <mzn>output ["xd"] :: "dx"</mzn>
    /// <returns>True if no error was encountered</returns>
    public bool ParseStringAnnotations(out List<SyntaxNode>? annotations)
    {
        annotations = null;
        while (Skip(TokenKind.COLON_COLON))
        {
            if (!ParseString(out var ann))
                return false;
            annotations ??= new List<SyntaxNode>();
            annotations.Add(new StringLiteralSyntax(ann));
        }

        return true;
    }
}
