namespace MiniZinc.Parser;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Syntax;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed class Parser
{
    private readonly Token[] _tokens;
    private string _sourceText;
    private Token _token;
    private TokenKind _kind;
    private int _i;
    private string? _errorMessage;
    private string? _errorTrace;

    internal Parser(string sourceText)
    {
        _tokens = Lexer.Lex(sourceText);
        _sourceText = sourceText;
        _i = 0;
        Step();
    }

    /// Progress the parser
    private void Step()
    {
        next:
        if (_i >= _tokens.Length)
        {
            _kind = TokenKind.EOF;
            return;
        }
        _token = _tokens[_i++];
        _kind = _token.Kind;
        if (_kind is TokenKind.LINE_COMMENT or TokenKind.BLOCK_COMMENT)
            goto next;
    }

    /// Progress the parser if the current token is of the given kind
    private bool Skip(TokenKind kind)
    {
        if (_kind != kind)
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
        if (_kind is TokenKind.INT_LITERAL)
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
    private bool ParseString(out Token result, TokenKind kind = TokenKind.STRING_LITERAL)
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

    private bool ParseIdent(out IdentifierSyntax node)
    {
        if (_kind is TokenKind.IDENTIFIER)
        {
            node = new IdentifierSyntax(_token);
            Step();
            return true;
        }
        else
        {
            node = null!;
            return Expected("Identifier");
        }
    }

    private bool ParseGeneric(out IdentifierSyntax ident)
    {
        if (_kind is TokenKind.GENERIC or TokenKind.GENERIC_SEQUENCE)
        {
            ident = new IdentifierSyntax(_token);
            Step();
            return true;
        }
        else
        {
            ident = null!;
            return Expected("Generic Variable ($$T, $T)");
        }
    }

    /// <summary>
    /// Parse a model
    /// </summary>
    /// <mzn>var 1..10: a; var 10..20: b; constraint a = b;</mzn>
    internal bool ParseModel(out ModelSyntax model)
    {
        var statements = new List<StatementSyntax>();
        while (true)
        {
            if (!ParseStatement(out var statement))
                break;

            statements.Add(statement);

            if (Skip(TokenKind.EOL))
                continue;

            if (!Expect(TokenKind.EOF))
                break;

            break;
        }

        model = new ModelSyntax(statements);
        return true;
    }

    internal bool ParseStatement([NotNullWhen(true)] out StatementSyntax? statement)
    {
        statement = null;
        switch (_kind)
        {
            case TokenKind.INCLUDE:
                if (!ParseIncludeStatement(out statement))
                    return false;
                break;

            case TokenKind.CONSTRAINT:
                if (!ParseConstraintItem(out statement))
                    return false;
                break;

            case TokenKind.SOLVE:
                if (!ParseSolveItem(out statement))
                    return false;
                break;

            case TokenKind.OUTPUT:
                if (!ParseOutputItem(out statement))
                    return false;
                break;

            case TokenKind.ENUM:
                if (!ParseEnumItem(out statement))
                    return false;
                break;

            case TokenKind.TYPE:
                if (!ParseAliasItem(out statement))
                    return false;
                break;

            case TokenKind.FUNCTION:
                if (!ParseFunctionItem(out statement))
                    return false;
                break;

            case TokenKind.PREDICATE:
                if (!ParsePredicateItem(out statement))
                    return false;
                break;

            case TokenKind.TEST:
                if (!ParseTestItem(out statement))
                    return false;
                break;

            case TokenKind.ANNOTATION:
                if (!ParseAnnotationItem(out statement))
                    return false;
                break;

            default:
                if (!ParseDeclareOrAssign(out statement))
                    return false;
                break;
        }

        return true;
    }

    /// <summary>
    /// Parse data (.dzn).
    /// For data each item is expected to be an assignment
    /// from name to value.
    /// </summary>
    /// <mzn>a = 1;b = 2; c= true;</mzn>
    internal bool ParseData(out DataSyntax data)
    {
        List<AssignmentSyntax> assignments = new();
        Dictionary<string, ExpressionSyntax> variables = new();

        while (_kind is not TokenKind.EOF)
        {
            if (!ParseIdent(out var ident))
                break;

            if (!Expect(TokenKind.EQUAL))
                break;

            if (!ParseExpr(out var expr))
                break;

            var assign = new AssignmentSyntax(ident, expr);
            assignments.Add(assign);

            if (!variables.ContainsKey(ident.Name))
            {
                Error($"Variable \"{ident}\" was assigned to multiple times");
                break;
            }

            variables.Add(ident.Name, expr);

            if (!Skip(TokenKind.EOL))
                if (!Expect(TokenKind.EOF))
                    break;
        }

        data = new DataSyntax(assignments, variables);
        return true;
    }

    /// <summary>
    /// Parse a predicate declaration
    /// </summary>
    /// <mzn>predicate ok(int: x) = x > 0;</mzn>
    private bool ParsePredicateItem([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        if (!Expect(TokenKind.PREDICATE, out var start))
            return false;

        var type = new TypeSyntax(start) { Var = true, Kind = TypeKind.Bool };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, out var dec))
            return false;

        node = dec;
        return true;
    }

