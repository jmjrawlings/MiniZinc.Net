namespace MiniZinc.Parser;

using System.Diagnostics;
using Ast;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed class Parser
{
    private readonly Token[] _tokens;
    private readonly Stopwatch _watch;
    private string _text;
    private Token _token;
    private TokenKind _kind;
    private ushort _precedence;
    private int _i;
    internal string? ErrorString { get; private set; }
    internal TimeSpan Elapsed => _watch.Elapsed;

    public static Parser ParseFile(string path)
    {
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        return parser;
    }

    public static Parser ParseText(string text)
    {
        var parser = new Parser(text);
        return parser;
    }

    internal Parser(string text)
    {
        _watch = Stopwatch.StartNew();
        using var lexer = Lexer.Lex(text);
        _tokens = lexer.ToArray();
        _text = text;
        _i = 0;
        Step();
    }

    /// Progress the parser
    private bool Step()
    {
        if (_i >= _tokens.Length)
        {
            _kind = TokenKind.EOF;
            return false;
        }

        _token = _tokens[_i++];
        _kind = _token.Kind;
        return true;
    }

    /// Progress the parser if the current token is of the given kind
    private bool Skip(TokenKind kind)
    {
        if (_token.Kind != kind)
            return false;
        Step();
        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind)
    {
        if (_kind != kind)
            return Expected($"a {kind} but encountered a {_token.Kind}");

        Step();
        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind, out Token token)
    {
        if (_kind != kind)
        {
            token = default;
            return Expected($"a {kind} but encountered a {_token.Kind}");
        }

        token = _token;
        Step();
        return true;
    }

    private bool ParseInt(out Token token)
    {
        if (_kind is TokenKind.INT_LIT)
        {
            token = _token;
            Step();
            return true;
        }

        token = default;
        return false;
    }

    /// <summary>
    /// Read the current tokens string into the given variable
    /// </summary>
    private bool ParseString(out Token result, TokenKind kind = TokenKind.STRING_LIT)
    {
        if (_kind == kind)
        {
            result = _token;
            Step();
            return true;
        }

        result = default;
        return false;
    }

    private bool ParseFloat(out double f)
    {
        if (_kind is TokenKind.FLOAT_LIT)
        {
            f = _token.DoubleValue;
            Step();
            return true;
        }

        f = default;
        return false;
    }

    private bool ParseIdent(out Token token)
    {
        if (_kind is TokenKind.IDENT or TokenKind.QUOTED_IDENT)
        {
            token = _token;
            Step();
            return true;
        }
        else
        {
            token = default;
            return false;
        }
    }

    /// <summary>
    /// Parse a model
    /// </summary>
    /// <mzn>var 1..10: a; var 10..20: b; constraint a = b;</mzn>
    internal bool Parse(out SyntaxTree tree)
    {
        tree = new SyntaxTree(_token);
        while (true)
        {
            switch (_kind)
            {
                case TokenKind.INCLUDE:
                    if (!ParseIncludeStatement(out var inc))
                        return false;
                    tree.Nodes.Add(inc);
                    break;

                case TokenKind.CONSTRAINT:
                    if (!ParseConstraintItem(out var cons))
                        return false;
                    tree.Nodes.Add(cons);
                    break;

                case TokenKind.SOLVE:
                    if (!ParseSolveItem(out var solve))
                        return false;
                    tree.Nodes.Add(solve);
                    break;

                case TokenKind.OUTPUT:
                    if (!ParseOutputItem(out var output))
                        return false;
                    tree.Nodes.Add(output);
                    break;

                case TokenKind.ENUM:
                    if (!ParseEnumItem(out var @enum))
                        return false;
                    tree.Nodes.Add(@enum);
                    break;

                case TokenKind.TYPE:
                    if (!ParseAliasItem(out var alias))
                        return false;
                    tree.Nodes.Add(alias);
                    break;

                case TokenKind.BLOCK_COMMENT:
                case TokenKind.LINE_COMMENT:
                    Step();
                    continue;

                case TokenKind.EOF:
                    return true;

                default:
                    if (!ParseDeclarationOrAssignment(out var declare, out var assign))
                        return false;

                    if (assign is not null)
                        tree.Nodes.Add(assign);
                    else
                        tree.Nodes.Add(declare);

                    break;
            }

            if (Skip(TokenKind.EOL))
                continue;

            if (_kind is not TokenKind.EOF)
                return Expected("; or end of file");
        }
    }

    /// <summary>
    /// Parse an enumeration declaration
    /// </summary>
    /// <mzn>enum Dir = {N,S,E,W};</mzn>
    /// <mzn>enum Z = anon_enum(10);</mzn>
    /// <mzn>enum X = Q({1,2});</mzn>
    internal bool ParseEnumItem(out EnumDeclarationSyntax @enum)
    {
        @enum = null!;
        if (!Expect(TokenKind.ENUM, out var start))
            return false;

        if (!ParseIdent(out var name))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        if (_kind is TokenKind.EOL)
            return true;

        if (!Expect(TokenKind.EQUAL))
            return false;

        @enum = new EnumDeclarationSyntax(start, name);
        @enum.Annotations = anns;

        cases:
        EnumCasesSyntax @case;
        var caseStart = _token;

        // Named cases: 'enum Dir = {N,S,E,W};`
        if (Skip(TokenKind.OPEN_BRACE))
        {
            var names = new List<Token>();
            while (_kind is not TokenKind.CLOSE_BRACE)
            {
                if (!ParseIdent(out name))
                    return false;
                names.Add(name);

                if (!Skip(TokenKind.COMMA))
                    break;
            }

            if (!Expect(TokenKind.CLOSE_BRACE))
                return false;
            @case = new EnumCasesSyntax(caseStart, EnumCaseType.Names) { Names = names };
            goto end;
        }

        // Underscore enum `enum A = _(1..10);`
        if (Skip(TokenKind.UNDERSCORE))
        {
            if (!Expect(TokenKind.OPEN_PAREN))
                return false;
            if (!ParseExpr(out var expr))
                return false;
            if (!Expect(TokenKind.CLOSE_PAREN))
                return false;
            @case = new EnumCasesSyntax(caseStart, EnumCaseType.Underscore, expr);
            goto end;
        }

        // Anonymous enum: `anon_enum(10);`
        if (Skip(TokenKind.ANONENUM))
        {
            if (!Expect(TokenKind.OPEN_PAREN))
                return false;
            if (!ParseExpr(out var expr))
                return false;
            if (!Expect(TokenKind.CLOSE_PAREN))
                return false;
            @case = new EnumCasesSyntax(caseStart, EnumCaseType.Anon, expr);
            goto end;
        }

        // Complex enum: `C(1..10)`
        if (ParseIdent(out name))
        {
            if (!Expect(TokenKind.OPEN_PAREN))
                return false;
            if (!ParseExpr(out var expr))
                return false;
            if (!Expect(TokenKind.CLOSE_PAREN))
                return false;
            @case = new EnumCasesSyntax(caseStart, EnumCaseType.Complex, expr);
            goto end;
        }

        return Error("Expected an enum definition");

        end:
        @enum.Cases.Add(@case);

        if (Skip(TokenKind.PLUS_PLUS))
            goto cases;
        return true;
    }

    /// <summary>
    /// Parse an Output Item
    /// </summary>
    /// <mzn>output ["The result is \(result)"];</mzn>
    internal bool ParseOutputItem(out OutputSyntax node)
    {
        node = null!;

        if (!Expect(TokenKind.OUTPUT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        node = new OutputSyntax(start, expr);
        node.Annotations = anns;
        return true;
    }

    /// <summary>
    /// Parse a type alias
    /// </summary>
    /// <mzn>type X = 1 .. 10;</mzn>
    internal bool ParseAliasItem(out DeclarationSyntax alias)
    {
        alias = null!;

        if (!Expect(TokenKind.TYPE, out var start))
            return false;

        if (!ParseString(out var name))
            return false;

        if (!Expect(TokenKind.EQUAL))
            return false;

        if (!ParseType(out var type))
            return false;

        alias = new DeclarationSyntax(start, type);
        alias.Name = name;
        return true;
    }

    /// <summary>
    /// Parse an include item
    /// </summary>
    /// <mzn>include "utils.mzn"</mzn>
    internal bool ParseIncludeStatement(out IncludeSyntax node)
    {
        node = null!;

        if (!Expect(TokenKind.INCLUDE, out var token))
            return false;

        if (!ParseString(out var path))
            return false;

        node = new IncludeSyntax(token, path);
        return true;
    }

    /// <summary>
    /// Parse a solve item
    /// </summary>
    /// <mzn>solve satisfy;</mzn>
    /// <mzn>solve maximize a;</mzn>
    internal bool ParseSolveItem(out SolveSyntax node)
    {
        node = null!;

        if (!Expect(TokenKind.SOLVE, out var start))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        SyntaxNode? objective = null;
        SolveMethod method = SolveMethod.Satisfy;
        switch (_kind)
        {
            case TokenKind.SATISFY:
                Step();
                method = SolveMethod.Satisfy;
                break;

            case TokenKind.MINIMIZE:
                Step();
                method = SolveMethod.Minimize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            case TokenKind.MAXIMIZE:
                Step();
                method = SolveMethod.Maximize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            default:
                return Error("Expected satisfy, minimize, or maximize");
        }

        node = new SolveSyntax(start, method, objective);
        return true;
    }

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    internal bool ParseConstraintItem(out ConstraintSyntax constraint)
    {
        constraint = null!;

        if (!Expect(TokenKind.CONSTRAINT, out var start))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        constraint = new ConstraintSyntax(start, expr);
        constraint.Annotations = anns;
        return Okay;
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    internal bool ParseDeclarationOrAssignment(
        out DeclarationSyntax? variable,
        out AssignmentSyntax? assign
    )
    {
        variable = null;
        assign = null;
        var token = _token;
        var kind = _kind;
        TypeSyntax type;

        if (ParseIdent(out var name))
        {
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = new AssignmentSyntax(name, expr);
                return true;
            }

            type = new NamedType(token, name) { Kind = TypeKind.Name };
            variable = new DeclarationSyntax(token, type);
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.GENERIC or TokenKind.GENERIC_SEQ)
        {
            Step();
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = new AssignmentSyntax(token, expr);
                return true;
            }

            type = new NamedType(token, token)
            {
                Kind = _kind is TokenKind.GENERIC ? TypeKind.Generic : TypeKind.GenericSeq
            };
            variable = new DeclarationSyntax(token, type);
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.FUNCTION)
        {
            Step();
            if (!ParseType(out type))
                return false;

            variable = new DeclarationSyntax(token, type);
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.PREDICATE)
        {
            Step();
            type = new TypeSyntax(name) { Kind = TypeKind.Bool };
            variable = new DeclarationSyntax(token, type);
        }
        else if (kind is TokenKind.TEST)
        {
            Step();
            type = new TypeSyntax(name) { Kind = TypeKind.Bool };
            variable = new DeclarationSyntax(token, type);
        }
        else if (kind is TokenKind.ANNOTATION)
        {
            Step();
            type = new TypeSyntax(name) { Kind = TypeKind.Annotation };
            variable = new DeclarationSyntax(token, type);
        }
        else if (kind is TokenKind.ANY)
        {
            Step();
            type = new TypeSyntax(name) { Kind = TypeKind.Any };
            variable = new DeclarationSyntax(token, type);
        }
        else if (ParseType(out type))
        {
            variable = new DeclarationSyntax(token, type);
            Expect(TokenKind.COLON);
        }
        else
        {
            Expected("variable declaration or assignment");
        }

        if (ErrorString is not null)
            return Error();

        if (!ParseIdent(out name))
            return false;

        variable!.Name = name;

        // Function call
        if (_kind is TokenKind.OPEN_PAREN)
        {
            if (!ParseParameters(out var pars))
                return false;
            variable.Parameters = pars;
        }

        if (!ParseAnnotations(out var anns))
            return false;

        variable.Annotations = anns;

        // Declaration only
        if (!Skip(TokenKind.EQUAL))
            return true;

        // Assignment right hand side
        if (!ParseExpr(out var value))
            return false;

        variable.Body = value;
        return true;
    }

    /// <summary>
    /// Parse an expression prefix / atom
    /// </summary>
    /// <mzn>100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1]</mzn>
    /// <mzn>record.field</mzn>
    internal bool ParseExprAtom(out SyntaxNode expr)
    {
        expr = null!;
        var token = _token;

        switch (_kind)
        {
            case TokenKind.INT_LIT:
                Step();
                expr = new IntLiteralSyntax(token);
                break;

            case TokenKind.FLOAT_LIT:
                Step();
                expr = new FloatLiteralSyntax(_token);
                break;

            case TokenKind.TRUE:
                Step();
                expr = new BoolSyntax(token);
                break;

            case TokenKind.FALSE:
                Step();
                expr = new BoolSyntax(token);
                break;

            case TokenKind.STRING_LIT:
                Step();
                expr = new StringLiteralSyntax(token);
                break;

            case TokenKind.PLUS:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Positive, expr);
                break;

            case TokenKind.MINUS:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Negative, expr);
                break;

            case TokenKind.NOT:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Not, expr);
                break;

            case TokenKind.DOT_DOT:
                Step();
                if (ParseExpr(out var right))
                    expr = new RangeLiteralSyntax(token, Upper: expr);
                else if (ErrorString is not null)
                    return false;
                else
                    expr = new RangeLiteralSyntax(token);
                break;

            case TokenKind.UNDERSCORE:
                Step();
                expr = new WildCardExpr(token);
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
                expr = new EmptyExpr(token);
                break;

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
    internal bool ParseExprAtomTail(ref SyntaxNode expr)
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

                expr = new ArrayAccessSyntax(expr, access);
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
    internal bool ParseExpr(out SyntaxNode expr, ushort minPrecedence = ushort.MaxValue)
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

    internal bool ParseInfixExpr(ref SyntaxNode left) =>
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
        else if (ErrorString is not null)
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
    internal bool ParseIdentExpr(out SyntaxNode result)
    {
        result = null!;
        var name = _token;
        if (_kind is not (TokenKind.IDENT or TokenKind.QUOTED_IDENT))
            return Expected("Identifier");

        Step();

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
    internal bool ParseBracketExpr(out SyntaxNode result)
    {
        result = null!;

        if (!Expect(TokenKind.OPEN_BRACKET, out var start))
            return false;

        if (_kind is TokenKind.CLOSE_BRACKET)
        {
            result = new Array1DSyntax(start);
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
            element = new IndexAndNode(index, value);
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
        var arr1d = new Array1DSyntax(start);
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
                element = new IndexAndNode(index, value);
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
    private bool Parse2dArrayLiteral(in Token start, out Array2dSyntax arr)
    {
        arr = new Array2dSyntax(start);
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
    private bool Parse3dArrayLiteral(in Token start, out Array3dSyntax arr)
    {
        arr = new Array3dSyntax(start);

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
    private bool ParseBraceExpr(out SyntaxNode result)
    {
        result = null!;

        if (!Expect(TokenKind.OPEN_BRACE, out var start))
            return false;

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

    internal bool ParseLetExpr(out LetSyntax let)
    {
        let = null!;

        if (!Expect(TokenKind.LET, out var start))
            return false;

        if (!Expect(TokenKind.OPEN_BRACE))
            return false;

        List<ILetLocal>? locals = null;

        while (_kind is not TokenKind.CLOSE_BRACE)
        {
            if (!ParseLetLocal(out var local))
                return false;

            locals ??= new List<ILetLocal>();
            locals.Add(local);

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

        let = new LetSyntax(start, locals, body);
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

        if (!ParseDeclarationOrAssignment(out var var, out var assign))
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
    private bool ParseIfElseExpr(out SyntaxNode node)
    {
        node = null!;

        if (!Expect(TokenKind.IF, out var start))
            return false;

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
    private bool ParseParenExpr(out SyntaxNode result)
    {
        result = null!;

        if (!Expect(TokenKind.OPEN_PAREN, out var start))
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
    internal bool ParseAnnotations(out List<SyntaxNode>? annotations)
    {
        annotations = null;
        while (Skip(TokenKind.COLON_COLON))
        {
            SyntaxNode? ann;

            // Edge case where 'output' keyword can be used
            // in a non-keyword context, eg:
            // int : x :: output = 3;
            if (_kind is TokenKind.OUTPUT)
            {
                ann = new IdentifierSyntax(_token);
                Step();
            }
            else if (!ParseExprAtom(out ann))
            {
                return Expected("Annotation");
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
    internal bool ParseStringAnnotations(out List<SyntaxNode>? annotations)
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

    internal bool ParseBaseType(out TypeSyntax type)
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
    internal bool ParseArrayType(out ArrayTypeSyntax arr)
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
    internal bool ParseTypeAndName(out (Token, TypeSyntax) result)
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

    internal bool ParseType(out TypeSyntax result)
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

    private bool Expected(string msg) => Error($"Expected {msg}");

    /// Record the given message as an error and return false
    private bool Error(string? msg = null)
    {
        if (ErrorString is not null)
            return false;

        _watch.Stop();
        var trace = _text[..(int)_token.Position];
        var message = $"""

            ---------------------------------------------
            {msg}
            ---------------------------------------------
            Token {_kind}
            Line {_token.Line}
            Col {_token.Col}
            Pos {_token.Position}
            ---------------------------------------------
            {trace}
            """;

        ErrorString = message;
        return false;
    }
}
