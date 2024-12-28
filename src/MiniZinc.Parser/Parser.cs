namespace MiniZinc.Parser;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Syntax;
using static TokenKind;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed class Parser
{
    /// <summary>
    /// Operator Associativity
    /// </summary>
    public enum Assoc : byte
    {
        None = 1,
        Left,
        Right
    }

    private readonly Token[] _tokens;
    private readonly int _n;
    private string _sourceText;
    private Token _current;
    private TokenKind _kind;
    private int _i;
    private string? _errorMessage;
    private string? _errorTrace;

    internal Parser(string sourceText)
    {
        _tokens = Lexer.Lex(sourceText);
        _sourceText = sourceText;
        _n = _tokens.Length;
        _i = 0;
        Step();
    }

    /// Progress the parser
    private void Step()
    {
        next:
        if (_i >= _n)
        {
            _kind = TOKEN_EOF;
            return;
        }

        _current = _tokens[_i++];
        _kind = _current.Kind;
        if (_kind is TOKEN_LINE_COMMENT or TOKEN_BLOCK_COMMENT)
            goto next;
    }

    private Token Peek()
    {
        for (int j = _i; j < _n; j++)
        {
            var token = _tokens[j];
            if (token.Kind is TOKEN_LINE_COMMENT or TOKEN_BLOCK_COMMENT)
                continue;
            return token;
        }

        return default;
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
            return Expected($"a {kind} but encountered a {_current.Kind}");

        Step();
        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind, out Token token)
    {
        if (_kind != kind)
        {
            token = default;
            return Expected($"a {kind} but encountered a {_current.Kind}");
        }

        token = _current;
        Step();
        return true;
    }

    private bool ParseInt(out Token token)
    {
        if (_kind is TOKEN_INT_LITERAL)
        {
            token = _current;
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
    /// Read the current tokens string into the given variable
    /// </summary>
    private bool ParseString(out Token result, TokenKind kind = TOKEN_STRING_LITERAL)
    {
        if (_kind == kind)
        {
            result = _current;
            Step();
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    private bool ParseIdent(out Token node)
    {
        if (_kind is TOKEN_IDENTIFIER)
        {
            node = _current;
            Step();
            return true;
        }
        else
        {
            node = default;
            return Expected("Identifier");
        }
    }

    /// <summary>
    /// Parse a model
    /// </summary>
    /// <mzn>var 1..10: a; var 10..20: b; constraint a = b;</mzn>
    internal bool ParseModel(out ModelSyntax model)
    {
        var statements = new List<StatementSyntax>();
        model = new ModelSyntax(statements);

        while (ParseStatement(out var statement))
        {
            statements.Add(statement);

            if (Skip(TOKEN_EOL) && !Skip(TOKEN_EOF))
                continue;
            else
                Expect(TOKEN_EOF);
            break;
        }

        if (_errorMessage is null)
            return true;
        else
            return false;
    }

    internal bool ParseStatement([NotNullWhen(true)] out StatementSyntax? statement)
    {
        statement = null;
        switch (_kind)
        {
            case KEYWORD_INCLUDE:
                if (!ParseIncludeStatement(out statement))
                    return false;
                break;

            case KEYWORD_CONSTRAINT:
                if (!ParseConstraintStatement(out statement))
                    return false;
                break;

            case KEYWORD_SOLVE:
                if (!ParseSolveStatement(out statement))
                    return false;
                break;

            case KEYWORD_OUTPUT:
                if (!ParseOutputStatement(out statement))
                    return false;
                break;

            case KEYWORD_ENUM:
                if (!ParseEnumStatement(out statement))
                    return false;
                break;

            case KEYWORD_TYPE:
                if (!ParseTypeAliasStatement(out statement))
                    return false;
                break;

            case KEYWORD_FUNCTION:
                if (!ParseFunctionStatement(out statement))
                    return false;
                break;

            case KEYWORD_PREDICATE:
                if (!ParsePredicateStatement(out statement))
                    return false;
                break;

            case KEYWORD_TEST:
                if (!ParseTestStatement(out statement))
                    return false;
                break;

            case KEYWORD_ANNOTATION:
                if (!ParseAnnotationStatement(out statement))
                    return false;
                break;

            default:
                if (!ParseDeclareOrAssignStatement(out statement))
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
    internal bool ParseData(out MiniZincData data)
    {
        Dictionary<string, Datum> values = new();
        data = new MiniZincData(values);

        while (true)
        {
            if (_kind is TOKEN_EOF)
                return true;

            if (!ParseIdent(out var name))
                return false;

            if (!Expect(TOKEN_EQUAL))
                return false;

            if (!ParseDatum(out var value))
                return false;

            if (values.ContainsKey(name.StringValue))
                return Error($"Variable \"{name}\" was assigned to multiple times");

            values.Add(name.StringValue, value);

            if (Skip(TOKEN_EOL))
                continue;

            if (!Expect(TOKEN_EOF))
                return false;

            break;
        }

        return true;
    }

    /// <summary>
    /// Parse a predicate declaration
    /// </summary>
    /// <mzn>predicate ok(int: x) = x > 0;</mzn>
    private bool ParsePredicateStatement([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        if (!Expect(KEYWORD_PREDICATE, out var start))
            return false;

        var type = new TypeSyntax(start) { Var = true, Kind = TypeKind.Bool };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.Predicate, out var dec))
            return false;

        node = dec;
        return true;
    }

    /// <summary>
    /// Parse a test declaration
    /// </summary>
    /// <mzn>predicate ok(int: x) = x > 0;</mzn>
    private bool ParseTestStatement([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        if (!Expect(KEYWORD_TEST, out var start))
            return false;

        var type = new TypeSyntax(start) { Kind = TypeKind.Bool };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.Test, out var decl))
            return false;

        node = decl;
        return true;
    }

    /// <summary>
    /// Parse a function declaration
    /// </summary>
    /// <mzn>function bool: opposite(bool: x) = not x;</mzn>
    private bool ParseFunctionStatement([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;
        if (!Expect(KEYWORD_FUNCTION, out var start))
            return false;

        if (!ParseType(out var type))
            return false;

        if (!Expect(TOKEN_COLON))
            return false;

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.Function, out var decl))
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
        in Token identifier,
        in TypeSyntax type,
        in DeclareKind kind,
        out DeclareStatement node
    )
    {
        node = null!;
        Token? ann = null;
        List<ParameterSyntax>? parameters = null;

        if (_kind is TOKEN_OPEN_PAREN)
        {
            if (!ParseParameters(out parameters))
                return false;

            if (Skip(KEYWORD_ANN))
            {
                if (!Expect(TOKEN_COLON))
                    return false;

                if (!ParseIdent(out var a))
                    return false;
                ann = a;
            }
        }

        if (!ParseAnnotations(out var anns))
            return false;

        ExpressionSyntax? body = null;
        if (Skip(TOKEN_EQUAL))
            if (!ParseExpr(out body))
                return false;

        node = new DeclareStatement(start, kind, type, identifier)
        {
            Annotations = anns,
            Ann = ann,
            Parameters = parameters,
            Body = body
        };
        return true;
    }

    /// <summary>
    /// Parse an annotation declaration
    /// </summary>
    /// <mzn>annotation custom;</mzn>
    /// <mzn>annotation custom(int: x);</mzn>
    private bool ParseAnnotationStatement([NotNullWhen(true)] out StatementSyntax? ann)
    {
        ann = null;
        if (!Expect(KEYWORD_ANNOTATION, out var start))
            return false;

        var type = new TypeSyntax(start) { Kind = TypeKind.Annotation };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.Annotation, out var decl))
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
    internal bool ParseEnumStatement([NotNullWhen(true)] out StatementSyntax? result)
    {
        result = null;
        if (!Expect(KEYWORD_ENUM, out var start))
            return false;

        if (!ParseIdent(out var name))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        // Enum without assignments are valid
        if (!Skip(TOKEN_EQUAL))
        {
            result = new DeclareStatement(start, DeclareKind.Enum, null, name)
            {
                Annotations = anns
            };
            return true;
        }

        if (!ParseEnumCases(out var cases))
            return false;

        result = new DeclareStatement(start, DeclareKind.Enum, null, name)
        {
            Annotations = anns,
            Body = cases
        };
        return true;
    }

    private bool ValidateEnumCases(ExpressionSyntax expr)
    {
        switch (expr)
        {
            // Named cases: 'enum Dir = {N,S,E,W};`
            case SetLiteralSyntax set:
                foreach (var item in set.Elements)
                {
                    if (item is not IdentifierSyntax name)
                        return Expected($"Enum case name, got {item}");
                }

                break;

            // Underscore enum `enum A = _(1..10);`
            case CallSyntax { Name.Kind: TOKEN_UNDERSCORE } call:
                if (call.Args is not { Count: 1 })
                    return Expected($"Single argument call to _, got {call.Args}");
                var arg = call.Args[0];
                break;

            // Anonymous enum: `anon_enum(10);`
            case CallSyntax { Name.StringValue: "anon_enum" } call:
                if (call.Args is not { Count: 1 })
                    return Expected($"Single argument call to _, got {call.Args}");
                var anonArg = call.Args[0];
                break;

            // Complex enum: `C(1..10)`
            case CallSyntax call:
                break;

            // ++
            case BinaryOperatorSyntax
            {
                Left: var left,
                Operator: TOKEN_PLUS_PLUS,
                Right: var right
            }:
                if (!ValidateEnumCases(left))
                    return false;
                if (!ValidateEnumCases(right))
                    return false;
                break;

            default:
                return Expected("Enum case");
        }

        return true;
    }

    internal bool ParseEnumCases([NotNullWhen(true)] out ExpressionSyntax? expr)
    {
        if (!ParseExpr(out expr))
            return false;

        if (!ValidateEnumCases(expr))
            return false;

        return true;
    }

    /// <summary>
    /// Parse an Output Item
    /// </summary>
    /// <mzn>output ["The result is \(result)"];</mzn>
    internal bool ParseOutputStatement([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;

        if (!Expect(KEYWORD_OUTPUT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        node = new OutputStatement(start, expr) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse a type alias
    /// </summary>
    /// <mzn>type X = 1 .. 10;</mzn>
    internal bool ParseTypeAliasStatement([NotNullWhen(true)] out StatementSyntax? alias)
    {
        alias = null;

        if (!Expect(KEYWORD_TYPE, out var start))
            return false;

        if (!ParseIdent(out var name))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        if (!Expect(TOKEN_EQUAL))
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

        if (!Expect(KEYWORD_INCLUDE, out var token))
            return false;

        if (!ParseString(out var path))
            return false;

        node = new IncludeStatement(token, path);
        return true;
    }

    /// <summary>
    /// Parse a solve item
    /// </summary>
    /// <mzn>solve satisfy;</mzn>
    /// <mzn>solve maximize a;</mzn>
    internal bool ParseSolveStatement([NotNullWhen(true)] out StatementSyntax? node)
    {
        node = null;

        if (!Expect(KEYWORD_SOLVE, out var start))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        ExpressionSyntax? objective = null;
        SolveMethod method = SolveMethod.Satisfy;
        switch (_kind)
        {
            case KEYWORD_SATISFY:
                Step();
                method = SolveMethod.Satisfy;
                break;

            case KEYWORD_MINIMIZE:
                Step();
                method = SolveMethod.Minimize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            case KEYWORD_MAXIMIZE:
                Step();
                method = SolveMethod.Maximize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            default:
                return Error("Expected satisfy, minimize, or maximize");
        }

        node = new SolveStatement(start, method, objective) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    internal bool ParseConstraintStatement([NotNullWhen(true)] out StatementSyntax? constraint)
    {
        constraint = null;

        if (!Expect(KEYWORD_CONSTRAINT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        constraint = new ConstraintStatement(start, expr) { Annotations = anns };
        return IsOk;
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    /// <mzn>$T: identity($T: x) = x;</mzn>
    internal bool ParseDeclareOrAssignStatement([NotNullWhen(true)] out StatementSyntax? statement)
    {
        statement = null;
        var start = _current;

        if (_kind is TOKEN_IDENTIFIER && Peek().Kind is TOKEN_EQUAL)
        {
            ParseIdent(out var name);
            Step();
            if (!ParseExpr(out var expr))
                return false;

            statement = new AssignStatement(name, expr);
            return true;
        }

        TypeSyntax type;
        if (Skip(KEYWORD_ANY))
        {
            type = new TypeSyntax(start) { Kind = TypeKind.Any };
        }
        else if (!ParseType(out type!))
        {
            return false;
        }

        if (!Expect(TOKEN_COLON))
            return false;

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.Value, out var dec))
            return false;

        if (dec.Body is null && type.Kind is TypeKind.Any)
            return Expected("=");

        statement = dec;
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
        var token = _current;

        switch (_kind)
        {
            case TOKEN_INT_LITERAL:
                Step();
                expr = new IntLiteralSyntax(token);
                break;

            case TOKEN_FLOAT_LITERAL:
                Step();
                expr = new FloatLiteralSyntax(token);
                break;

            case KEYWORD_TRUE:
                Step();
                expr = new BoolLiteralSyntax(token, true);
                break;

            case KEYWORD_FALSE:
                Step();
                expr = new BoolLiteralSyntax(token, false);
                break;

            case TOKEN_STRING_LITERAL:
                Step();
                expr = new StringLiteralSyntax(token);
                break;

            case TOKEN_OPEN_PAREN:
                if (!ParseParenExpr(out expr))
                    return false;
                break;

            case TOKEN_OPEN_BRACE:
                if (!ParseBraceExpr(out expr))
                    return false;
                break;

            case TOKEN_OPEN_BRACKET:
                if (!ParseBracketExpr(out expr))
                    return false;
                break;

            case KEYWORD_IF:
                if (!ParseIfElseExpr(out var ite))
                    return false;
                expr = ite;
                break;

            case KEYWORD_LET:
                if (!ParseLetExpr(out var let))
                    return false;
                expr = let;
                break;

            case TOKEN_UNDERSCORE:
            case TOKEN_IDENTIFIER:
                if (!ParseIdentifierExpr(out expr))
                    return false;
                break;

            case TOKEN_EMPTY:
                Step();
                expr = new EmptyLiteralSyntax(token);
                break;

            default:
                return false;
        }

        while (true)
        {
            if (Skip(TOKEN_OPEN_BRACKET))
            {
                // Array access eg: `a[1,2]`
                var access = new List<ExpressionSyntax>();
                while (_kind is not TOKEN_CLOSE_BRACKET)
                {
                    if (!ParseExpr(out var index))
                        return false;
                    access.Add(index);
                    if (!Skip(TOKEN_COMMA))
                        break;
                }

                expr = new ArrayAccessSyntax(expr, access);
                if (!Expect(TOKEN_CLOSE_BRACKET))
                    return false;
            }
            else if (_kind is TOKEN_TUPLE_ACCESS)
            {
                expr = new TupleAccessSyntax(expr, _current);
                Step();
            }
            else if (_kind is TOKEN_RECORD_ACCESS)
            {
                expr = new RecordAccessSyntax(expr, _current);
                Step();
            }
            else
            {
                break;
            }
        }

        if (!ParseAnnotations(out var anns))
            return false;

        expr.Annotations = anns;
        return true;
    }

    private bool IsOk => _errorMessage is null;

    /// Parse an int range data starting at the given token
    /// The parser will step over the given lower bound
    /// Will only return false in the case of an error
    internal bool ParseIntDatum(out int i, out IntRange? range)
    {
        range = null;
        Token start = _current;
        i = start.IntValue;
        Step();
        if (!Skip(TOKEN_CLOSED_RANGE))
            return true;
        if (!Expect(TOKEN_INT_LITERAL, out Token upper))
            return false;
        range = new IntRange(i, upper.IntValue);
        return true;
    }

    /// Parse a float range starting at the given token
    /// The parser will step over the given lower bound
    /// Will only return false in the case of an error
    internal bool ParseFloatDatum(out decimal f, out FloatRange? range)
    {
        range = null;
        Token start = _current;
        f = start.FloatValue;
        Step();
        if (!Skip(TOKEN_CLOSED_RANGE))
            return true;
        if (!Expect(TOKEN_FLOAT_LITERAL, out Token upper))
            return false;
        range = new FloatRange(f, upper.FloatValue);
        return true;
    }

    /// <summary>
    /// Parse an array datum
    /// </summary>
    /// <mzn>[1,2,3]</mzn>
    /// <mzn>[<>,true,false,<>]</mzn>
    /// <mzn>[(1,"s"), (2, "a")]</mzn>
    /// <remarks>
    /// We attempt to refine the return type as much as possible
    /// which complicates the code a little but is well worth it
    /// as it makes common cases such as int and float arrays much
    /// easier to consume and faster too as they do not need to be
    /// unpacked from their Datum equivalents.
    ///
    /// Perhaps a case could be made for first class support for
    /// int?[] and float?[] but for now those will fallback
    /// to Datum[]
    /// </remarks>
    internal bool ParseArrayDatum([NotNullWhen(true)] out Datum? array)
    {
        array = null;
        if (!Skip(TOKEN_OPEN_BRACKET))
            return false;

        List<int>? ints = null;
        List<bool>? bools = null;
        List<decimal>? floats = null;
        List<string>? strings = null;
        List<Datum>? values = null;
        DatumKind type = default;
        bool opt = false;
        while (_kind is not TOKEN_CLOSE_BRACKET)
        {
            switch (_kind, type, opt)
            {
                // First value is <>
                case (TOKEN_EMPTY, DatumKind.Unknown, false):
                    values = [Datum.Empty];
                    opt = true;
                    break;

                // <> found in IntArray
                case (TOKEN_EMPTY, DatumKind.Int, false):
                    values = [];
                    foreach (var ix in ints!)
                        values.Add(Datum.Int(ix));
                    values.Add(Datum.Empty);
                    opt = true;
                    break;

                // <> found in FloatArray
                case (TOKEN_EMPTY, DatumKind.Float, false):
                    values = [];
                    foreach (var fx in floats!)
                        values.Add(Datum.Float(fx));
                    values.Add(Datum.Empty);
                    opt = true;
                    break;

                // <> found in StringArray
                case (TOKEN_EMPTY, DatumKind.String, false):
                    values = [];
                    foreach (var s in strings!)
                        values.Add(Datum.String(s));
                    values.Add(Datum.Empty);
                    opt = true;
                    break;

                // <> found in BoolArray
                case (TOKEN_EMPTY, DatumKind.Bool, false):
                    values = [];
                    foreach (var b in bools!)
                        values.Add(Datum.Bool(b));
                    values.Add(Datum.Empty);
                    opt = true;
                    break;

                // <> occured
                case (TOKEN_EMPTY, _, _):
                    values!.Add(Datum.Empty);
                    opt = true;
                    break;

                // Int or IntRange
                case (TOKEN_INT_LITERAL, _, _):
                    if (!ParseIntDatum(out int i, out var intRange))
                    {
                        return false;
                    }
                    else if (intRange is not null)
                    {
                        (values ??= []).Add(intRange);
                        if (type is not (DatumKind.Set or DatumKind.Unknown))
                            return Expected($"datum of type {type}, got IntRange");
                        type = DatumKind.Set;
                    }
                    else if (opt)
                    {
                        if (type is not (DatumKind.Int or DatumKind.Unknown))
                            return Expected($"datum of type {type}, got int");

                        values!.Add(Datum.Int(i));
                        type = DatumKind.Int;
                    }
                    else
                    {
                        if (type is not (DatumKind.Int or DatumKind.Unknown))
                            return Expected($"datum of type {type}, got int");

                        (ints ??= []).Add(i);
                        type = DatumKind.Int;
                    }
                    break;

                // Float or FloatRange
                case (TOKEN_FLOAT_LITERAL, _, _):
                    if (!ParseFloatDatum(out decimal f, out var floatRange))
                    {
                        return false;
                    }
                    else if (floatRange is not null)
                    {
                        (values ??= []).Add(floatRange);
                        if (type is not (DatumKind.Set or DatumKind.Unknown))
                            return Expected($"datum of type {type}, got FloatRange");
                        type = DatumKind.Set;
                    }
                    else if (opt)
                    {
                        if (type is not (DatumKind.Float or DatumKind.Unknown))
                            return Expected($"datum of type {type}, got Float");

                        values!.Add(Datum.Float(f));
                        type = DatumKind.Float;
                    }
                    else
                    {
                        if (type is not (DatumKind.Float or DatumKind.Unknown))
                            return Expected($"datum of type {type}, got Float");

                        (floats ??= []).Add(f);
                        type = DatumKind.Float;
                    }
                    break;

                case (KEYWORD_TRUE, DatumKind.Unknown, false):
                    bools = [true];
                    type = DatumKind.Bool;
                    Step();
                    break;

                case (KEYWORD_TRUE, DatumKind.Bool, false):
                    bools!.Add(true);
                    Step();
                    break;

                case (KEYWORD_FALSE, DatumKind.Unknown, false):
                    bools = [false];
                    type = DatumKind.Bool;
                    Step();
                    break;

                case (KEYWORD_FALSE, DatumKind.Bool, false):
                    bools!.Add(false);
                    Step();
                    break;

                case (TOKEN_STRING_LITERAL, DatumKind.Unknown or DatumKind.String, false):
                    (strings ??= []).Add(_current.StringValue);
                    type = DatumKind.String;
                    Step();
                    break;

                case (TOKEN_STRING_LITERAL, DatumKind.Unknown or DatumKind.String, true):
                    values!.Add(Datum.String(_current.StringValue));
                    type = DatumKind.String;
                    Step();
                    break;

                default:
                    if (!ParseDatum(out var datum))
                        return false;
                    if (type is not DatumKind.Unknown)
                        if (type != datum.Kind)
                            return Expected($"Datum of type {type}, got {datum.Kind}");
                    type = datum.Kind;
                    (values ??= []).Add(datum);
                    break;
            }

            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (!Expect(TOKEN_CLOSE_BRACKET))
            return false;

        if (ints is not null)
            array = new IntArray(ints);
        else if (floats is not null)
            array = new FloatArray(floats);
        else if (bools is not null)
            array = new BoolArray(bools);
        else if (strings is not null)
            array = new StringArray(strings);
        else
            array = new DatumArray(values ?? []);
        return true;
    }

    /// Parse set data into the most refined type possible
    internal bool ParseSetDatum([NotNullWhen(true)] out Datum? set)
    {
        set = null;
        if (!Skip(TOKEN_OPEN_BRACE))
            return false;

        List<int>? ints = null;
        List<bool>? bools = null;
        List<decimal>? floats = null;
        List<Datum>? data = null;

        while (_kind is not TOKEN_CLOSE_BRACE)
        {
            var token = _current;
            Step();
            switch (token.Kind)
            {
                case TOKEN_INT_LITERAL:
                    (ints ??= []).Add(token.IntValue);
                    break;

                case TOKEN_FLOAT_LITERAL:
                    (floats ??= []).Add(token.FloatValue);
                    break;

                case KEYWORD_TRUE:
                    (bools ??= []).Add(true);
                    break;

                case KEYWORD_FALSE:
                    (bools ??= []).Add(false);
                    break;

                default:
                    return Error($"Unexpected {token} in set data");
            }

            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (!Expect(TOKEN_CLOSE_BRACE))
            return false;

        if (ints is not null)
            set = new IntSet(ints);
        else if (floats is not null)
            set = new FloatSet(floats);
        else if (bools is not null)
            set = new BoolSet(bools);
        else
            set = new SetDatum(data ?? []);
        return true;
    }

    /// <summary>
    /// Parse a <see cref="Datum"/>.
    /// </summary>
    /// <mzn>1</mzn>
    /// <mzn>true</mzn>
    /// <mzn>{1,2,3}</mzn>
    /// <mzn>1..10</mzn>
    public bool ParseDatum([NotNullWhen(true)] out Datum? datum)
    {
        datum = null;
        switch (_kind)
        {
            case TOKEN_OPEN_BRACE:
                if (!ParseSetDatum(out datum))
                    return false;
                break;

            case TOKEN_OPEN_BRACKET:
                if (!ParseArrayDatum(out datum))
                    return false;
                break;

            case TOKEN_OPEN_PAREN when Peek().Kind is TOKEN_IDENTIFIER:
                if (!ParseRecordDatum(out datum))
                    return false;
                break;

            case TOKEN_OPEN_PAREN:
                if (!ParseTupleDatum(out datum))
                    return false;
                break;

            case TOKEN_INT_LITERAL:
                if (!ParseIntDatum(out int i, out var intRange))
                    return false;
                else if (intRange is not null)
                    datum = intRange;
                else
                    datum = new IntDatum(i);
                break;

            case TOKEN_FLOAT_LITERAL:
                if (!ParseFloatDatum(out decimal f, out var floatRange))
                    return false;
                else if (floatRange is not null)
                    datum = floatRange;
                else
                    datum = new FloatDatum(f);
                break;

            case KEYWORD_TRUE:
                Step();
                datum = Datum.True;
                break;

            case KEYWORD_FALSE:
                Step();
                datum = Datum.False;
                break;

            case TOKEN_STRING_LITERAL:
                var s = _current.StringValue;
                Step();
                datum = new StringDatum(s);
                break;

            case TOKEN_EMPTY:
                Step();
                datum = Datum.Empty;
                break;

            default:
                return Error($"Unexpected token {_current}");
        }

        return true;
    }

    internal bool ParseTupleDatum([NotNullWhen(true)] out Datum? value)
    {
        Step();
        value = null;
        List<Datum> values = [];
        while (_kind is not TOKEN_CLOSE_PAREN)
        {
            if (!ParseDatum(out value))
                return false;
            values.Add(value);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (!Expect(TOKEN_CLOSE_PAREN))
            return false;

        value = new DatumTuple(values);
        return true;
    }

    internal bool ParseRecordDatum([NotNullWhen(true)] out Datum? value)
    {
        value = null;

        if (!Expect(TOKEN_OPEN_PAREN))
            return false;

        Dictionary<string, Datum> fields = [];
        value = new RecordDatum(fields);

        while (_kind is not TOKEN_CLOSE_PAREN)
        {
            if (!ParseIdent(out var field))
                return false;

            var fieldName = field.ToString();

            if (!Expect(TOKEN_COLON))
                return false;

            if (!ParseDatum(out var fieldValue))
                return false;

            if (fields.ContainsKey(fieldName))
                return Expected($"a unique value name, got {fieldName}");

            fields.Add(fieldName, fieldValue);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (!Expect(TOKEN_CLOSE_PAREN))
            return false;

        return true;
    }

    /// <summary>
    /// Parse an Expression
    /// </summary>
    /// <mzn>a + b + 100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1] * arr[2]</mzn>
    internal bool ParseExpr(
        [NotNullWhen(true)] out ExpressionSyntax? expr,
        Assoc associativity = 0,
        short precedence = 0,
        bool typeInst = false
    )
    {
        expr = null;
        Token token = _current;
        TokenKind op = token.Kind;
        short prec = Precedence(op);
        Assoc assoc = Associativity(op);

        switch (op)
        {
            // Unary operators bind tighter than all binary operators
            case TOKEN_PLUS:
            case TOKEN_MINUS:
            case KEYWORD_NOT:
                Step();
                if (!ParseExpr(out expr, Assoc.Right, MAX_PRECEDENCE))
                    return false;
                expr = UnOpExpr(token, expr);
                break;

            // Open range operators are a special case as their binary precedence
            // rules still apply which means  ..1+1
            case TOKEN_CLOSED_RANGE:
            case TOKEN_RIGHT_OPEN_RANGE:
                Step();
                if (!ParseExpr(out expr, assoc, prec))
                    return false;
                expr = UnOpExpr(token, expr);
                break;

            default:
                if (!ParseExprAtom(out expr))
                    return false;
                break;
        }

        while (true)
        {
            token = _current;
            op = token.Kind;
            prec = Precedence(op);
            assoc = Associativity(op);

            if (typeInst && op is TOKEN_PLUS_PLUS)
                return true;

            if (prec < precedence)
                return true;

            if (prec == precedence)
                if (associativity is Assoc.None && assoc is Assoc.None)
                    return Error($"Invalid application of operator {op} due to precedence rules");
                else if (associativity is Assoc.Left)
                    return true;

            Step();

            if (
                op
                is TOKEN_CLOSED_RANGE
                    or TOKEN_OPEN_RANGE
                    or TOKEN_RIGHT_OPEN_RANGE
                    or TOKEN_LEFT_OPEN_RANGE
            )
            {
                int i = _i;
                if (!ParseExpr(out var right))
                {
                    if (_i > i) // Failed while consuming input
                        return false;

                    expr = new RangeLiteralSyntax(expr.Start, op, lower: expr);
                }
                else
                {
                    expr = new RangeLiteralSyntax(expr.Start, op, lower: expr, upper: right);
                }
            }
            else
            {
                if (!ParseExpr(out var right, assoc, prec))
                    return false;
                expr = BinOpExpr(expr, token, right);
            }
        }
    }

    internal static ExpressionSyntax UnOpExpr(in Token prefix, ExpressionSyntax expr)
    {
        switch (prefix.Kind, expr)
        {
            case (TOKEN_PLUS, IntLiteralSyntax):
            case (TOKEN_PLUS, FloatLiteralSyntax):
                return expr;
            case (TOKEN_MINUS, IntLiteralSyntax { Value: var i }):
                return new IntLiteralSyntax(expr.Start, -i);
            case (TOKEN_MINUS, FloatLiteralSyntax { Value: var i }):
                return new FloatLiteralSyntax(expr.Start, -i);
            case (TOKEN_CLOSED_RANGE, _):
            case (TOKEN_OPEN_RANGE, _):
            case (TOKEN_RIGHT_OPEN_RANGE, _):
            case (TOKEN_LEFT_OPEN_RANGE, _):
                return new RangeLiteralSyntax(prefix, prefix.Kind, upper: expr);
            default:
                return new UnaryOperatorSyntax(prefix, expr);
        }
    }

    internal static ExpressionSyntax BinOpExpr(
        ExpressionSyntax left,
        in Token infix,
        ExpressionSyntax right
    )
    {
        switch (infix.Kind)
        {
            case TOKEN_CLOSED_RANGE:
            case TOKEN_OPEN_RANGE:
            case TOKEN_RIGHT_OPEN_RANGE:
            case TOKEN_LEFT_OPEN_RANGE:

            default:
                return new BinaryOperatorSyntax(left, infix, right);
        }
    }

    internal static Assoc Associativity(in TokenKind kind)
    {
        switch (kind)
        {
            case TOKEN_LESS_THAN:
            case TOKEN_GREATER_THAN:
            case TOKEN_LESS_THAN_EQUAL:
            case TOKEN_GREATER_THAN_EQUAL:
            case TOKEN_EQUAL:
            case TOKEN_NOT_EQUAL:
            case KEYWORD_IN:
            case KEYWORD_SUBSET:
            case KEYWORD_SUPERSET:
            case TOKEN_OPEN_RANGE:
            case TOKEN_CLOSED_RANGE:
            case TOKEN_LEFT_OPEN_RANGE:
            case TOKEN_RIGHT_OPEN_RANGE:
                return Assoc.None;
            case TOKEN_PLUS_PLUS:
                return Assoc.Right;
            default:
                return Assoc.Left;
        }
    }

    public const short MAX_PRECEDENCE = 2000;

    /// <summary>
    /// Get the precedence for the given operator
    /// </summary>
    /// <remarks>These numbers are taken from <see href="https://docs.minizinc.dev/en/stable/spec.html#operators"/> but
    /// inverted such that stronger binding has a higher number
    /// </remarks>
    internal static short Precedence(in TokenKind kind)
    {
        switch (kind)
        {
            case TOKEN_BI_IMPLICATION:
                return 800; //1200,
            case TOKEN_FORWARD_IMPLICATION:
            case TOKEN_REVERSE_IMPLICATION:
                return 900; //1100,
            case TOKEN_DISJUNCTION:
            case KEYWORD_XOR:
            case TOKEN_CONJUNCTION:
                return 1100; //900,
            case TOKEN_LESS_THAN:
            case TOKEN_GREATER_THAN:
            case TOKEN_LESS_THAN_EQUAL:
            case TOKEN_GREATER_THAN_EQUAL:
            case TOKEN_EQUAL:
            case TOKEN_NOT_EQUAL:
                return 1200; //800
            case KEYWORD_IN:
            case KEYWORD_SUBSET:
            case KEYWORD_SUPERSET:
                return 1300; // 700
            case KEYWORD_UNION:
            case KEYWORD_DIFF:
            case KEYWORD_SYMDIFF:
                return 1400; //600
            case TOKEN_OPEN_RANGE:
            case TOKEN_CLOSED_RANGE:
            case TOKEN_LEFT_OPEN_RANGE:
            case TOKEN_RIGHT_OPEN_RANGE:
                return 1500; //500
            case TOKEN_PLUS:
            case TOKEN_MINUS:
                return 1600; //400
            case TOKEN_TIMES:
            case TOKEN_DIVIDE:
            case KEYWORD_MOD:
            case KEYWORD_DIV:
            case KEYWORD_INTERSECT:
            case TOKEN_TILDE_PLUS:
            case TOKEN_TILDE_MINUS:
            case TOKEN_TILDE_TIMES:
            case TOKEN_TILDE_EQUALS:
                return 1700; //300
            case TOKEN_EXPONENT:
                return 1800; //200
            case TOKEN_PLUS_PLUS:
                return 1900; //100
            case KEYWORD_DEFAULT:
                return 1930; // 70
            case TOKEN_IDENTIFIER_INFIX:
                return 1950; //50
            default:
                return -1;
        }
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
    internal bool ParseIdentifierExpr([NotNullWhen(true)] out ExpressionSyntax? result)
    {
        result = null;
        var name = _current;
        Step();

        // Simple identifier
        if (!Skip(TOKEN_OPEN_PAREN))
        {
            result = new IdentifierSyntax(name);
            return true;
        }

        // Function call without arguments
        if (Skip(TOKEN_CLOSE_PAREN))
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
                Operator: KEYWORD_IN,
                Left: IdentifierSyntax id,
                Right: { } from
            } when maybeGen:
                if (!Skip(KEYWORD_WHERE))
                {
                    exprs.Add(expr);
                    break;
                }

                // The where indicates this definitely is a gencall
                isGen = true;
                if (!ParseExpr(out var where))
                    return false;

                expr = new GeneratorSyntax(expr.Start, [id.Name]) { From = from, Where = where };
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

        if (Skip(TOKEN_COMMA))
        {
            if (isGen)
                goto next;

            if (_kind is not TOKEN_CLOSE_PAREN)
                goto next;
        }

        if (!Expect(TOKEN_CLOSE_PAREN))
            return false;

        // For sure it's just a call
        if (!maybeGen)
        {
            result = new CallSyntax(name, exprs);
            return true;
        }

        // Could be a gencall if followed by (
        if (!isGen && _kind is not TOKEN_OPEN_PAREN)
        {
            result = new CallSyntax(name, exprs);
            return true;
        }

        if (!Expect(TOKEN_OPEN_PAREN))
            return false;

        if (!ParseExpr(out var yields))
            return false;

        if (!Expect(TOKEN_CLOSE_PAREN))
            return false;

        var generators = new List<GeneratorSyntax>();
        var gencall = new GeneratorCallSyntax(name, yields, generators);

        List<Token>? idents = null;
        for (i = 0; i < exprs.Count; i++)
        {
            switch (exprs[i])
            {
                // Identifiers must be collected as they are part of generators
                case IdentifierSyntax id:
                    idents ??= [];
                    idents.Add(id.Name);
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
                    idents ??= [];
                    idents.Add(((IdentifierSyntax)binop.Left).Name);
                    var gen = new GeneratorSyntax(default, idents) { From = binop.Right };
                    generators.Add(gen);
                    idents = null;
                    break;
            }
        }

        result = gencall;
        return true;
    }

    private bool ParseGenerators([NotNullWhen(true)] out List<GeneratorSyntax>? generators)
    {
        var start = _current;
        generators = null;

        begin:
        var names = new List<Token>();
        while (true)
        {
            Token name;
            if (_kind is TOKEN_UNDERSCORE)
            {
                name = _current;
                Step();
            }
            else if (!ParseIdent(out name))
            {
                return Error("Expected identifier in generator names");
            }

            names.Add(name);
            if (Skip(TOKEN_COMMA))
                continue;

            break;
        }

        if (!Expect(KEYWORD_IN))
            return false;

        if (!ParseExpr(out var from))
            return false;

        var gen = new GeneratorSyntax(start, names) { From = from };

        if (Skip(KEYWORD_WHERE))
            if (!ParseExpr(out var where))
                return false;
            else
                gen.Where = where;

        generators ??= [];
        generators.Add(gen);

        if (Skip(TOKEN_COMMA))
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

        if (!Expect(TOKEN_OPEN_BRACKET, out var start))
            return false;

        if (_kind is TOKEN_CLOSE_BRACKET)
        {
            result = new Array1dSyntax(start);
            return Expect(TOKEN_CLOSE_BRACKET);
        }

        if (Skip(TOKEN_PIPE))
        {
            if (_kind is TOKEN_PIPE)
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
        bool indexed = Skip(TOKEN_COLON);
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
        if (Skip(TOKEN_PIPE))
        {
            if (!ParseGenerators(out var generators))
                return false;

            result = new ComprehensionSyntax(start, element)
            {
                IsSet = false,
                Generators = generators
            };

            return Expect(TOKEN_CLOSE_BRACKET);
        }

        // 1D Array literal
        var arr1d = new Array1dSyntax(start);
        result = arr1d;
        arr1d.Elements.Add(element);

        while (true)
        {
            if (!Skip(TOKEN_COMMA))
                return Expect(TOKEN_CLOSE_BRACKET);

            if (Skip(TOKEN_CLOSE_BRACKET))
                return true;

            if (indexed)
            {
                if (!ParseExpr(out index!))
                    return false;
                if (!Skip(TOKEN_COLON))
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

        if (Skip(TOKEN_PIPE))
            return Expect(TOKEN_CLOSE_BRACKET);

        if (!ParseExpr(out var value))
            return false;

        int j = 1;

        if (!Skip(TOKEN_COLON))
        {
            // If first value is not an index skip the rest of the check
            arr.Elements.Add(value);
            Skip(TOKEN_COMMA);
            goto parse_row_values;
        }

        arr.Indices.Add(value);

        if (Skip(TOKEN_PIPE))
        {
            arr.ColIndexed = true;
            goto parse_row_values;
        }

        arr.ColIndexed = true;
        arr.RowIndexed = true;

        while (_kind is not TOKEN_PIPE)
        {
            j++;

            if (!ParseExpr(out value))
                return false;

            if (Skip(TOKEN_COLON))
            {
                if (!arr.ColIndexed)
                    return Error("Invalid : in row indexed array literal");

                arr.RowIndexed = false;
                arr.Indices.Add(value);
                continue;
            }

            arr.ColIndexed = false;
            arr.Elements.Add(value);

            if (!Skip(TOKEN_COMMA))
                break;
        }

        arr.I = 1;
        arr.J = j;

        if (!Expect(TOKEN_PIPE))
            return false;

        if (Skip(TOKEN_CLOSE_BRACKET))
            return true;

        /* Use the second row if necessary to detect dual
         * indexing */
        if (!arr.RowIndexed)
        {
            if (!ParseExpr(out value))
                return false;

            if (Skip(TOKEN_COLON))
            {
                arr.RowIndexed = true;
                arr.Indices.Add(value);
            }
            else
            {
                arr.Elements.Add(value);
                Skip(TOKEN_COMMA);
            }

            goto parse_row_values;
        }

        parse_row_index:
        if (!ParseExpr(out value))
            return false;

        if (!Expect(TOKEN_COLON))
            return false;

        arr.Indices.Add(value);

        parse_row_values:
        arr.I++;
        while (_kind is not TOKEN_PIPE)
        {
            if (!ParseExpr(out value))
                return false;

            j++;

            arr.Elements.Add(value);

            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (arr.J is 0)
            arr.J = j;

        if (!Expect(TOKEN_PIPE))
            return false;

        // Optional double pipe at the end
        // [|1, 2,|3, 4,||]
        if (Skip(TOKEN_PIPE))
            return Expect(TOKEN_CLOSE_BRACKET);

        if (Skip(TOKEN_CLOSE_BRACKET))
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

        if (!Expect(TOKEN_PIPE))
            return false;

        // Check for empty literal
        // `[| | | |]`
        if (Skip(TOKEN_PIPE))
        {
            Expect(TOKEN_PIPE);
            Expect(TOKEN_CLOSE_BRACKET);
            return IsOk;
        }

        int i = 0;

        table:
        i++;
        int j = 0;

        row:
        j++;
        int k = 0;

        while (_kind is not TOKEN_PIPE)
        {
            if (!ParseExpr(out var expr))
                return false;
            k++;
            arr.Elements.Add(expr);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (!Expect(TOKEN_PIPE))
            return false;

        if (Skip(TOKEN_COMMA))
        {
            if (!Expect(TOKEN_PIPE))
                return false;
            goto table;
        }

        if (!Skip(TOKEN_PIPE))
            goto row;

        arr.I = i;
        arr.J = j;
        arr.K = k;
        if (!Expect(TOKEN_CLOSE_BRACKET))
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

        if (!Expect(TOKEN_OPEN_BRACE, out var start))
            return false;

        // Empty Set
        if (_kind is TOKEN_CLOSE_BRACE)
        {
            result = new SetLiteralSyntax(start);
            return Expect(TOKEN_CLOSE_BRACE);
        }

        // Parse first expression
        if (!ParseExpr(out var element))
            return false;

        // Set comprehension
        if (Skip(TOKEN_PIPE))
        {
            if (!ParseGenerators(out var generators))
                return false;

            result = new ComprehensionSyntax(start, element)
            {
                IsSet = true,
                Generators = generators
            };
            return Expect(TOKEN_CLOSE_BRACE);
        }

        // Set literal
        var set = new SetLiteralSyntax(start);
        result = set;
        set.Elements.Add(element);
        Skip(TOKEN_COMMA);
        while (_kind is not TOKEN_CLOSE_BRACE)
        {
            if (!ParseExpr(out var item))
                return false;

            set.Elements.Add(item);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        return Expect(TOKEN_CLOSE_BRACE);
    }

    internal bool ParseLetExpr([NotNullWhen(true)] out ExpressionSyntax? let)
    {
        let = null;

        if (!Expect(KEYWORD_LET, out var start))
            return false;

        if (!Expect(TOKEN_OPEN_BRACE))
            return false;

        List<ILetLocalSyntax>? locals = null;

        while (_kind is not TOKEN_CLOSE_BRACE)
        {
            if (!ParseLetLocal(out var local))
                return false;

            locals ??= [];
            locals.Add(local);

            if (Skip(TOKEN_EOL) || Skip(TOKEN_COMMA))
                continue;
            break;
        }

        if (!Expect(TOKEN_CLOSE_BRACE))
            return false;

        if (!Expect(KEYWORD_IN))
            return false;

        if (!ParseExpr(out var body))
            return false;

        let = new LetSyntax(start, locals, body);
        return true;
    }

    private bool ParseLetLocal(out ILetLocalSyntax result)
    {
        result = null!;
        if (_kind is KEYWORD_CONSTRAINT)
        {
            if (ParseConstraintStatement(out var con))
            {
                result = (ConstraintStatement)con;
                return true;
            }

            return false;
        }

        if (!ParseDeclareOrAssignStatement(out var node))
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

        if (!Skip(KEYWORD_THEN))
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
        var start = _current;

        if (!ParseIfThenCase(out var ifCase, out var thenCase, KEYWORD_IF))
            return false;

        var ite = new IfThenSyntax(start, ifCase, thenCase);
        result = ite;

        while (ParseIfThenCase(out ifCase, out thenCase, KEYWORD_ELSEIF))
        {
            ite.ElseIfs ??= [];
            ite.ElseIfs.Add((ifCase, thenCase!));
        }

        if (Skip(KEYWORD_ELSE))
        {
            if (!ParseExpr(out var @else))
                return false;

            ite.Else = @else;
        }

        if (!Expect(KEYWORD_ENDIF))
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

        if (!Expect(TOKEN_OPEN_PAREN, out var start))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        // Bracketed expr
        if (Skip(TOKEN_CLOSE_PAREN))
        {
            result = expr;
            return true;
        }

        // Record expr
        if (Skip(TOKEN_COLON))
        {
            if (expr is not IdentifierSyntax { Name: var name })
                return Expected("Identifier");

            var record = new RecordLiteralSyntax(start);
            result = record;
            if (!ParseExpr(out expr))
                return false;

            var field = (name, expr);
            record.Fields.Add(field);

            record_field:
            if (!Skip(TOKEN_COMMA))
                return Expect(TOKEN_CLOSE_PAREN);

            if (Skip(TOKEN_CLOSE_PAREN))
                return true;

            if (!ParseIdent(out name))
                return false;

            if (!Expect(TOKEN_COLON))
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
        if (!Expect(TOKEN_COMMA))
            return false;
        while (_kind is not TOKEN_CLOSE_PAREN)
        {
            if (!ParseExpr(out expr))
                return false;
            tuple.Fields.Add(expr);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        return Expect(TOKEN_CLOSE_PAREN);
    }

    /// <summary>
    /// Parse annotations
    /// </summary>
    /// <returns>True if no error was encountered</returns>
    internal bool ParseAnnotations(out List<ExpressionSyntax>? annotations)
    {
        annotations = null;
        while (Skip(TOKEN_COLON_COLON))
        {
            ExpressionSyntax? ann;

            // Edge case where 'output' keyword can be used
            // in a non-keyword context, eg:
            // int : x :: output = 3;
            if (_kind is KEYWORD_OUTPUT)
            {
                ann = new IdentifierSyntax(_current);
                Step();
            }
            else if (!ParseExprAtom(out ann))
            {
                return Expected("Annotation");
            }

            annotations ??= [];
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
    internal bool ParseStringAnnotations(out List<ExpressionSyntax>? annotations)
    {
        annotations = null;
        while (Skip(TOKEN_COLON_COLON))
        {
            if (!ParseString(out var ann))
                return false;
            annotations ??= [];
            annotations.Add(new StringLiteralSyntax(ann));
        }

        return true;
    }

    internal bool ParseBaseType([NotNullWhen(true)] out TypeSyntax? type)
    {
        type = default;
        var start = _current;

        if (Skip(KEYWORD_ANY))
        {
            var ident = _current;
            if (ident.Kind is TOKEN_IDENTIFIER_GENERIC or TOKEN_IDENTIFIER_GENERIC_SEQUENCE)
            {
                Step();
                type = new TypeSyntax(start) { Name = ident, Kind = TypeKind.Identifier };
                return true;
            }
            else
            {
                return Expected("Generic Variable ($$T, $T)");
            }
        }

        var var = false;
        if (Skip(KEYWORD_VAR))
            var = true;
        else
            Skip(KEYWORD_PAR);

        var opt = Skip(KEYWORD_OPT);

        if (Skip(KEYWORD_SET))
        {
            if (!Expect(KEYWORD_OF))
                return false;

            if (!ParseBaseType(out type))
                return false;

            type = new SetTypeSyntax(start, type) { Var = var, Opt = opt };
            return true;
        }

        start = _current;
        switch (_kind)
        {
            case KEYWORD_INT:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Int };
                break;

            case KEYWORD_BOOL:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Bool };
                break;

            case KEYWORD_FLOAT:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Float };
                break;

            case KEYWORD_STRING:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.String };
                break;

            case KEYWORD_ANN:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Ann };
                break;

            case KEYWORD_RECORD:
                if (!ParseRecordType(out type))
                    return false;
                break;

            case KEYWORD_TUPLE:
                if (!ParseTupleType(out type))
                    return false;
                break;

            case TOKEN_IDENTIFIER_GENERIC:
                Step();
                type = new TypeSyntax(start) { Name = start, Kind = TypeKind.Identifier };
                break;

            case TOKEN_IDENTIFIER_GENERIC_SEQUENCE:
                Step();
                type = new TypeSyntax(start) { Kind = TypeKind.Identifier, Name = start };
                break;

            default:
                if (!ParseExpr(out var expr, typeInst: true))
                    return false;
                type = new ExprTypeSyntax(start, expr);
                break;
        }

        type.Var = var;
        type.Opt = opt;
        return true;
    }

    /// <summary>
    /// Parse an array type
    /// </summary>
    /// <mzn>array[X, 1..2] of var int</mzn>
    internal bool ParseArrayType([NotNullWhen(true)] out TypeSyntax? arr)
    {
        arr = default;
        var dims = new List<TypeSyntax>();

        if (!Expect(KEYWORD_ARRAY, out var start))
            return false;

        if (!Expect(TOKEN_OPEN_BRACKET))
            return false;

        while (ParseBaseType(out var expr))
        {
            dims.Add(expr);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (_errorMessage is not null)
            return false;

        if (!Expect(TOKEN_CLOSE_BRACKET))
            return false;

        if (!Expect(KEYWORD_OF))
            return false;

        if (!ParseType(out var type))
            return false;

        arr = new ArrayTypeSyntax(start, dims, type) { Kind = TypeKind.Array };
        return true;
    }

    /// <summary>
    /// Parse a tuple type constructor
    /// </summary>
    /// <mzn>tuple(int, bool, tuple(int))</mzn>
    private bool ParseTupleType([NotNullWhen(true)] out TypeSyntax? tuple)
    {
        tuple = null;
        if (!Expect(KEYWORD_TUPLE, out var start))
            return false;

        if (!Expect(TOKEN_OPEN_PAREN))
            return false;

        List<TypeSyntax> items = [];
        while (_kind is not TOKEN_CLOSE_PAREN)
        {
            if (!ParseType(out var ti))
                return false;

            items.Add(ti);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        tuple = new TupleTypeSyntax(start, items);
        return Expect(TOKEN_CLOSE_PAREN);
    }

    /// <summary>
    /// Parse a record type constructor
    /// </summary>
    /// <mzn>record(int: a, bool: b)</mzn>
    private bool ParseRecordType([NotNullWhen(true)] out TypeSyntax? record)
    {
        record = null;

        if (!Expect(KEYWORD_RECORD, out var start))
            return false;

        if (!ParseParameters(out var fields))
            return false;

        record = new RecordTypeSyntax(start, fields);
        return true;
    }

    /// <summary>
    /// Parse an comma separated list of types
    /// and names between parentheses
    /// </summary>
    /// <mzn>(int: a, bool: b)</mzn>
    private bool ParseParameters([NotNullWhen(true)] out List<ParameterSyntax>? parameters)
    {
        parameters = default;
        if (!Expect(TOKEN_OPEN_PAREN))
            return false;

        Token name;
        List<ExpressionSyntax>? anns;
        TypeSyntax? type;

        parameters = [];
        while (_kind is not TOKEN_CLOSE_PAREN)
        {
            if (!ParseType(out type))
                return false;

            // Parameters can sometimes have a type only which
            // seems like a really bad idea and it quite confusing
            // but what can you do.  In practice this is extremely rare
            if (!Skip(TOKEN_COLON))
            {
                name = default;
                anns = null;
            }
            else
            {
                if (!ParseIdent(out name))
                    return false;
                if (!ParseAnnotations(out anns))
                    return false;
            }

            var param = new ParameterSyntax(type.Start, type, name) { Annotations = anns };
            parameters.Add(param);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        return Expect(TOKEN_CLOSE_PAREN);
    }

    internal bool ParseType([NotNullWhen(true)] out TypeSyntax? result)
    {
        result = default;
        var start = _current;
        switch (_kind)
        {
            case KEYWORD_ARRAY:
                if (!ParseArrayType(out result))
                    return false;
                break;

            case KEYWORD_LIST:
                if (!ParseListType(out result))
                    return false;
                break;

            default:
                if (!ParseBaseType(out result))
                    return false;
                break;
        }

        List<TypeSyntax>? types = null;

        while (Skip(TOKEN_PLUS_PLUS) || Skip(KEYWORD_UNION))
        {
            types ??= [result];
            if (!ParseBaseType(out result))
                return false;
            types.Add(result);
        }

        if (types is not null)
            result = new CompositeTypeSyntax(start, types);

        return true;
    }

    /// <summary>
    /// Parse a list type
    /// </summary>
    /// <mzn>list of var int</mzn>
    private bool ParseListType([NotNullWhen(true)] out TypeSyntax? list)
    {
        list = null!;

        if (!Expect(KEYWORD_LIST, out var start))
            return false;

        if (!Expect(KEYWORD_OF))
            return false;

        if (!ParseBaseType(out var items))
            return false;

        list = new ListTypeSyntax(start, items);
        return true;
    }

    private bool Expected(string msg) => Error($"Expected {msg}");

    private bool Unexpected(string msg) => Error($"Unexpected {msg}");

    /// Record the given message as an error and return false
    private bool Error(string? msg = null)
    {
        if (_errorMessage is not null)
            return false;

        _errorTrace = _sourceText[.._current.End];
        _errorTrace = $"""
            ---------------------------------------------
            {msg}
            ---------------------------------------------
            Token {_kind}
            Line {_current.Line}
            Col {_current.Col}
            Pos {_current.Start}
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
    public static ParseResult ParseModelFile(string path, out ModelSyntax model)
    {
        var watch = Stopwatch.StartNew();
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        var ok = parser.ParseModel(out model);
        var elapsed = watch.Elapsed;
        var result = new ParseResult
        {
            SourceFile = path,
            SourceText = mzn,
            Ok = ok,
            FinalToken = parser._current,
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
    public static ParseResult ParseModelString(string text, out ModelSyntax model)
    {
        var watch = Stopwatch.StartNew();
        var parser = new Parser(text);
        var ok = parser.ParseModel(out model);
        var elapsed = watch.Elapsed;
        var result = new ParseResult
        {
            SourceFile = null,
            SourceText = text,
            Ok = ok,
            FinalToken = parser._current,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <inheritdoc cref="ParseModelFile(string,out MiniZinc.Parser.ModelSyntax)"/>
    public static ParseResult ParseModelFile(FileInfo file, out ModelSyntax model) =>
        ParseModelFile(file.FullName, out model);

    /// <summary>
    /// Parse the given minizinc data file.
    /// Data files only allow assignments eg: `a = 10;`
    /// </summary>
    /// <example>Parser.ParseDataFile("data.dzn")</example>
    public static ParseResult ParseDataFile(string path, out MiniZincData data)
    {
        var watch = Stopwatch.StartNew();
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        var ok = parser.ParseData(out data);
        var elapsed = watch.Elapsed;
        var result = new ParseResult
        {
            SourceFile = path,
            SourceText = mzn,
            Ok = ok,
            FinalToken = parser._current,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <summary>
    /// Parse a <see cref="Datum"/> from the given string.
    /// </summary>
    /// <example>
    /// Parser.ParseDatum("{1, 2, 3}"); // IntSet([1,2,3]);
    /// </example>
    public static bool ParseDatum(string text, [NotNullWhen(true)] out Datum? datum)
    {
        datum = null;
        var parser = new Parser(text);
        if (!parser.ParseDatum(out datum))
            return false;
        return true;
    }

    /// <summary>
    /// Parse a <see cref="Datum"/> from the given string.
    /// </summary>
    /// <example>
    /// Parser.ParseDatum%ltIntArray%gt("[1,2,1,2,3]"); // IntArray([1,2,3]);
    /// </example>
    public static bool ParseDatum<T>(string text, [NotNullWhen(true)] out T? datum)
        where T : Datum
    {
        datum = null;
        var parser = new Parser(text);
        if (!parser.ParseDatum(out var mzDatum))
            return false;
        if (mzDatum is not T t)
            return false;
        datum = t;
        return true;
    }

    /// <summary>
    /// Parse the given minizinc data string.
    /// Data strings only allow assignments eg: `a = 10;`
    /// </summary>
    /// <example>
    /// Parser.ParseDataString("a = 10; b=true;");
    /// </example>
    public static ParseResult ParseDataString(string text, out MiniZincData data)
    {
        var watch = Stopwatch.StartNew();
        var parser = new Parser(text);
        var ok = parser.ParseData(out data);
        var elapsed = watch.Elapsed;
        var result = new ParseResult
        {
            SourceFile = null,
            SourceText = text,
            Ok = ok,
            FinalToken = parser._current,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <inheritdoc cref="ParseDataFile(string,out MiniZincData)"/>
    public static ParseResult ParseDataFile(FileInfo file, out MiniZincData data) =>
        ParseDataFile(file.FullName, out data);

    /// Parse an expression of the given type from text
    public static T ParseExpression<T>(string text)
        where T : ExpressionSyntax
    {
        var parser = new Parser(text);
        if (!parser.ParseExpr(out var expr))
            throw new MiniZincParseException(
                parser._errorMessage ?? "",
                parser._current,
                parser._errorTrace
            );

        if (expr is not T result)
        {
            throw new MiniZincParseException(
                $"The parsed expression is of type {expr.GetType()} but expected {typeof(T)}",
                parser._current
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
                parser._current,
                parser._errorTrace
            );

        if (statement is not T result)
        {
            throw new MiniZincParseException(
                $"The parsed statement is of type {statement.GetType()} but expected a {typeof(T)}",
                parser._current
            );
        }

        return result;
    }

    /// Try to parse a statement of the given type from text
    public static bool TryParseStatement<T>(string text, [NotNullWhen(true)] out T? result)
        where T : StatementSyntax
    {
        result = null;
        var parser = new Parser(text);
        if (!parser.ParseStatement(out var statement))
            return false;

        if (statement is not T t)
            return false;
        result = t;
        return true;
    }

    /// Parse an expression of the given type from text
    public static bool TryParseExpression<T>(string text, [NotNullWhen(true)] out T? expression)
        where T : ExpressionSyntax
    {
        expression = null;
        var parser = new Parser(text);
        if (!parser.ParseExpr(out var expr))
            return false;

        if (expr is not T t)
            return false;
        expression = t;
        return true;
    }
}