    /// <summary>
    /// Parse a test declaration
    /// </summary>
    /// <mzn>predicate ok(int: x) = x > 0;</mzn>
    private bool ParseTestItem([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        if (!Expect(TokenKind.TEST, out var start))
            return false;

        var type = new TypeSyntax(start) { Kind = TypeKind.Bool };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, out var decl))
            return false;

        node = decl;
        return true;
    }

    /// <summary>
    /// Parse a function declaration
    /// </summary>
    /// <mzn>function bool: opposite(bool: x) = not x;</mzn>
    private bool ParseFunctionItem([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        if (!Expect(TokenKind.FUNCTION, out var start))
            return false;

        if (!ParseType(out var type))
            return false;

        if (!Expect(TokenKind.COLON))
            return false;

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, out var decl))
            return false;

        node = decl;
        return true;
    }

    /// <summary>
    /// Parse the tail end of a declaration.
    /// Optional arguments, optional ann, optional RHS
    /// {ArgumentList} ann {Ann} = {Body}
    /// </summary>
    private bool ParseDeclareTail(
        in Token start,
        in IdentifierSyntax identifier,
        in TypeSyntax type,
        out DeclarationSyntax node
    )
    {
        node = null!;
        bool isFunction = false;
        IdentifierSyntax? ann = null;
        List<ParameterSyntax>? parameters = null;

        if (_kind is TokenKind.OPEN_PAREN)
        {
            isFunction = true;
            if (!ParseParameters(out parameters))
                return false;

            if (Skip(TokenKind.ANN))
            {
                if (!Expect(TokenKind.COLON))
                    return false;

                if (!ParseIdent(out var a))
                    return false;
                ann = a;
            }
        }

        if (!ParseAnnotations(out var anns))
            return false;

        ExpressionSyntax? body = null;
        if (Skip(TokenKind.EQUAL))
            if (!ParseExpr(out body))
                return false;

        node = new DeclarationSyntax(start, type, identifier)
        {
            Annotations = anns,
            Ann = ann,
            Parameters = parameters,
            Body = body,
            IsFunction = isFunction
        };
        return true;
    }

    /// <summary>
    /// Parse an annotation declaration
    /// </summary>
    /// <mzn>annotation custom;</mzn>
    /// <mzn>annotation custom(int: x);</mzn>
    private bool ParseAnnotationItem([NotNullWhen(true)] out StatementSyntax? ann)
    {
        ann = null;
        if (!Expect(TokenKind.ANNOTATION, out var start))
            return false;

        var type = new TypeSyntax(start) { Kind = TypeKind.Annotation };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, out var decl))
            return false;

        ann = decl;
        return true;
    }

    /// <summary>
    /// Parse an enumeration declaration
    /// </summary>
    /// <mzn>enum Dir = {N,S,E,W};</mzn>
    /// <mzn>enum Z = anon_enum(10);</mzn>
    /// <mzn>enum X = Q({1,2});</mzn>
    internal bool ParseEnumItem([NotNullWhen(true)] out StatementSyntax? result)
    {
        result = null;
        if (!Expect(TokenKind.ENUM, out var start))
            return false;

        if (!ParseIdent(out var name))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        // Enum without assignments are valid
        if (_kind is TokenKind.EOL)
        {
            result = new EnumDeclarationSyntax(start, name);
            return true;
        }

        if (!Expect(TokenKind.EQUAL))
            return false;

        var @enum = new EnumDeclarationSyntax(start, name);
        @enum.Annotations = anns;

        cases:
        EnumCasesSyntax @case;
        var caseStart = _token;

        // Named cases: 'enum Dir = {N,S,E,W};`
        if (Skip(TokenKind.OPEN_BRACE))
        {
            var names = new List<IdentifierSyntax>();
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

            @case = new EnumCasesSyntax(caseStart, EnumCaseType.Complex, expr)
            {
                Constructor = name
            };
            goto end;
        }

        return Error("Expected an enum definition");

        end:
        @enum.Cases.Add(@case);

        if (Skip(TokenKind.PLUS_PLUS))
            goto cases;
        result = @enum;
        return true;
    }

    /// <summary>
    /// Parse an Output Item
    /// </summary>
    /// <mzn>output ["The result is \(result)"];</mzn>
    internal bool ParseOutputItem([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;

        if (!Expect(TokenKind.OUTPUT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        node = new OutputSyntax(start, expr) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse a type alias
    /// </summary>
    /// <mzn>type X = 1 .. 10;</mzn>
    internal bool ParseAliasItem([NotNullWhen(true)] out StatementSyntax? alias)
    {
        alias = null;

        if (!Expect(TokenKind.TYPE, out var start))
            return false;

        if (!ParseIdent(out var name))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        if (!Expect(TokenKind.EQUAL))
            return false;

        if (!ParseType(out var type))
            return false;

        alias = new TypeAliasSyntax(start, name, type) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse an include item
    /// </summary>
    /// <mzn>include "utils.mzn"</mzn>
    internal bool ParseIncludeStatement([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;

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
    internal bool ParseSolveItem([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;

        if (!Expect(TokenKind.SOLVE, out var start))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        ExpressionSyntax? objective = null;
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

        node = new SolveSyntax(start, method, objective) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    internal bool ParseConstraintItem([NotNullWhen(true)] out StatementSyntax? constraint)
    {
        constraint = null;

        if (!Expect(TokenKind.CONSTRAINT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        constraint = new ConstraintSyntax(start, expr) { Annotations = anns };
        return IsOk;
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    /// <mzn>$T: identity($T: x) = x;</mzn>
    internal bool ParseDeclareOrAssign([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        var start = _token;
        TypeSyntax? type = null;
        IdentifierSyntax ident;

        if (Skip(TokenKind.IDENTIFIER))
        {
            if (_kind is TokenKind.EQUAL)
            {
                ident = new IdentifierSyntax(start);
                goto tail;
            }
            else
            {
                type = new IdentifierTypeSyntax(start, new IdentifierSyntax(start))
                {
                    Kind = TypeKind.Name
                };
            }
        }
        else if (Skip(TokenKind.ANY))
            type = new TypeSyntax(start) { Kind = TypeKind.Any };
        else if (!ParseType(out type))
            return false;

        if (!Expect(TokenKind.COLON))
            return false;

        if (!ParseIdent(out ident))
            return false;

        tail:
        if (type is null)
        {
            if (!ParseAnnotations(out var anns))
                return false;

            if (!Skip(TokenKind.EQUAL))
                return Expected("=");

            if (!ParseExpr(out var value))
                return false;

            node = new AssignmentSyntax(ident, value) { Annotations = anns };
            return true;
        }
        if (!ParseDeclareTail(start, ident, type, out var dec))
            return false;

        if (dec.Body is null && dec.Type.Kind is TypeKind.Any)
            return Expected("=");

        node = dec;
        return true;
    }

    /// <summary>
    /// Parse an expression prefix / atom
    /// </summary>
    /// <mzn>100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1]</mzn>
    /// <mzn>record.field</mzn>
    internal bool ParseExprAtom([NotNullWhen(true)] out ExpressionSyntax? expr)
    {
        expr = null;
        var token = _token;

        switch (_kind)
        {
            case TokenKind.INT_LITERAL:
                Step();
                expr = new IntLiteralSyntax(token);
                break;

            case TokenKind.FLOAT_LITERAL:
                Step();
                expr = new FloatLiteralSyntax(token);
                break;

            case TokenKind.TRUE:
                Step();
                expr = new BoolLiteralSyntax(token);
                break;

            case TokenKind.FALSE:
                Step();
                expr = new BoolLiteralSyntax(token);
                break;

            case TokenKind.STRING_LITERAL:
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
                if (!ParseExprAtom(out expr))
                    return false;
                if (expr is IntLiteralSyntax i)
                    expr = new IntLiteralSyntax(token, -i.Value);
                else if (expr is FloatLiteralSyntax f)
                    expr = new FloatLiteralSyntax(token, -f.Value);
                else
                    expr = new UnaryOperatorSyntax(token, Operator.Negative, expr);
                break;

            case TokenKind.NOT:
                Step();
                if (!ParseExprAtom(out expr))
                    return false;
                expr = new UnaryOperatorSyntax(token, Operator.Not, expr);
                break;

            case TokenKind.DOT_DOT:
                Step();
                if (ParseExpr(out var right))
                    expr = new RangeLiteralSyntax(token, upper: expr);
                else if (_errorMessage is not null)
                    return false;
                else
                    expr = new RangeLiteralSyntax(token);
                break;

            case TokenKind.UNDERSCORE:
                Step();
                expr = new WildCardSyntax(token);
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

            case TokenKind.IDENTIFIER:
                if (!ParseIdentExpr(out expr))
                    return false;
                break;

            case TokenKind.EMPTY:
                Step();
                expr = new EmptyLiteralSyntax(token);
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
    internal bool ParseExprAtomTail(ref ExpressionSyntax expr)
    {
        while (true)
        {
            if (Skip(TokenKind.OPEN_BRACKET))
            {
                // Array access eg: `a[1,2]`
                var access = new List<ExpressionSyntax>();
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
            else if (_kind is TokenKind.TUPLE_ACCESS)
            {
                expr = new TupleAccessSyntax(expr, _token);
                Step();
            }
            else if (_kind is TokenKind.RECORD_ACCESS)
            {
                expr = new RecordAccessSyntax(expr, _token);
                Step();
            }
            else
            {
                break;
            }
        }
        return true;
    }

    private bool IsOk => _errorMessage is null;

    /// <summary>
    /// Parse an Expression
    /// </summary>
    /// <mzn>a + b + 100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1] * arr[2]</mzn>
    internal bool ParseExpr(
        [NotNullWhen(true)] out ExpressionSyntax? expr,
        Assoc associativity = 0,
        ushort precedence = ushort.MaxValue,
        bool typeInst = false
    )
    {
        if (!ParseExprAtom(out expr))
            return false;

        while (true)
        {
            var infix = _token;
            var (op, assoc, prec) = Precedence(infix.Kind);
            if (op is 0)
                return true;

            switch (assoc)
            {
                case Assoc.None when associativity is Assoc.None && prec == precedence:
                    return Error($"Invalid application of operator {op} due to precedence rules");

                case Assoc.Left when prec >= precedence:
                case Assoc.None when prec >= precedence:
                    return true;

                case Assoc.None when prec > precedence:
                case Assoc.Right when prec > precedence:
                    return true;
            }

            if (op is Operator.Range)
            {
                Step();
                if (ParseExpr(out var right, assoc, prec))
                    expr = new RangeLiteralSyntax(expr.Start, expr, right);
                else if (_errorMessage is not null)
                    return false;
                else
                    expr = new RangeLiteralSyntax(expr.Start, expr);
            }
            else if (op is Operator.Concat && typeInst)
            {
                return true;
            }
            else
            {
                Step();
                if (!ParseExpr(out var right, assoc, prec))
                    return false;
                expr = new BinaryOperatorSyntax(expr, infix, op, right);
            }
        }
    }

    /// <summary>
    /// https://docs.minizinc.dev/en/stable/spec.html#operators
    /// </summary>
    internal static (Operator, Assoc, ushort) Precedence(in TokenKind kind) =>
        kind switch
        {
            TokenKind.DOUBLE_ARROW => (Operator.Equivalent, Assoc.Left, 1200),
            TokenKind.RIGHT_ARROW => (Operator.Implies, Assoc.Left, 1100),
            TokenKind.LEFT_ARROW => (Operator.ImpliedBy, Assoc.Left, 1100),
            TokenKind.DOWN_WEDGE => (Operator.Or, Assoc.Left, 1000),
            TokenKind.XOR => (Operator.Xor, Assoc.Left, 1000),
            TokenKind.UP_WEDGE => (Operator.And, Assoc.Left, 900),
            TokenKind.LESS_THAN => (Operator.LessThan, Assoc.None, 800),
            TokenKind.GREATER_THAN => (Operator.GreaterThan, Assoc.None, 800),
            TokenKind.LESS_THAN_EQUAL => (Operator.LessThanEqual, Assoc.None, 800),
            TokenKind.GREATER_THAN_EQUAL => (Operator.GreaterThanEqual, Assoc.None, 800),
            TokenKind.EQUAL => (Operator.Equal, Assoc.None, 800),
            TokenKind.NOT_EQUAL => (Operator.NotEqual, Assoc.None, 800),
            TokenKind.IN => (Operator.In, Assoc.None, 700),
            TokenKind.SUBSET => (Operator.Subset, Assoc.None, 700),
            TokenKind.SUPERSET => (Operator.Superset, Assoc.None, 700),
            TokenKind.UNION => (Operator.Union, Assoc.Left, 600),
            TokenKind.DIFF => (Operator.Diff, Assoc.Left, 600),
            TokenKind.SYMDIFF => (Operator.SymDiff, Assoc.Left, 600),
            TokenKind.DOT_DOT => (Operator.Range, Assoc.None, 500),
            TokenKind.PLUS => (Operator.Add, Assoc.Left, 400),
            TokenKind.MINUS => (Operator.Subtract, Assoc.Left, 400),
            TokenKind.STAR => (Operator.Multiply, Assoc.Left, 300),
            TokenKind.DIV => (Operator.Div, Assoc.Left, 300),
            TokenKind.MOD => (Operator.Mod, Assoc.Left, 300),
            TokenKind.FWDSLASH => (Operator.Divide, Assoc.Left, 300),
            TokenKind.INTERSECT => (Operator.Intersect, Assoc.Left, 300),
            TokenKind.EXP => (Operator.Exponent, Assoc.Left, 200),
            TokenKind.PLUS_PLUS => (Operator.Concat, Assoc.Right, 100),
            TokenKind.DEFAULT => (Operator.Default, Assoc.Left, 70),
            TokenKind.INFIX_IDENTIFIER => (Operator.Identifier, Assoc.Left, 50),
            TokenKind.TILDE_PLUS => (Operator.TildeAdd, Assoc.Left, 10),
            TokenKind.TILDE_MINUS => (Operator.TildeSubtract, Assoc.Left, 10),
            TokenKind.TILDE_STAR => (Operator.TildeMultiply, Assoc.Left, 10),
            TokenKind.TILDE_EQUALS => (Operator.TildeEqual, Assoc.Left, 10),
            _ => default
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
    internal bool ParseIdentExpr([NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = null;
        var name = new IdentifierSyntax(_token);
        if (_kind is not TokenKind.IDENTIFIER)
            return Expected("Identifier");

        Step();

        // Simple identifier
        if (!Skip(TokenKind.OPEN_PAREN))
        {
            result = name;
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
        var exprs = new List<ExpressionSyntax>();
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
                Operator: Operator.In,
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

                expr = new GeneratorSyntax(expr.Start)
                {
                    From = from,
                    Where = where,
                    Names = new() { id }
                };
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
            result = new CallSyntax(name, exprs);
            return true;
        }

        // Could be a gencall if followed by (
        if (!isGen && _kind is not TokenKind.OPEN_PAREN)
        {
            result = new CallSyntax(name, exprs);
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
                    if (idents is not null)
                        g.Names.InsertRange(0, idents);
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
            if (_kind is TokenKind.UNDERSCORE)
            {
                name = new IdentifierSyntax(_token);
                Step();
            }
            else if (!ParseIdent(out name))
            {
                return Error("Expected identifier in generator names");
            }

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
    internal bool ParseBracketExpr([NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = null;

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
    bool Parse1dArrayLiteral(in Token start, [NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = default;
        ExpressionSyntax index;
        ExpressionSyntax element;

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
                if (!ParseExpr(out index!))
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

        if (Skip(TokenKind.PIPE))
            return Expect(TokenKind.CLOSE_BRACKET);

        if (!ParseExpr(out var value))
            return false;

        int j = 1;

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

            j++;

            arr.Elements.Add(value);

            if (!Skip(TokenKind.COMMA))
                break;
        }

        if (arr.J is 0)
            arr.J = j;

        if (!Expect(TokenKind.PIPE))
            return false;

        // Optional double pipe at the end
        // [|1, 2,|3, 4,||]
        if (Skip(TokenKind.PIPE))
            return Expect(TokenKind.CLOSE_BRACKET);

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
            return IsOk;
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
    private bool ParseBraceExpr([NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = null;

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

    internal bool ParseLetExpr([NotNullWhen(true)] out ExpressionSyntax? let)
    {
        let = null;

        if (!Expect(TokenKind.LET, out var start))
            return false;

        if (!Expect(TokenKind.OPEN_BRACE))
            return false;

        List<ILetLocalSyntax>? locals = null;

        while (_kind is not TokenKind.CLOSE_BRACE)
        {
            if (!ParseLetLocal(out var local))
                return false;

            locals ??= new List<ILetLocalSyntax>();
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

    private bool ParseLetLocal(out ILetLocalSyntax result)
    {
        result = null!;
        if (_kind is TokenKind.CONSTRAINT)
        {
            if (ParseConstraintItem(out var con))
            {
                result = (ConstraintSyntax)con;
                return true;
            }

            return false;
        }

        if (!ParseDeclareOrAssign(out var node))
            return false;
        result = (ILetLocalSyntax)node;
        return true;
    }

    private bool ParseIfThenCase(
        [NotNullWhen(true)] out ExpressionSyntax? ifCase,
        out ExpressionSyntax? thenCase,
        TokenKind ifKeyword
    )
    {
        ifCase = null;
        thenCase = null;

        if (!Skip(ifKeyword))
            return false;

        if (!ParseExpr(out ifCase))
            return false;

        if (!Skip(TokenKind.THEN))
            return false;

        if (!ParseExpr(out thenCase))
            return false;

        return true;
    }

    /// <summary>
    /// Parse an if-then-else expression
    /// </summary>
    /// <mzn>if x > 0 then y > 0 else true endif</mzn>
    /// <mzn>if z then 100 else 200 endif</mzn>
    private bool ParseIfElseExpr([NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = null;
        var start = _token;

        if (!ParseIfThenCase(out var ifCase, out var thenCase, TokenKind.IF))
            return false;

        var ite = new IfThenSyntax(start, ifCase, thenCase);
        result = ite;

        while (ParseIfThenCase(out ifCase, out thenCase, TokenKind.ELSEIF))
        {
            ite.ElseIfs ??= new List<(ExpressionSyntax elseif, ExpressionSyntax then)>();
            ite.ElseIfs.Add((ifCase, thenCase!));
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
    private bool ParseParenExpr([NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = null;

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
            if (expr is not IdentifierSyntax name)
                return Expected("Identifier");

            var record = new RecordLiteralSyntax(start);
            result = record;
            if (!ParseExpr(out expr))
                return false;

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
            ExpressionSyntax? ann;

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
            if (!ParseGeneric(out var ident))
                return false;

            type = new IdentifierTypeSyntax(start, ident)
            {
                Kind =
                    ident.Kind is TokenKind.GENERIC_SEQUENCE
                        ? TypeKind.GenericSeq
                        : TypeKind.Generic
            };
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
                type = new TypeSyntax(start) { Kind = TypeKind.Ann };
                break;

            case TokenKind.RECORD:
                if (!ParseRecordType(out var record))
                    return false;
                type = record;
                break;

            case TokenKind.TUPLE:
                if (!ParseTupleType(out var tuple))
                    return false;
                type = tuple;
                break;

            case TokenKind.GENERIC:
                Step();
                type = new IdentifierTypeSyntax(start, new IdentifierSyntax(start))
                {
                    Kind = TypeKind.Generic
                };
                break;

            case TokenKind.GENERIC_SEQUENCE:
                Step();
                type = new IdentifierTypeSyntax(start, new IdentifierSyntax(start))
                {
                    Kind = TypeKind.GenericSeq
                };
                break;

            default:
                if (!ParseExpr(out var expr, typeInst: true))
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
    internal bool ParseTypeAndName(out ParameterSyntax param)
    {
        param = null!;
        if (!ParseType(out var type))
            return false;

        // Parameters can have a type only
        // `int: get_two(int) = 2`
        if (!Skip(TokenKind.COLON))
        {
            param = new ParameterSyntax(type, null);
            return true;
        }

        if (!ParseIdent(out var name))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        param = new ParameterSyntax(type, name);
        param.Annotations = anns;
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

        record.Fields = fields!;
        return true;
    }

    /// <summary>
    /// Parse an comma separated list of types
    /// and names between parentheses
    /// </summary>
    /// <mzn>(int: a, bool: b)</mzn>
    private bool ParseParameters(out List<ParameterSyntax>? parameters)
    {
        parameters = default;
        if (!Expect(TokenKind.OPEN_PAREN))
            return false;

        if (_kind is TokenKind.CLOSE_PAREN)
            goto end;

        next:
        if (!ParseTypeAndName(out var param))
            return false;

        parameters ??= new List<ParameterSyntax>();
        parameters.Add(param);
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
        var complex = new CompositeTypeSyntax(start) { Kind = TypeKind.Composite };
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
        if (_errorMessage is not null)
            return false;

        _errorTrace = _sourceText[.._token.End];
        _errorTrace = $"""
            ---------------------------------------------
            {msg}
            ---------------------------------------------
            Token {_kind}
            Line {_token.Line}
            Col {_token.Col}
            Pos {_token.Start}
            ---------------------------------------------
            {_errorTrace}
            """;

        _errorMessage = msg;
        return false;
    }

    /// <summary>
    /// Parse the given minizinc model or data file
    /// </summary>
    /// <example>Parser.ParseFile("model.mzn")</example>
    /// <example>Parser.ParseFile("data.dzn")</example>
    public static ModelParseResult ParseModelFile(string path)
    {
        var watch = Stopwatch.StartNew();
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        var ok = parser.ParseModel(out var model);
        var elapsed = watch.Elapsed;
        var result = new ModelParseResult
        {
            Model = model,
            SourceFile = path,
            SourceText = mzn,
            Ok = ok,
            FinalToken = parser._token,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <summary>
    /// Parse the given minizinc string
    /// </summary>
    /// <example>
    /// Parser.ParseText("""
    ///     var bool: a;
    ///     var bool: b;
    ///     constraint a /\ b;
    ///     """);
    /// </example>
    public static ModelParseResult ParseModelString(string text)
    {
        var watch = Stopwatch.StartNew();
        var parser = new Parser(text);
        var ok = parser.ParseModel(out var model);
        var elapsed = watch.Elapsed;
        var result = new ModelParseResult
        {
            Model = model,
            SourceFile = null,
            SourceText = text,
            Ok = ok,
            FinalToken = parser._token,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <inheritdoc cref="ParseModelFile(string)"/>
    public static ModelParseResult ParseModelFile(FileInfo file) => ParseModelFile(file.FullName);

    /// <summary>
    /// Parse the given minizinc data file.
    /// Data files only allow assignments eg: `a = 10;`
    /// </summary>
    /// <example>Parser.ParseDataFile("data.dzn")</example>
    public static DataParseResult ParseDataFile(string path)
    {
        var watch = Stopwatch.StartNew();
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        var ok = parser.ParseData(out var data);
        var elapsed = watch.Elapsed;
        var result = new DataParseResult
        {
            Data = data,
            SourceFile = path,
            SourceText = mzn,
            Ok = ok,
            FinalToken = parser._token,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <summary>
    /// Parse the given minizinc data string.
    /// Data strings only allow assignments eg: `a = 10;`
    /// </summary>
    /// <example>
    /// Parser.ParseDataString("a = 10; b=true;");
    /// </example>
    public static DataParseResult ParseDataString(string text)
    {
        var watch = Stopwatch.StartNew();
        var parser = new Parser(text);
        var ok = parser.ParseData(out var data);
        var elapsed = watch.Elapsed;
        var result = new DataParseResult
        {
            Data = data,
            SourceFile = null,
            SourceText = text,
            Ok = ok,
            FinalToken = parser._token,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <inheritdoc cref="ParseDataFile(string)"/>
    public static DataParseResult ParseDataFile(FileInfo file) => ParseDataFile(file.FullName);

    /// Parse an expression of the given type from text
    public static T ParseExpression<T>(string text)
        where T : ExpressionSyntax
    {
        var parser = new Parser(text);
        if (!parser.ParseExpr(out var expr))
            throw new MiniZincParseException(
                parser._errorMessage ?? "",
                parser._token,
                parser._errorTrace
            );

        if (expr is not T result)
        {
            throw new MiniZincParseException(
                $"The parsed expression is of type {expr.GetType()} but expected {typeof(T)}",
                parser._token
            );
        }
        return result;
    }

    /// Parse a statement of the given type from text
    public static T ParseStatement<T>(string text)
        where T : StatementSyntax
    {
        var parser = new Parser(text);
        if (!parser.ParseStatement(out var statement))
            throw new MiniZincParseException(
                parser._errorMessage ?? "",
                parser._token,
                parser._errorTrace
            );

        if (statement is not T result)
        {
            throw new MiniZincParseException(
                $"The parsed statement is of type {statement.GetType()} but expected a {typeof(T)}",
                parser._token
            );
        }
        return result;
    }
}
