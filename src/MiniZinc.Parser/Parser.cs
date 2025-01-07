namespace MiniZinc.Parser;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static SolveMethod;
using static TokenKind;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public struct Parser
{
    /// <summary>
    /// Operator Associativity
    /// </summary>
    internal enum Assoc : byte
    {
        None = 1,
        Left,
        Right
    }

    private readonly Token[] _tokens;
    private readonly int _n;
    private string _sourceText;
    private Token _token;
    private int _i;
    private string? _errorMessage;
    private string? _errorTrace;

    internal Parser(string sourceText)
    {
        _sourceText = sourceText;
        if (!Lexer.LexString(sourceText, out _tokens, out var final))
            Error(final.StringValue);

        _n = _tokens.Length - 1;
        _i = -1;
        Step();
    }

    /// Progress the parser
    private void Step()
    {
        while (_i < _n)
        {
            _i++;
            _token = _tokens[_i];
            if (_token.Kind is not (TOKEN_LINE_COMMENT or TOKEN_BLOCK_COMMENT))
                break;
        }
    }

    /// Backtrack to the given position and clear any error information
    private void Backtrack(int pos)
    {
        _i = pos;
        _token = _tokens[_i];
        _errorMessage = null;
        _errorTrace = null;
    }

    /// Return the next non-comment token after the current one
    private Token Peek()
    {
        for (int j = _i + 1; j < _n; j++)
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
        if (_token.Kind != kind)
            return false;

        Step();
        return true;
    }

    /// Progress the parser if the current token is of the given kind
    private bool Skip(TokenKind a, TokenKind b)
    {
        if (_token.Kind != a)
            return false;

        Token next = Peek();
        if (next.Kind != b)
            return false;

        Step();
        Step();
        return true;
    }

    /// Returns true if the next token is of the given time
    private bool Peek(TokenKind kind)
    {
        int i = _i + 1;
        if (i >= _n)
            return false;

        var next = _tokens[i];
        if (next.Kind != kind)
            return false;

        return true;
    }

    private bool Skip(TokenKind kind, out Token token)
    {
        token = _token;
        if (_token.Kind != kind)
            return false;
        Step();
        return true;
    }

    private bool Skip(TokenKind a, TokenKind b, out Token first, out Token second)
    {
        int i = _i + 1;
        int j = _i + 2;
        if (j >= _n)
        {
            first = default;
            second = default;
            return false;
        }

        first = _tokens[i];
        second = _tokens[j];
        if (a != first.Kind)
            return false;
        if (b != second.Kind)
            return false;

        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind)
    {
        if (_token.Kind != kind)
            return Expected($"a {kind} but encountered a {_token.Kind}");

        Step();
        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind, out Token token)
    {
        token = _token;
        if (token.Kind != kind)
            return Expected($"a {kind} but encountered a {_token.Kind}");

        Step();
        return true;
    }

    private bool ParseIdent(out Token node)
    {
        if (_token.Kind is TOKEN_IDENTIFIER)
        {
            node = _token;
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
    internal bool ParseItems(out List<MiniZincItem>? statements)
    {
        statements = null;
        while (true)
        {
            if (!ParseItem(out var statement))
                return false;

            statements ??= [];
            statements.Add(statement);

            if (Skip(TOKEN_EOL) && !Skip(TOKEN_EOF))
                continue;
            else if (!Expect(TOKEN_EOF))
                return false;

            return true;
        }
    }

    internal bool ParseItem([NotNullWhen(true)] out MiniZincItem? statement)
    {
        statement = null;
        switch (_token.Kind)
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
        Dictionary<string, MiniZincExpr> values = new();
        data = new MiniZincData(values);

        while (true)
        {
            if (_token.Kind is TOKEN_EOF)
                return true;

            if (!ParseIdent(out var ident))
                return false;

            if (ident.StringValue is not { } name)
                return false;

            if (!Expect(TOKEN_EQUAL))
                return false;

            if (!ParseExpr(out var expr))
                return false;

            // if (expr is not IMiniZincDatum datum)
            //     return false;
            // TODO - Datum Check

            if (values.ContainsKey(name))
                return Error($"Variable \"{name}\" was assigned to multiple times");

            values.Add(name, expr);

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
    private bool ParsePredicateStatement([NotNullWhen(true)] out MiniZincItem? node)
    {
        node = null;
        if (!Expect(KEYWORD_PREDICATE, out var start))
            return false;

        var type = new BaseTypeSyntax(start, TypeKind.TYPE_BOOL) { IsVar = true };

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.DECLARE_PREDICATE, out var dec))
            return false;

        node = dec;
        return true;
    }

    /// <summary>
    /// Parse a test declaration
    /// </summary>
    /// <mzn>predicate ok(int: x) = x > 0;</mzn>
    private bool ParseTestStatement([NotNullWhen(true)] out MiniZincItem? node)
    {
        node = null;
        if (!Expect(KEYWORD_TEST, out var start))
            return false;

        var type = new BaseTypeSyntax(start, TypeKind.TYPE_BOOL);

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.DECLARE_TEST, out var decl))
            return false;

        node = decl;
        return true;
    }

    /// <summary>
    /// Parse a function declaration
    /// </summary>
    /// <mzn>function bool: opposite(bool: x) = not x;</mzn>
    private bool ParseFunctionStatement([NotNullWhen(true)] out MiniZincItem? node)
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

        if (!ParseDeclareTail(start, ident, type, DeclareKind.DECLARE_FUNCTION, out var decl))
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
        out DeclareItem node
    )
    {
        node = null!;
        Token? ann = null;
        List<ParameterSyntax>? parameters = null;

        if (_token.Kind is TOKEN_OPEN_PAREN)
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

        MiniZincExpr? body = null;
        if (Skip(TOKEN_EQUAL))
            if (!ParseExpr(out body))
                return false;

        node = new DeclareItem(start, kind, type, identifier)
        {
            Annotations = anns,
            Ann = ann,
            Parameters = parameters,
            Expr = body
        };
        return true;
    }

    /// <summary>
    /// Parse an annotation declaration
    /// </summary>
    /// <mzn>annotation custom;</mzn>
    /// <mzn>annotation custom(int: x);</mzn>
    private bool ParseAnnotationStatement([NotNullWhen(true)] out MiniZincItem? ann)
    {
        ann = null;
        if (!Expect(KEYWORD_ANNOTATION, out var start))
            return false;

        var type = new BaseTypeSyntax(start, TypeKind.TYPE_ANNOTATION);

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.DECLARE_ANNOTATION, out var decl))
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
    internal bool ParseEnumStatement([NotNullWhen(true)] out MiniZincItem? result)
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
            result = new DeclareItem(start, DeclareKind.DECLARE_ENUM, null, name)
            {
                Annotations = anns
            };
            return true;
        }

        if (!ParseEnumCases(out var cases))
            return false;

        result = new DeclareItem(start, DeclareKind.DECLARE_ENUM, null, name)
        {
            Annotations = anns,
            Expr = cases
        };
        return true;
    }

    private bool ValidateEnumCases(MiniZincExpr expr)
    {
        switch (expr)
        {
            case SetExpr { Elements: null }:
                break;

            // Named cases: 'enum Dir = {N,S,E,W};`
            case SetExpr set:
                foreach (var item in set.Elements)
                {
                    if (item is not IdentExpr name)
                        return Expected($"Enum case name, got {item}");
                }

                break;

            // Underscore enum `enum A = _(1..10);`
            case CallExpr { Name.Kind: TOKEN_UNDERSCORE } call:
                if (call.Args is not { Count: 1 })
                    return Expected($"Single argument call to _, got {call.Args}");
                var arg = call.Args[0];
                break;

            // Anonymous enum: `anon_enum(10);`
            case CallExpr { Name.StringValue: "anon_enum" } call:
                if (call.Args is not { Count: 1 })
                    return Expected($"Single argument call to _, got {call.Args}");
                var anonArg = call.Args[0];
                break;

            // Complex enum: `C(1..10)`
            case CallExpr call:
                break;

            // ++
            case BinOpExpr { Left: var left, Operator: TOKEN_PLUS_PLUS, Right: var right }:
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

    internal bool ParseEnumCases([NotNullWhen(true)] out MiniZincExpr? expr)
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
    internal bool ParseOutputStatement([NotNullWhen(true)] out MiniZincItem? node)
    {
        node = null;

        if (!Expect(KEYWORD_OUTPUT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        node = new OutputItem(start, expr) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse a type alias
    /// </summary>
    /// <mzn>type X = 1 .. 10;</mzn>
    internal bool ParseTypeAliasStatement([NotNullWhen(true)] out MiniZincItem? alias)
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
    internal bool ParseIncludeStatement([NotNullWhen(true)] out MiniZincItem? node)
    {
        node = null;

        if (!Expect(KEYWORD_INCLUDE, out var token))
            return false;

        if (!Expect(TOKEN_STRING_LITERAL, out var path))
            return false;

        node = new IncludeItem(token, path);
        return true;
    }

    /// <summary>
    /// Parse a solve item
    /// </summary>
    /// <mzn>solve satisfy;</mzn>
    /// <mzn>solve maximize a;</mzn>
    internal bool ParseSolveStatement([NotNullWhen(true)] out MiniZincItem? node)
    {
        node = null;

        if (!Expect(KEYWORD_SOLVE, out var start))
            return false;

        if (!ParseAnnotations(out var anns))
            return false;

        MiniZincExpr? objective = null;
        SolveMethod method = SOLVE_SATISFY;
        switch (_token.Kind)
        {
            case KEYWORD_SATISFY:
                Step();
                method = SOLVE_SATISFY;
                break;

            case KEYWORD_MINIMIZE:
                Step();
                method = SOLVE_MINIMIZE;
                if (!ParseExpr(out objective))
                    return false;
                break;

            case KEYWORD_MAXIMIZE:
                Step();
                method = SOLVE_MAXIMIZE;
                if (!ParseExpr(out objective))
                    return false;
                break;

            default:
                return Error("Expected satisfy, minimize, or maximize");
        }

        node = new SolveItem(start, method, objective) { Annotations = anns };
        return true;
    }

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    internal bool ParseConstraintStatement([NotNullWhen(true)] out MiniZincItem? constraint)
    {
        constraint = null;

        if (!Expect(KEYWORD_CONSTRAINT, out var start))
            return false;

        if (!ParseStringAnnotations(out var anns))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        constraint = new ConstraintItem(start, expr) { Annotations = anns };
        return IsOk;
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    /// <mzn>$T: identity($T: x) = x;</mzn>
    internal bool ParseDeclareOrAssignStatement([NotNullWhen(true)] out MiniZincItem? statement)
    {
        statement = null;
        var start = _token;

        if (_token.Kind is TOKEN_IDENTIFIER && Peek().Kind is TOKEN_EQUAL)
        {
            ParseIdent(out var name);
            Step();
            if (!ParseExpr(out var expr))
                return false;

            statement = new AssignItem(name, expr);
            return true;
        }

        TypeSyntax type;
        if (Skip(KEYWORD_ANY))
        {
            type = new BaseTypeSyntax(start, TypeKind.TYPE_ANY);
        }
        else if (!ParseType(out type!))
        {
            return false;
        }

        if (!Expect(TOKEN_COLON))
            return false;

        if (!ParseIdent(out var ident))
            return false;

        if (!ParseDeclareTail(start, ident, type, DeclareKind.DECLARE_VALUE, out var dec))
            return false;

        if (dec.Expr is null && type.Kind is TypeKind.TYPE_ANY)
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
    internal bool ParseExprAtom([NotNullWhen(true)] out MiniZincExpr? expr)
    {
        expr = null;
        var token = _token;

        switch (_token.Kind)
        {
            case TOKEN_INT_LITERAL:
                Step();
                expr = new IntExpr(token);
                break;

            case TOKEN_FLOAT_LITERAL:
                Step();
                expr = new FloatExpr(token);
                break;

            case KEYWORD_TRUE:
                Step();
                expr = new BoolExpr(token, true);
                break;

            case KEYWORD_FALSE:
                Step();
                expr = new BoolExpr(token, false);
                break;

            case TOKEN_STRING_LITERAL:
                Step();
                expr = new StringExpr(token);
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
                expr = new EmptyExpr(token);
                break;

            default:
                return false;
        }

        while (true)
        {
            if (Skip(TOKEN_OPEN_BRACKET))
            {
                // Array access eg: `a[1,2]`
                var indices = new List<MiniZincExpr>();
                while (_token.Kind is not TOKEN_CLOSE_BRACKET)
                {
                    MiniZincExpr? index = null;

                    // For array access only, the `..` token
                    // can be used.  We explicitly allow this here instead
                    // of in the general Expr parser.
                    if (Skip(TOKEN_RANGE_INCLUSIVE, out var idx))
                    {
                        index = new SliceExpr(idx);
                        indices.Add(index);
                    }
                    else if (ParseExpr(out index))
                    {
                        indices.Add(index);
                    }
                    else
                    {
                        return false;
                    }

                    if (!Skip(TOKEN_COMMA))
                        break;
                }

                expr = new ArrayAccessExpr(expr, indices);
                if (!Expect(TOKEN_CLOSE_BRACKET))
                    return false;
            }
            else if (_token.Kind is TOKEN_TUPLE_ACCESS)
            {
                expr = new TupleAccessExpr(expr, _token);
                Step();
            }
            else if (_token.Kind is TOKEN_RECORD_ACCESS)
            {
                expr = new RecordAccessExpr(expr, _token);
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

    // /// Parse an int range data starting at the given token
    // /// The parser will step over the given lower bound
    // /// Will only return false in the case of an error
    // internal bool ParseIntDatum(out int i, out IntRange? range)
    // {
    //     range = null;
    //     Token start = _current;
    //     i = start.IntValue;
    //     Step();
    //     if (!Skip(TOKEN_CLOSED_RANGE))
    //         return true;
    //     if (!Expect(TOKEN_INT_LITERAL, out Token upper))
    //         return false;
    //     range = new IntRange(i, upper.IntValue);
    //     return true;
    // }
    //
    // /// Parse a float range starting at the given token
    // /// The parser will step over the given lower bound
    // /// Will only return false in the case of an error
    // internal bool ParseFloatDatum(out decimal f, out FloatRange? range)
    // {
    //     range = null;
    //     Token start = _current;
    //     f = start.FloatValue;
    //     Step();
    //     if (!Skip(TOKEN_CLOSED_RANGE))
    //         return true;
    //     if (!Expect(TOKEN_FLOAT_LITERAL, out Token upper))
    //         return false;
    //     range = new FloatRange(f, upper.FloatValue);
    //     return true;
    // }

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
    // internal bool ParseArrayDatum([NotNullWhen(true)] out Datum? array)
    // {
    //     array = null;
    //     if (!Skip(TOKEN_OPEN_BRACKET))
    //         return false;
    //
    //     List<int>? ints = null;
    //     List<bool>? bools = null;
    //     List<decimal>? floats = null;
    //     List<string>? strings = null;
    //     List<Datum>? values = null;
    //     DatumKind type = default;
    //     bool opt = false;
    //     while (_token.Kind is not TOKEN_CLOSE_BRACKET)
    //     {
    //         switch (_token.Kind, type, opt)
    //         {
    //             // First value is <>
    //             case (TOKEN_EMPTY, DatumKind.Unknown, false):
    //                 values = [Datum.Empty];
    //                 opt = true;
    //                 break;
    //
    //             // <> found in IntArray
    //             case (TOKEN_EMPTY, DatumKind.Int, false):
    //                 values = [];
    //                 foreach (var ix in ints!)
    //                     values.Add(Datum.Int(ix));
    //                 values.Add(Datum.Empty);
    //                 opt = true;
    //                 break;
    //
    //             // <> found in FloatArray
    //             case (TOKEN_EMPTY, DatumKind.Float, false):
    //                 values = [];
    //                 foreach (var fx in floats!)
    //                     values.Add(Datum.Float(fx));
    //                 values.Add(Datum.Empty);
    //                 opt = true;
    //                 break;
    //
    //             // <> found in StringArray
    //             case (TOKEN_EMPTY, DatumKind.String, false):
    //                 values = [];
    //                 foreach (var s in strings!)
    //                     values.Add(Datum.String(s));
    //                 values.Add(Datum.Empty);
    //                 opt = true;
    //                 break;
    //
    //             // <> found in BoolArray
    //             case (TOKEN_EMPTY, DatumKind.Bool, false):
    //                 values = [];
    //                 foreach (var b in bools!)
    //                     values.Add(Datum.Bool(b));
    //                 values.Add(Datum.Empty);
    //                 opt = true;
    //                 break;
    //
    //             // <> occured
    //             case (TOKEN_EMPTY, _, _):
    //                 values!.Add(Datum.Empty);
    //                 opt = true;
    //                 break;
    //
    //             // Int or IntRange
    //             case (TOKEN_INT_LITERAL, _, _):
    //                 if (!ParseIntDatum(out int i, out var intRange))
    //                 {
    //                     return false;
    //                 }
    //                 else if (intRange is not null)
    //                 {
    //                     (values ??= []).Add(intRange);
    //                     if (type is not (DatumKind.Set or DatumKind.Unknown))
    //                         return Expected($"datum of type {type}, got IntRange");
    //                     type = DatumKind.Set;
    //                 }
    //                 else if (opt)
    //                 {
    //                     if (type is not (DatumKind.Int or DatumKind.Unknown))
    //                         return Expected($"datum of type {type}, got int");
    //
    //                     values!.Add(Datum.Int(i));
    //                     type = DatumKind.Int;
    //                 }
    //                 else
    //                 {
    //                     if (type is not (DatumKind.Int or DatumKind.Unknown))
    //                         return Expected($"datum of type {type}, got int");
    //
    //                     (ints ??= []).Add(i);
    //                     type = DatumKind.Int;
    //                 }
    //                 break;
    //
    //             // Float or FloatRange
    //             case (TOKEN_FLOAT_LITERAL, _, _):
    //                 if (!ParseFloatDatum(out decimal f, out var floatRange))
    //                 {
    //                     return false;
    //                 }
    //                 else if (floatRange is not null)
    //                 {
    //                     (values ??= []).Add(floatRange);
    //                     if (type is not (DatumKind.Set or DatumKind.Unknown))
    //                         return Expected($"datum of type {type}, got FloatRange");
    //                     type = DatumKind.Set;
    //                 }
    //                 else if (opt)
    //                 {
    //                     if (type is not (DatumKind.Float or DatumKind.Unknown))
    //                         return Expected($"datum of type {type}, got Float");
    //
    //                     values!.Add(Datum.Float(f));
    //                     type = DatumKind.Float;
    //                 }
    //                 else
    //                 {
    //                     if (type is not (DatumKind.Float or DatumKind.Unknown))
    //                         return Expected($"datum of type {type}, got Float");
    //
    //                     (floats ??= []).Add(f);
    //                     type = DatumKind.Float;
    //                 }
    //                 break;
    //
    //             case (KEYWORD_TRUE, DatumKind.Unknown, false):
    //                 bools = [true];
    //                 type = DatumKind.Bool;
    //                 Step();
    //                 break;
    //
    //             case (KEYWORD_TRUE, DatumKind.Bool, false):
    //                 bools!.Add(true);
    //                 Step();
    //                 break;
    //
    //             case (KEYWORD_FALSE, DatumKind.Unknown, false):
    //                 bools = [false];
    //                 type = DatumKind.Bool;
    //                 Step();
    //                 break;
    //
    //             case (KEYWORD_FALSE, DatumKind.Bool, false):
    //                 bools!.Add(false);
    //                 Step();
    //                 break;
    //
    //             case (TOKEN_STRING_LITERAL, DatumKind.Unknown or DatumKind.String, false):
    //                 (strings ??= []).Add(_current.StringValue);
    //                 type = DatumKind.String;
    //                 Step();
    //                 break;
    //
    //             case (TOKEN_STRING_LITERAL, DatumKind.Unknown or DatumKind.String, true):
    //                 values!.Add(Datum.String(_current.StringValue));
    //                 type = DatumKind.String;
    //                 Step();
    //                 break;
    //
    //             default:
    //                 if (!ParseDatum(out var datum))
    //                     return false;
    //                 if (type is not DatumKind.Unknown)
    //                     if (type != datum.Kind)
    //                         return Expected($"Datum of type {type}, got {datum.Kind}");
    //                 type = datum.Kind;
    //                 (values ??= []).Add(datum);
    //                 break;
    //         }
    //
    //         if (!Skip(TOKEN_COMMA))
    //             break;
    //     }
    //
    //     if (!Expect(TOKEN_CLOSE_BRACKET))
    //         return false;
    //
    //     if (ints is not null)
    //         array = new IntArray(ints);
    //     else if (floats is not null)
    //         array = new FloatArray(floats);
    //     else if (bools is not null)
    //         array = new BoolArray(bools);
    //     else if (strings is not null)
    //         array = new StringArray(strings);
    //     else
    //         array = new DatumArray(values ?? []);
    //     return true;
    // }

    /// Parse set data into the most refined type possible
    // internal bool ParseSetDatum([NotNullWhen(true)] out Datum? set)
    // {
    //     set = null;
    //     if (!Skip(TOKEN_OPEN_BRACE))
    //         return false;
    //
    //     List<int>? ints = null;
    //     List<bool>? bools = null;
    //     List<decimal>? floats = null;
    //     List<Datum>? data = null;
    //
    //     while (_token.Kind is not TOKEN_CLOSE_BRACE)
    //     {
    //         var token = _current;
    //         Step();
    //         switch (token.Kind)
    //         {
    //             case TOKEN_INT_LITERAL:
    //                 (ints ??= []).Add(token.IntValue);
    //                 break;
    //
    //             case TOKEN_FLOAT_LITERAL:
    //                 (floats ??= []).Add(token.FloatValue);
    //                 break;
    //
    //             case KEYWORD_TRUE:
    //                 (bools ??= []).Add(true);
    //                 break;
    //
    //             case KEYWORD_FALSE:
    //                 (bools ??= []).Add(false);
    //                 break;
    //
    //             default:
    //                 return Error($"Unexpected {token} in set data");
    //         }
    //
    //         if (!Skip(TOKEN_COMMA))
    //             break;
    //     }
    //
    //     if (!Expect(TOKEN_CLOSE_BRACE))
    //         return false;
    //
    //     if (ints is not null)
    //         set = new IntSet(ints);
    //     else if (floats is not null)
    //         set = new FloatSet(floats);
    //     else if (bools is not null)
    //         set = new BoolSet(bools);
    //     else
    //         set = new SetDatum(data ?? []);
    //     return true;
    // }

    // /// <summary>
    // /// Parse a <see cref="Datum"/>.
    // /// </summary>
    // /// <mzn>1</mzn>
    // /// <mzn>true</mzn>
    // /// <mzn>{1,2,3}</mzn>
    // /// <mzn>1..10</mzn>
    // public bool ParseDatum([NotNullWhen(true)] out IMiniZincDatum? datum)
    // {
    //     datum = null;
    //     switch (_token.Kind)
    //     {
    //         case TOKEN_OPEN_BRACE:
    //             if (!ParseSetDatum(out datum))
    //                 return false;
    //             break;
    //
    //         case TOKEN_OPEN_BRACKET:
    //             if (!ParseArrayDatum(out datum))
    //                 return false;
    //             break;
    //
    //         case TOKEN_OPEN_PAREN when Peek().Kind is TOKEN_IDENTIFIER:
    //             if (!ParseRecordDatum(out datum))
    //                 return false;
    //             break;
    //
    //         case TOKEN_OPEN_PAREN:
    //             if (!ParseTupleDatum(out datum))
    //                 return false;
    //             break;
    //
    //         case TOKEN_INT_LITERAL:
    //             if (!ParseIntDatum(out int i, out var intRange))
    //                 return false;
    //             else if (intRange is not null)
    //                 datum = intRange;
    //             else
    //                 datum = new IntDatum(i);
    //             break;
    //
    //         case TOKEN_FLOAT_LITERAL:
    //             if (!ParseFloatDatum(out decimal f, out var floatRange))
    //                 return false;
    //             else if (floatRange is not null)
    //                 datum = floatRange;
    //             else
    //                 datum = new FloatDatum(f);
    //             break;
    //
    //         case KEYWORD_TRUE:
    //             Step();
    //             datum = Datum.True;
    //             break;
    //
    //         case KEYWORD_FALSE:
    //             Step();
    //             datum = Datum.False;
    //             break;
    //
    //         case TOKEN_STRING_LITERAL:
    //             var s = _current.StringValue;
    //             Step();
    //             datum = new StringDatum(s);
    //             break;
    //
    //         case TOKEN_EMPTY:
    //             Step();
    //             datum = Datum.Empty;
    //             break;
    //
    //         default:
    //             return Error($"Unexpected token {_current}");
    //     }
    //
    //     return true;
    // }
    //
    // internal bool ParseTupleDatum([NotNullWhen(true)] out Datum? value)
    // {
    //     Step();
    //     value = null;
    //     List<Datum> values = [];
    //     while (_token.Kind is not TOKEN_CLOSE_PAREN)
    //     {
    //         if (!ParseDatum(out value))
    //             return false;
    //         values.Add(value);
    //         if (!Skip(TOKEN_COMMA))
    //             break;
    //     }
    //
    //     if (!Expect(TOKEN_CLOSE_PAREN))
    //         return false;
    //
    //     value = new DatumTuple(values);
    //     return true;
    // }
    //
    // internal bool ParseRecordDatum([NotNullWhen(true)] out Datum? value)
    // {
    //     value = null;
    //
    //     if (!Expect(TOKEN_OPEN_PAREN))
    //         return false;
    //
    //     Dictionary<string, Datum> fields = [];
    //     value = new RecordDatum(fields);
    //
    //     while (_token.Kind is not TOKEN_CLOSE_PAREN)
    //     {
    //         if (!ParseIdent(out var field))
    //             return false;
    //
    //         var fieldName = field.ToString();
    //
    //         if (!Expect(TOKEN_COLON))
    //             return false;
    //
    //         if (!ParseDatum(out var fieldValue))
    //             return false;
    //
    //         if (fields.ContainsKey(fieldName))
    //             return Expected($"a unique value name, got {fieldName}");
    //
    //         fields.Add(fieldName, fieldValue);
    //         if (!Skip(TOKEN_COMMA))
    //             break;
    //     }
    //
    //     if (!Expect(TOKEN_CLOSE_PAREN))
    //         return false;
    //
    //     return true;
    // }

    /// <summary>
    /// Parse an Expression
    /// </summary>
    /// <mzn>a + b + 100</mzn>
    /// <mzn>sum([1,2,3])</mzn>
    /// <mzn>arr[1] * arr[2]</mzn>
    bool ParseExpr(
        [NotNullWhen(true)] out MiniZincExpr? expr,
        Assoc associativity = 0,
        short precedence = 0,
        bool typeInst = false
    )
    {
        expr = null;
        Token token = _token;
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
            case TOKEN_RANGE_INCLUSIVE:
            case TOKEN_RANGE_EXCLUSIVE:
            case TOKEN_RANGE_RIGHT_EXCLUSIVE:
            case TOKEN_RANGE_LEFT_EXCLUSIVE:
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
            token = _token;
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
                is TOKEN_RANGE_INCLUSIVE
                    or TOKEN_RANGE_EXCLUSIVE
                    or TOKEN_RANGE_RIGHT_EXCLUSIVE
                    or TOKEN_RANGE_LEFT_EXCLUSIVE
            )
            {
                int i = _i;
                if (!ParseExpr(out var right))
                {
                    if (_i > i) // Failed while consuming input
                        return false;

                    expr = new RangeExpr(expr.Start, op, lower: expr);
                }
                else
                {
                    expr = new RangeExpr(expr.Start, op, lower: expr, upper: right);
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

    internal static MiniZincExpr UnOpExpr(in Token prefix, MiniZincExpr expr)
    {
        switch (prefix.Kind, expr)
        {
            case (TOKEN_PLUS, IntExpr):
            case (TOKEN_PLUS, FloatExpr):
                return expr;
            case (TOKEN_MINUS, IntExpr { Value: var i }):
                return new IntExpr(expr.Start, -i);
            case (TOKEN_MINUS, FloatExpr { Value: var i }):
                return new FloatExpr(expr.Start, -i);
            case (TOKEN_RANGE_INCLUSIVE, _):
            case (TOKEN_RANGE_EXCLUSIVE, _):
            case (TOKEN_RANGE_RIGHT_EXCLUSIVE, _):
            case (TOKEN_RANGE_LEFT_EXCLUSIVE, _):
                return new RangeExpr(prefix, prefix.Kind, upper: expr);
            default:
                return new UnOpExpr(prefix, expr);
        }
    }

    internal static MiniZincExpr BinOpExpr(MiniZincExpr left, in Token infix, MiniZincExpr right)
    {
        switch (infix.Kind)
        {
            case TOKEN_RANGE_INCLUSIVE:
            case TOKEN_RANGE_EXCLUSIVE:
            case TOKEN_RANGE_RIGHT_EXCLUSIVE:
            case TOKEN_RANGE_LEFT_EXCLUSIVE:

            default:
                return new BinOpExpr(left, infix, right);
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
            case TOKEN_RANGE_EXCLUSIVE:
            case TOKEN_RANGE_INCLUSIVE:
            case TOKEN_RANGE_LEFT_EXCLUSIVE:
            case TOKEN_RANGE_RIGHT_EXCLUSIVE:
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
            case TOKEN_RANGE_EXCLUSIVE:
            case TOKEN_RANGE_INCLUSIVE:
            case TOKEN_RANGE_LEFT_EXCLUSIVE:
            case TOKEN_RANGE_RIGHT_EXCLUSIVE:
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
    internal bool ParseIdentifierExpr([NotNullWhen(true)] out MiniZincExpr? result)
    {
        result = null;
        var name = _token;
        Step();

        // Simple identifier
        if (!Skip(TOKEN_OPEN_PAREN))
        {
            result = new IdentExpr(name);
            return true;
        }

        // Zero-arg function call
        if (Skip(TOKEN_CLOSE_PAREN))
        {
            result = new CallExpr(name);
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
         */

        int pos = _i;

        if (ParseGenerators(out var generators))
        {
            if (!Expect(TOKEN_CLOSE_PAREN))
                return false;

            if (!Skip(TOKEN_OPEN_PAREN))
                goto call;

            if (!ParseExpr(out var expr))
                return false;

            result = new GenCallExpr(name, expr, generators);

            if (!Expect(TOKEN_CLOSE_PAREN))
                return false;

            return true;
        }

        call:
        Backtrack(pos);
        List<MiniZincExpr> exprs = new();

        // First try to parse simple args
        while (_token.Kind is not TOKEN_CLOSE_PAREN)
        {
            if (!ParseExpr(out var expr))
                return false;

            exprs.Add(expr);

            if (!Skip(TOKEN_COMMA))
                break;
        }
        result = new CallExpr(name, exprs);

        if (!Expect(TOKEN_CLOSE_PAREN))
            return false;

        return true;
    }

    private bool ParseGenerators([NotNullWhen(true)] out List<GenExpr>? generators)
    {
        generators = null;
        var start = _token;
        Token id;

        // accumulate generators
        while (true)
        {
            List<Token> ids = new();
            bool underscore = false;

            // accumulate identifiers
            while (true)
            {
                if (Skip(TOKEN_IDENTIFIER, out id))
                {
                    ids.Add(id);
                }
                else if (Skip(TOKEN_UNDERSCORE, out id))
                {
                    underscore = true;
                    ids.Add(id);
                }
                else
                {
                    return Expected("Identifier or underscore");
                }

                if (!Skip(TOKEN_COMMA))
                    break;
            }

            int n = ids.Count;
            GenExpr gen;
            MiniZincExpr? where = null;
            // `x,y,z in 1..3 where x > y`
            if (Skip(KEYWORD_IN))
            {
                if (n > 1 && underscore)
                    return Error(
                        "Wildcard identifier cannot be used inside a generator expression when part of a multi-variable unpacking, eg: `i,j,_ in 1..10`"
                    );

                if (!ParseExpr(out var source))
                    return false;

                if (Skip(KEYWORD_WHERE))
                    if (!ParseExpr(out where))
                        return false;
                gen = new GenYieldExpr(start, ids, source, where);
            }
            // `a=1`
            else if (Skip(TOKEN_EQUAL))
            {
                if (n > 1 || underscore)
                    return Error(
                        "Assignments inside generator expression must take the form `$id$=$var`, eg: `a=1`."
                    );

                if (!ParseExpr(out var source))
                    return false;

                if (Skip(KEYWORD_WHERE))
                    if (!ParseExpr(out where))
                        return false;

                id = ids[0];
                gen = new GenAssignExpr(id, source, where);
            }
            else
            {
                return Expected("IN or EQUALS");
            }

            generators ??= new();
            generators.Add(gen);

            if (!Skip(TOKEN_COMMA))
                break;
        }
        return true;
    }

    /// <summary>
    /// Parse anything that starts with a '[', which will
    /// be an array or array comprehension
    /// </summary>
    /// <mzn>[1,2,3]</mzn>
    /// <mzn>[ x | x in [a,b,c]]</mzn>
    internal bool ParseBracketExpr([NotNullWhen(true)] out MiniZincExpr? result)
    {
        result = null;

        if (!Expect(TOKEN_OPEN_BRACKET, out var start))
            return false;

        // Empty 1d Array
        if (Skip(TOKEN_CLOSE_BRACKET))
        {
            result = new Array1dExpr(start, null);
        }
        // Non-Empty 1d Array
        else if (!Skip(TOKEN_PIPE))
        {
            if (!Parse1dArrayLiteral(start, out result))
                return false;
        }
        // Empty 2d Array
        else if (Skip(TOKEN_PIPE, TOKEN_CLOSE_BRACKET))
        {
            result = new Array2dExpr(start, null, null, 0, 0, false, false);
        }
        // Non-Empty 2d Array
        else if (_token.Kind is not TOKEN_PIPE)
        {
            if (!Parse2dArrayLiteral(start, out var arr2d))
                return false;
            result = arr2d;
        }
        // 3d array
        else
        {
            if (!Parse3dArrayLiteral(start, out var arr3d))
                return false;
            result = arr3d;
        }
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
    bool Parse1dArrayLiteral(in Token start, [NotNullWhen(true)] out MiniZincExpr? result)
    {
        result = null;
        MiniZincExpr index;
        MiniZincExpr element;

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
            element = new IndexedExpr(index, value);
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

            result = new ArrayCompExpr(start, element, generators);
            return Expect(TOKEN_CLOSE_BRACKET);
        }

        // 1D Array literal
        List<MiniZincExpr> elements = [element];
        Array1dExpr arr1d = new(start, elements);
        result = arr1d;

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
                    elements.Add(index);
                    indexed = false;
                    continue;
                }

                if (!ParseExpr(out value))
                    return false;
                element = new IndexedExpr(index, value);
            }
            else if (!ParseExpr(out value))
                return false;
            else
                element = value;

            elements.Add(element);
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
        List<MiniZincExpr>? elements = null;
        List<MiniZincExpr>? indices = null;
        bool rowIndexed = false;
        bool colIndexed = false;
        int j = 0;
        int i = 0;
        int J = 0;
        if (Skip(TOKEN_PIPE))
        {
            if (!Expect(TOKEN_CLOSE_BRACKET))
                goto err;
            else
                goto ok;
        }

        if (!ParseExpr(out var value))
            goto err;
        j++;

        if (!Skip(TOKEN_COLON))
        {
            // If first value is not an index skip the rest of the check
            elements ??= new();
            elements.Add(value);
            Skip(TOKEN_COMMA);
            goto parse_row_values;
        }

        indices ??= new();
        indices.Add(value);

        if (Skip(TOKEN_PIPE))
        {
            colIndexed = true;
            goto parse_row_values;
        }

        colIndexed = true;
        rowIndexed = true;

        while (_token.Kind is not TOKEN_PIPE)
        {
            j++;

            if (!ParseExpr(out value))
                goto err;

            if (Skip(TOKEN_COLON))
            {
                if (!colIndexed)
                {
                    Error("Invalid : in row indexed array literal");
                    goto err;
                }

                rowIndexed = false;
                indices ??= new();
                indices.Add(value);
                continue;
            }

            colIndexed = false;
            elements ??= new();
            elements.Add(value);

            if (!Skip(TOKEN_COMMA))
                break;
        }

        i = 1;
        J = j;

        if (!Expect(TOKEN_PIPE))
            goto err;

        if (Skip(TOKEN_CLOSE_BRACKET))
            goto ok;

        /* Use the second row if necessary to detect dual
         * indexing */
        if (!rowIndexed)
        {
            if (!ParseExpr(out value))
                goto err;

            if (Skip(TOKEN_COLON))
            {
                rowIndexed = true;
                indices ??= new();
                indices.Add(value);
            }
            else
            {
                elements ??= new();
                elements.Add(value);
                Skip(TOKEN_COMMA);
            }

            goto parse_row_values;
        }

        parse_row_index:
        if (!ParseExpr(out value))
            goto err;

        if (!Expect(TOKEN_COLON))
            goto err;

        indices ??= new();
        indices.Add(value);

        parse_row_values:
        i++;
        while (_token.Kind is not TOKEN_PIPE)
        {
            if (!ParseExpr(out value))
                goto err;

            j++;

            elements ??= new();
            elements.Add(value);

            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (J is 0)
            J = j;

        if (!Expect(TOKEN_PIPE))
            goto err;

        // Optional double pipe at the end
        // [|1, 2,|3, 4,||]
        if (Skip(TOKEN_PIPE))
        {
            if (!Expect(TOKEN_CLOSE_BRACKET))
                goto err;
            goto ok;
        }

        if (Skip(TOKEN_CLOSE_BRACKET))
            goto ok;

        j = 0;
        if (rowIndexed)
            goto parse_row_index;
        else
            goto parse_row_values;

        ok:
        arr = new Array2dExpr(start, elements, indices, i, J, rowIndexed, colIndexed);
        return true;

        err:
        arr = new Array2dExpr(start, elements, indices, i, J, rowIndexed, colIndexed);
        return false;
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
        List<MiniZincExpr>? elements = null;
        int i = 0;
        int j = 0;
        int k = 0;

        if (!Expect(TOKEN_PIPE))
            goto err;

        // Check for empty literal
        // `[| | | |]`
        if (Skip(TOKEN_PIPE))
        {
            if (!(Expect(TOKEN_PIPE) && Expect(TOKEN_CLOSE_BRACKET)))
                goto err;

            goto ok;
        }

        i = 0;

        table:
        i++;
        j = 0;

        row:
        j++;
        k = 0;

        while (_token.Kind is not TOKEN_PIPE)
        {
            if (!ParseExpr(out var expr))
                goto err;
            k++;
            elements ??= new();
            elements.Add(expr);
            if (!Skip(TOKEN_COMMA))
                break;
        }

        if (!Expect(TOKEN_PIPE))
            goto err;

        if (Skip(TOKEN_COMMA))
        {
            if (!Expect(TOKEN_PIPE))
                goto err;
            goto table;
        }

        if (!Skip(TOKEN_PIPE))
            goto row;

        if (!Expect(TOKEN_CLOSE_BRACKET))
            goto err;

        ok:
        arr = new Array3dSyntax(start, elements);
        arr.I = i;
        arr.J = j;
        arr.K = k;
        return true;

        err:
        arr = new Array3dSyntax(start, elements);
        arr.I = i;
        arr.J = j;
        arr.K = k;
        return false;
    }

    /// <summary>
    /// Parse anything that starts with a '{', this could be a
    /// set literal or set comprehension
    /// </summary>
    /// <mzn>{1,2,3}</mzn>
    /// <mzn>{ x | x in [1,2,3]}</mzn>
    private bool ParseBraceExpr([NotNullWhen(true)] out MiniZincExpr? result)
    {
        result = null;

        if (!Expect(TOKEN_OPEN_BRACE, out var start))
            return false;

        // Empty Set
        if (Skip(TOKEN_CLOSE_BRACE))
        {
            result = new SetExpr(start, null);
            return true;
        }

        // Parse first expression
        if (!ParseExpr(out var expr))
            return false;

        // Set comprehension
        if (Skip(TOKEN_PIPE))
        {
            if (!ParseGenerators(out var generators))
                return false;

            result = new SetCompExpr(start, expr, generators);
        }
        // Set literal
        else
        {
            var elements = new List<MiniZincExpr>();
            elements.Add(expr);

            while (Skip(TOKEN_COMMA))
            {
                if (_token.Kind is TOKEN_CLOSE_BRACE)
                    break;

                if (!ParseExpr(out expr))
                    return false;

                elements.Add(expr);
            }
            result = new SetExpr(start, elements);
        }

        if (!Expect(TOKEN_CLOSE_BRACE))
            return false;

        return true;
    }

    internal bool ParseLetExpr([NotNullWhen(true)] out MiniZincExpr? let)
    {
        let = null;

        if (!Expect(KEYWORD_LET, out var start))
            return false;

        if (!Expect(TOKEN_OPEN_BRACE))
            return false;

        List<ILetLocalSyntax>? locals = null;

        while (_token.Kind is not TOKEN_CLOSE_BRACE)
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

        let = new LetExpr(start, locals, body);
        return true;
    }

    private bool ParseLetLocal(out ILetLocalSyntax result)
    {
        result = null!;
        if (_token.Kind is KEYWORD_CONSTRAINT)
        {
            if (ParseConstraintStatement(out var con))
            {
                result = (ConstraintItem)con;
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
        [NotNullWhen(true)] out MiniZincExpr? ifCase,
        out MiniZincExpr? thenCase,
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
    private bool ParseIfElseExpr([NotNullWhen(true)] out MiniZincExpr? result)
    {
        result = null;
        var start = _token;

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
    private bool ParseParenExpr([NotNullWhen(true)] out MiniZincExpr? result)
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
            if (expr is not IdentExpr name)
                return Expected("Identifier");

            if (!ParseExpr(out expr))
                return false;

            var fields = new List<(IdentExpr, MiniZincExpr)>();
            var field = (name, expr);
            fields.Add(field);
            result = new RecordExpr(start, fields);

            while (Skip(TOKEN_COMMA))
            {
                if (_token.Kind is TOKEN_CLOSE_PAREN)
                    break;

                if (!ParseIdent(out var token))
                    return false;

                name = new IdentExpr(token);

                if (!Expect(TOKEN_COLON))
                    return false;

                if (!ParseExpr(out expr))
                    return false;

                field = (name, expr);
                fields.Add(field);
            }
        }
        else
        {
            // Else must be a tuple
            var fields = new List<MiniZincExpr>();
            fields.Add(expr);
            result = new TupleExpr(start, fields);

            // 1-Element tuples must have a trailing comma
            if (_token.Kind is not TOKEN_COMMA)
                return Expected("Comma");

            while (Skip(TOKEN_COMMA))
            {
                if (_token.Kind is TOKEN_CLOSE_PAREN)
                    break;

                if (!ParseExpr(out expr))
                    return false;

                fields.Add(expr);
            }
        }

        if (!Expect(TOKEN_CLOSE_PAREN))
            return false;

        return true;
    }

    /// <summary>
    /// Parse annotations
    /// </summary>
    /// <returns>True if no error was encountered</returns>
    internal bool ParseAnnotations(out List<MiniZincExpr>? annotations)
    {
        annotations = null;
        while (Skip(TOKEN_COLON_COLON))
        {
            MiniZincExpr? ann;

            // Edge case where 'output' keyword can be used
            // in a non-keyword context, eg:
            // int : x :: output = 3;
            if (_token.Kind is KEYWORD_OUTPUT)
            {
                ann = new IdentExpr(_token);
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
    internal bool ParseStringAnnotations(out List<MiniZincExpr>? annotations)
    {
        annotations = null;
        while (Skip(TOKEN_COLON_COLON))
        {
            if (!Expect(TOKEN_STRING_LITERAL, out var ann))
                return false;

            annotations ??= [];
            annotations.Add(new StringExpr(ann));
        }

        return true;
    }

    internal bool ParseBaseType([NotNullWhen(true)] out TypeSyntax? type)
    {
        type = null;
        var start = _token;

        if (Skip(KEYWORD_ANY))
        {
            var ident = _token;
            if (ident.Kind is TOKEN_IDENTIFIER_GENERIC or TOKEN_IDENTIFIER_GENERIC_SEQUENCE)
            {
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_IDENT) { Name = ident };
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

            type = new SetTypeSyntax(start, type) { IsVar = var, IsOpt = opt };
            return true;
        }

        start = _token;
        switch (_token.Kind)
        {
            case KEYWORD_INT:
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_INT);
                break;

            case KEYWORD_BOOL:
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_BOOL);
                break;

            case KEYWORD_FLOAT:
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_FLOAT);
                break;

            case KEYWORD_STRING:
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_STRING);
                break;

            case KEYWORD_ANN:
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_ANN);
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
                type = new BaseTypeSyntax(start, TypeKind.TYPE_IDENT) { Name = start };
                break;

            case TOKEN_IDENTIFIER_GENERIC_SEQUENCE:
                Step();
                type = new BaseTypeSyntax(start, TypeKind.TYPE_IDENT) { Name = start };
                break;

            default:
                if (!ParseExpr(out var expr, typeInst: true))
                    return false;
                type = new ExprTypeSyntax(start, expr);
                break;
        }

        type.IsVar = var;
        type.IsOpt = opt;
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

        arr = new ArrayTypeSyntax(start, dims, type) { Kind = TypeKind.TYPE_ARRAY };
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
        while (_token.Kind is not TOKEN_CLOSE_PAREN)
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
        List<MiniZincExpr>? anns;
        TypeSyntax? type;

        parameters = [];
        while (_token.Kind is not TOKEN_CLOSE_PAREN)
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
        var start = _token;
        switch (_token.Kind)
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

        _errorTrace = _sourceText[.._token.End];
        _errorTrace = $"""
            ---------------------------------------------
            {msg}
            ---------------------------------------------
            Token {_token.Kind}
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
    public static ParseResult ParseItemsFromFile(string path, out List<MiniZincItem>? items)
    {
        var watch = Stopwatch.StartNew();
        var mzn = File.ReadAllText(path);
        var parser = new Parser(mzn);
        var ok = parser.ParseItems(out items);
        var elapsed = watch.Elapsed;
        var result = new ParseResult
        {
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
    public static ParseResult ParseItemsFromString(string text, out List<MiniZincItem>? model)
    {
        var watch = Stopwatch.StartNew();
        var parser = new Parser(text);
        var ok = parser.ParseItems(out model);
        var elapsed = watch.Elapsed;
        var result = new ParseResult
        {
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

    /// <inheritdoc cref="ParseItemsFromFile(string,ref System.Collections.Generic.List{MiniZinc.Parser.MiniZincItem}?)"/>
    public static ParseResult ParseItemsFromFile(FileInfo file, out List<MiniZincItem>? items) =>
        ParseItemsFromFile(file.FullName, out items);

    /// <summary>
    /// Parse the given minizinc data file.
    /// Data files only allow assignments eg: `a = 10;`
    /// </summary>
    /// <example>Parser.ParseDataFile("data.dzn")</example>
    public static ParseResult ParseDataFromFile(string path, out MiniZincData data)
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
            FinalToken = parser._token,
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
    // public static bool ParseDatum(string text, [NotNullWhen(true)] out Datum? datum)
    // {
    //     datum = null;
    //     var parser = new Parser(text);
    //     if (!parser.ParseDatum(out datum))
    //         return false;
    //     return true;
    // }

    /// <summary>
    /// Parse a <see cref="Datum"/> from the given string.
    /// </summary>
    /// <example>
    /// Parser.ParseDatum%ltIntArray%gt("[1,2,1,2,3]"); // IntArray([1,2,3]);
    /// </example>
    // public static bool ParseDatum<T>(string text, [NotNullWhen(true)] out T? datum)
    //     where T : Datum
    // {
    //     datum = null;
    //     var parser = new Parser(text);
    //     if (!parser.ParseDatum(out var mzDatum))
    //         return false;
    //     if (mzDatum is not T t)
    //         return false;
    //     datum = t;
    //     return true;
    // }

    /// <summary>
    /// Parse the given minizinc data string.
    /// Data strings only allow assignments eg: `a = 10;`
    /// </summary>
    /// <example>
    /// Parser.ParseDataString("a = 10; b=true;");
    /// </example>
    public static ParseResult ParseDataFromString(string text, out MiniZincData data)
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
            FinalToken = parser._token,
            Elapsed = elapsed,
            ErrorMessage = parser._errorMessage,
            ErrorTrace = parser._errorTrace
        };
        return result;
    }

    /// <inheritdoc cref="ParseDataFromFile(string, out MiniZincData)"/>
    public static ParseResult ParseDataFromFile(FileInfo file, out MiniZincData data) =>
        ParseDataFromFile(file.FullName, out data);

    public static bool ParseModelFromString(string mzn, out MiniZincModel model)
    {
        model = new MiniZincModel();
        model.AddString(mzn);
        return true;
    }

    public static bool ParseModelFromFile(string path, out MiniZincModel model)
    {
        model = new MiniZincModel();
        model.AddFile(path);
        return true;
    }

    public static bool ParseModelFromFile(FileInfo file, out MiniZincModel model) =>
        ParseModelFromFile(file.FullName, out model);

    /// Parse an expression of the given type from text
    public static T ParseExpression<T>(string text)
        where T : MiniZincExpr
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
    public static T ParseItem<T>(string text)
        where T : MiniZincItem
    {
        var parser = new Parser(text);
        if (!parser.ParseItem(out var statement))
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

    /// Try to parse a statement of the given type from text
    public static bool TryParseStatement<T>(string text, [NotNullWhen(true)] out T? result)
        where T : MiniZincItem
    {
        result = null;
        var parser = new Parser(text);
        if (!parser.ParseItem(out var statement))
            return false;

        if (statement is not T t)
            return false;
        result = t;
        return true;
    }

    /// Parse an expression of the given type from text
    public static bool TryParseExpr<T>(
        string text,
        [NotNullWhen(true)] out T? expression,
        out Token location,
        [NotNullWhen(false)] out string? error
    )
        where T : MiniZincExpr
    {
        expression = null;
        if (!TryParseExpr(text, out var expr, out location, out error))
            return false;

        if (expr is not T t)
        {
            error = $"Expected type {typeof(T)} but got {expr.GetType()}";
            return false;
        }

        expression = t;
        return true;
    }

    public static bool TryParseExpr(
        string text,
        [NotNullWhen(true)] out MiniZincExpr? expression,
        out Token location,
        [NotNullWhen(false)] out string? error
    )
    {
        expression = null;
        var parser = new Parser(text);
        if (!parser.ParseExpr(out expression))
        {
            location = parser._token;
            error = parser._errorMessage!;
            return false;
        }

        location = parser._token;
        error = null;
        return true;
    }
}
