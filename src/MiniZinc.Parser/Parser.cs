namespace MiniZinc.Parser;

using System.Diagnostics;
using Ast;

/// <summary>
/// Parses a MiniZinc AST from the given stream of tokens
/// </summary>
public sealed class Parser
{
    private readonly IEnumerator<Token> _tokens;
    private readonly Queue<Token> _cache;
    private readonly Stopwatch _watch;
    private Token _token;
    private TokenKind _kind;
    private int _bufferIndex;
    private int _bufferCount;
    private Token[] _buffer;
    private int _pos;
    public TimeSpan Elapsed => _watch.Elapsed;

    internal Parser(IEnumerator<Token> tokens)
    {
        _watch = Stopwatch.StartNew();
        _tokens = tokens;
        _buffer = new Token[1024];
        _bufferIndex = -1;
        _bufferCount = 0;
        _pos = 0;
    }

    /// Progress the parser
    public bool Read()
    {
        // Read token from the buffer if possible
        if (++_bufferIndex < _bufferCount)
        {
            _token = _buffer[_bufferIndex];
            _kind = _token.Kind;
            _pos++;
            return true;
        }

        // Fill the buffer from the source
        _bufferIndex = 0;
        _bufferCount = 0;

        while (_bufferCount <= _buffer.Length && _tokens.MoveNext())
            _buffer[_bufferCount++] = _tokens.Current;

        _pos++;
        _token = _buffer[0];
        _kind = _token.Kind;
        return _bufferCount > 0;
    }

    /// Progress the parser if the current token is of the given kind
    private bool Skip(TokenKind kind)
    {
        if (_token.Kind != kind)
            return false;
        Read();
        return true;
    }

    /// Skip over the given token kind
    private bool Expect(TokenKind kind)
    {
        if (_token.Kind != kind)
        {
            Error($"Expected a {kind} but encountered a {_token.Kind}");
            return false;
        }

        Read();
        return true;
    }

    bool EndLine() => Expect(TokenKind.EOL);

    public Model? ParseModel()
    {
        var model = new Model();
        while (true)
        {
            switch (_kind)
            {
                case TokenKind.EOF:
                    break;

                case TokenKind.KeywordInclude:
                    if (!ParseIncludeItem(model))
                        return default;
                    break;

                case TokenKind.KeywordSolve:
                    if (!ParseSolveItem(model))
                        return default;
                    break;

                case TokenKind.KeywordOutput:
                    if (!ParseOutputItem(model))
                        return default;
                    break;

                case TokenKind.KeywordFunction:
                case TokenKind.KeywordPredicate:
                    if (!ParseFunctionDeclare(model))
                        return default;
                    break;

                case TokenKind.KeywordEnum:
                    if (!ParseEnumDeclare(model.NameSpace))
                        return default;
                    break;

                case TokenKind.KeywordType:
                    ParseAliasItem(model);
                    break;

                case TokenKind.BlockComment:
                case TokenKind.LineComment:
                    break;

                case TokenKind.Identifier:
                    ParseVariableAssign(model.NameSpace);
                    break;

                case TokenKind.KeywordVar:
                case TokenKind.KeywordPar:
                case TokenKind.KeywordArray:
                case TokenKind.KeywordSet:
                case TokenKind.KeywordOpt:
                    ParseVariableAssign(model.NameSpace);
                    break;

                case TokenKind.Identifier:
                    break;

                default:
                    break;
            }
        }
    }

    private bool ParseEnumDeclare(NameSpace<IExpr> ns)
    {
        if (!Expect(TokenKind.KeywordEnum))
            return false;

        if (!ReadName(out var name))
            return false;

        var item = new EnumDeclare { Name = name };
        if (!ParseAnnotations(item))
            return false;

        if (_token.Kind is TokenKind.EOL)
            goto end;

        if (!Expect(TokenKind.Equal))
            return false;

        enumcases:
        EnumCase enumCase;
        switch (_token.Kind)
        {
            // {A, B, C}
            case TokenKind.OpenBrace:
                Read();
                while (_token.Kind is not TokenKind.CloseBrace)
                {
                    enumCase = new EnumCase { Name = ReadString(), Type = EnumCaseType.Name };
                    item.Cases.Add(enumCase);
                    if (!Skip(TokenKind.Comma))
                        break;
                }
                Expect(TokenKind.CloseBrace);
                break;

            // enum A = anon_enum(100);
            // enum B = _(1 .. 10);
            case TokenKind.Underscore:
            case TokenKind.KeywordAnonEnum:
                Read();
                Expect(TokenKind.OpenParen);
                enumCase = new EnumCase { Type = EnumCaseType.Anonymous, Expr = ParseExpr() };
                item.Cases.Add(enumCase);
                Expect(TokenKind.CloseParen);
                break;

            // C({A,B})
            default:
                enumCase = new EnumCase { Type = EnumCaseType.Complex, Expr = ParseExpr() };
                item.Cases.Add(enumCase);
                break;
        }
        if (Skip(TokenKind.PlusPlus))
            goto enumcases;

        end:
        EndLine();
        ns.Push(name, item);
    }

    public bool ParseOutputItem(Model model)
    {
        Expect(TokenKind.KeywordOutput);
        var expr = ParseExpr();
        var item = new OutputItem { Expr = expr };
        EndLine();
        model.Outputs.Add(item);
    }

    public void ParseAliasItem(Model model)
    {
        Expect(TokenKind.KeywordType);
        var name = ReadString();
        Expect(TokenKind.Equal);
        var type = ParseTypeInst();
        EndLine();
        model.NameSpace.Push(name, type);
    }

    public void ParseVariableAssign(NameSpace<IExpr> ns)
    {
        var name = ReadString();
        Expect(TokenKind.Equal);
        var value = ParseExpr();
        EndLine();
        ns.Push(name, value);
    }

    public bool ParseFunctionDeclare(Model model)
    {
        var func = new FunctionDeclare();

        if (Skip(TokenKind.KeywordPredicate))
            func.ReturnType = new TypeInst { Kind = TypeKind.Bool, Flags = TypeFlags.Var };
        else
        {
            Skip(TokenKind.KeywordFunction);
            func.ReturnType = ParseTypeInst();
        }
        Expect(TokenKind.Colon);
        func.Name = ReadString(TokenKind.Identifier);
        ParseParameters(func.Parameters);
        ns.Push(func.Name, func);
    }

    private bool ReadString(out string result, TokenKind kind = TokenKind.StringLiteral)
    {
        if (_kind == kind && _token.String is { } s)
        {
            Read();
            result = s;
            return true;
        }

        result = string.Empty;
        return false;
    }

    private bool ReadName(out string name) => ReadString(out name, TokenKind.Identifier);

    private bool ParseParameters(List<Binding<TypeInst>>? list)
    {
        Expect(TokenKind.OpenParen);
        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        next:
        var ti = ParseTypeInstAndId();
        list ??= new List<Binding<TypeInst>>();
        list.Add(ti);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        Expect(TokenKind.CloseParen);
    }

    public void ParseArguments(List<IExpr>? list)
    {
        Expect(TokenKind.OpenParen);
        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        next:
        var expr = ParseExpr();
        list ??= new List<IExpr>();
        list.Add(expr);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        Expect(TokenKind.CloseParen);
    }

    public bool ParseTypeInst(out TypeInst ti)
    {
        switch (_token.Kind)
        {
            case TokenKind.KeywordArray:
            case TokenKind.KeywordList:
                return ParseArrayTypeInst(ti);
            default:
                return ParseBaseTypeInst(ti);
        }

        return type;
    }

    public bool ParseBaseTypeInst(out TypeInst ti)
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

        ti = ParseBaseTypeInstTail();
        ti.Flags = inst;
        return true;
    }

    public bool ParseArrayTypeInst(out TypeInst ti)
    {
        var type = new ArrayType();
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

    private bool ParseBaseTypeInstTail(out TypeInst ti)
    {
        switch (_token.Kind)
        {
            case TokenKind.KeywordInt:
                Read();
                ti = new TypeInst { Kind = TypeKind.Int };
                break;

            case TokenKind.KeywordBool:
                Read();
                ti = new TypeInst { Kind = TypeKind.Bool };
                break;

            case TokenKind.KeywordFloat:
                Read();
                ti = new TypeInst { Kind = TypeKind.Float };
                break;

            case TokenKind.KeywordString:
                Read();
                ti = new TypeInst { Kind = TypeKind.String };
                break;

            case TokenKind.KeywordAnn:
                Read();
                ti = new TypeInst { Kind = TypeKind.Annotation };
                break;

            case TokenKind.KeywordRecord:
                ti = ParseRecordType();
                break;

            case TokenKind.KeywordTuple:
                ti = ParseTupleType();
                break;

            default:
                var expr = ParseExpr();
                ti = new ExprType { Expr = expr };
                break;
        }

        return true;
    }

    private bool ParseTupleType(out TupleTypeInst tuple)
    {
        Expect(TokenKind.KeywordTuple);
        Expect(TokenKind.OpenParen);
        tuple = new TupleTypeInst();
        next:
        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        if (!ParseTypeInst(out var ti))
            return false;

        tuple.Items.Add(ti);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseBracket);
    }

    private bool ParseRecordType(out RecordTypeInst record)
    {
        Expect(TokenKind.KeywordRecord);
        record = new RecordTypeInst();
        next:
        if (_token.Kind is TokenKind.CloseParen)
            goto end;

        if (!ParseTypeInstAndId(out var ti))
            return false;

        record.Fields.Add(ti);
        if (Skip(TokenKind.Comma))
            goto next;

        end:
        return Expect(TokenKind.CloseBracket);
    }

    private bool At(TokenKind kind) => _kind == kind;

    public bool ParseVariableDeclare(out VariableDeclareItem item)
    {
        item = new VariableDeclareItem();

        if (!ReadName(out var name))
            return false;

        item.Name = name;

        if (!Expect(TokenKind.Colon))
            return false;

        if (!ParseTypeInst(out var type))
            return false;

        item.Type = type;

        if (Skip(TokenKind.Equal))
            if (!ParseExpr(out item.Value))
                return false;

        return EndLine();
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

    public bool ParseIncludeItem(Model model)
    {
        if (!Expect(TokenKind.KeywordInclude))
            return false;

        if (!ReadString(out var path))
            return false;

        model.Includes.Add(path);
        return EndLine();
    }

    public bool ParseSolveItem(Model model)
    {
        if (!Expect(TokenKind.KeywordSolve))
            return false;

        var item = new SolveItem();
        switch (_token.Kind)
        {
            case TokenKind.KeywordSatisfy:
                Read();
                item.Method = SolveMethod.Satisfy;
                break;
            case TokenKind.KeywordMinimize:
                Read();
                item.Method = SolveMethod.Minimize;
                if (!ParseExpr(out item.Objective))
                    return false;
                break;
            case TokenKind.KeywordMaximize:
                Read();
                item.Method = SolveMethod.Maximize;
                if (!ParseExpr(out item.Objective))
                    return false;
                break;
            default:
                return Error("Expected satisfy, minimize, or maximize");
        }
        model.SolveItems.Add(item);
        return EndLine();
    }

    public bool ParseConstraintItem(out ConstraintItem constraint)
    {
        constraint = new ConstraintItem();
        if (!Expect(TokenKind.KeywordConstraint))
            return false;

        if (!ParseExpr(out constraint.Expr))
            return false;

        return true;
    }

    public bool ParseExpr(out IExpr expr)
    {
        expr = null;

        switch (_token.Kind)
        {
            case TokenKind.Plus:
                Read();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.UnOp(Operator.Plus, expr);
                break;

            case TokenKind.Minus:
                Read();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.UnOp(Operator.Minus, expr);
                break;

            case TokenKind.KeywordNot:
                Read();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.UnOp(Operator.Not, expr);
                break;

            case TokenKind.DotDot:
                Read();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.Range(upper: expr);
                break;

            case TokenKind.Underscore:
                Read();
                expr = new WildCardExpr();
                break;

            case TokenKind.KeywordTrue:
                Read();
                expr = Expr.Bool(true);
                break;

            case TokenKind.KeywordFalse:
                Read();
                expr = Expr.Bool(false);
                break;

            case TokenKind.IntLiteral:
                Read();
                expr = Expr.Int(_token.Int);
                break;

            case TokenKind.FloatLiteral:
                Read();
                expr = Expr.Float(_token.Double);
                break;

            case TokenKind.StringLiteral:
                Read();
                expr = Expr.String(_token.String!);
                break;

            case TokenKind.OpenParen:
                if (!ParseTupleLikeExpr(out var let))
                    return false;
                expr = let;
                break;

            case TokenKind.OpenBrace:
                if (!ParseSetLikeExpr(out var set))
                    return false;
                expr = set;
                break;

            case TokenKind.OpenBracket:
                if (!ParseArrayLikeExpr(out var arr))
                    return false;
                expr = arr;
                break;

            case TokenKind.KeywordIf:
                if (!ParseIfElseExpr(out expr))
                    return false;
                expr = XD;
                break;

            case TokenKind.KeywordLet:
                if (!ParseLetExpr(out expr))
                    return false;
                expr = XD;
                break;

            case TokenKind.Identifier:
                expr = ParseCallLikeExpr();
                break;

            default:
                throw new Exception();
        }

        return true;
    }

    private IExpr ParseCallLikeExpr()
    {
        throw new NotImplementedException();
    }

    private bool ParseLetExpr(out LetExpr let)
    {
        let = new LetExpr();

        if (!Expect(TokenKind.KeywordLet))
            return false;

        if (!Expect(TokenKind.OpenBrace))
            return false;

        while (_token.Kind is not TokenKind.CloseBrace)
        {
            if (!ParseTypeInst(out var type))
                return false;

            let.Declares.Add(type);
            if (!Skip(TokenKind.EOL))
                break;
        }

        if (!Expect(TokenKind.KeywordIn))
            return false;

        if (!ParseExpr(out let.Body))
            return false;

        return true;
    }

    private bool ParseIfElseExpr(out IfThenElseExpr ite)
    {
        ite = new IfThenElseExpr();
        return false;
    }

    /// <summary>
    /// Parse anything starting with a '(' eg:
    /// - (1)
    /// - (2,)
    /// - (a: 100, b:200)
    /// </summary>
    /// <returns></returns>
    private bool ParseTupleLikeExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Expect(TokenKind.OpenParen))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        IExpr name;

        // Bracketed expr
        if (Skip(TokenKind.CloseParen))
            return true;

        // Record expr
        if (Skip(TokenKind.Colon))
        {
            var record = new RecordExpr();
            result = record;
            name = expr;
            if (!ParseExpr(out expr))
                return false;

            record.Fields.Add((name, expr));

            record_field:
            if (!Skip(TokenKind.Comma))
                return Expect(TokenKind.CloseParen);

            if (!ParseExpr(out name))
                return false;

            if (!Expect(TokenKind.Colon))
                return false;

            if (!ParseExpr(out expr))
                return false;

            record.Fields.Add((name, expr));
            goto record_field;
        }

        // Else must be a tuple
        var tuple = new TupleExpr();
        result = tuple;
        tuple.Exprs.Add(expr);
        if (!Expect(TokenKind.Comma))
            return false;

        do
        {
            if (!ParseExpr(out expr))
                return false;
            tuple.Exprs.Add(expr);
        } while (Skip(TokenKind.Comma));

        return Expect(TokenKind.CloseParen);
    }

    public bool ParseAnnotations(IAnnotations target)
    {
        while (_token.Kind is TokenKind.DoubleColon)
        {
            if (!ParseAnnotation(out var ann))
                return false;

            target.Annotate(ann);
        }

        return true;
    }

    public bool ParseAnnotation(out IExpr expr) => ParseExpr(out expr);

    /// Throw an error
    private bool Error(string msg)
    {
        _watch.Stop();
        var exn =
            $@"""w
            An error occurred at Token {_kind} (Line {_token.Line} Col {_token.Col}):
                {msg}
            """;
        return false;
    }

    //
    // addPrefix "+"   2000 (fun expr -> Expr.UnaryOp (UnOp.Plus, expr))
    // addPrefix "-"   2000 (fun expr -> Expr.UnaryOp (UnOp.Minus, expr))
    // addPrefix "not" 2000 (fun expr -> Expr.UnaryOp (UnOp.Not, expr))
    // addPrefix ".."  2000 (fun expr -> Expr.RightOpenRange expr)
    //
    // let right_open_range =
    //
    //     let after : Parser<string> =
    //         pstring "."
    //         .>> ws
    //
    //     let op =
    //         PostfixOperator(".", after, 1, false, Expr.RightOpenRange)
    //
    //     opp.AddOperator op
    //
    // // <annotations>
    // // eg: `:: output :: x(2)`
    // let annotations : Parser<Annotations> =
    //     many (annotation .>> ws)
    //
    // // <annotations>
    // // eg: `:: output :: x(2)`
    // let ann_capture : Parser<Ident> =
    //     skip "ann"
    //     >>. skip ':'
    //     >>. ident
    //
    // // A TypeInst with a name
    // // <ti-expr-and-id>
    // // eg: `var int: x`
    // let ti_expr_and_id : Parser<TypeInst> =
    //     fun stream ->
    //         let reply = ti_expr stream
    //         if reply.Status <> Ok then
    //             Reply(reply.Status, reply.Error)
    //         else
    //             stream.SkipWhitespace()
    //             stream.SkipWs(':')
    //             match ident stream with
    //             | r when r.Status = Ok ->
    //                 let ti = {reply.Result with Name = r.Result}
    //                 Reply(ti)
    //             | r ->
    //                 Reply(r.Status, r.Error)
    //
    // // A TypeInst with an optional name
    // // eg: function(int, a:bool)
    // let parameter_ti : Parser<TypeInst> =
    //     pipe(
    //         ti_expr,
    //         opt_or "" (skip ':' >>. ident),
    //         annotations,
    //         fun ti id anns ->
    //             { ti with
    //                 Name = id
    //                 Annotations = anns }
    //     )
    //
    // let assign_tail : Parser<Expr> =
    //     let error = expected "=[=] expr"
    //     fun stream ->
    //         if stream.Skip('=') then
    //             stream.Skip('=')
    //             stream.SkipWhitespace()
    //             expr stream
    //         else
    //             Reply(ReplyStatus.Error, error)
    //
    // // eg: `(int: a, var bool: b)`
    // let parameters : Parser<TypeInst list> =
    //     parameter_ti
    //     |> commaSep
    //     |> betweenWs('(', ')')
    //     <!> "parameters"
    //
    // // eg: `(1, 2, "abc")`
    // let tupled_args : Parser<Expr list> =
    //     expr
    //     |> commaSep
    //     |> betweenWs('(', ')')
    //     <!> "tupled-args"
    //
    // // <predicate-item>
    // // eg: `predicate isOk(int x) = x > 2`
    // let predicate_item : Parser<Item> =
    //     pipe(
    //         ident,
    //         parameters,
    //         opt_or "" ann_capture,
    //         annotations,
    //         opt assign_tail,
    //         fun id pars ann anns body ->
    //             { Name = id
    //             ; Parameters = pars
    //             ; Ann = ann
    //             ; Annotations = anns
    //             ; Returns =
    //                 { TypeInst.Empty
    //                     with
    //                         Type = Type.Bool
    //                         IsVar = true  }
    //             ; Body = body }
    //             |> Item.Function
    //             )
    //     <!> "predicate"
    //
    // // <test_item>
    // let test_item : Parser<Item> =
    //     pipe(
    //         ident,
    //         parameters,
    //         annotations,
    //         opt assign_tail,
    //         fun id pars anns body ->
    //             { Name = id
    //             ; Parameters = pars
    //             ; Annotations = anns
    //             ; Body = body }
    //             |> Item.Test
    //             )
    //     <!> "test"
    //
    // // <function-item>
    // (*
    // TODO - combine this with variable parsing because the optional function keyword is making it hard
    // *)
    // let function_item : Parser<Item> =
    //     pipe(
    //         ti_expr_and_id,
    //         parameters,
    //         opt_or "" ann_capture,
    //         annotations,
    //         opt assign_tail,
    //         (fun ti pars ann anns body ->
    //             { Name = ti.Name
    //             ; Returns = { ti with Name = ""}
    //             ; Ann = ann
    //             ; Annotations = anns
    //             ; Parameters = pars
    //             ; Body = body }
    //             |> Item.Function
    //             )
    //     )
    //     <!> "function"
    //
    // // <enum-item>
    // let enum_item : Parser<Item> =
    //
    //     // <enum-case-list>
    //     let enum_cases : Parser<EnumCases list> =
    //
    //         let enum_names : Parser<EnumCases> =
    //             ident
    //             |> commaSep
    //             |> betweenWs('{', '}')
    //             |>> EnumCases.Names
    //
    //         let anon_enum : Parser<EnumCases> =
    //             skip "_"
    //             <|> skip "anon_enum"
    //             >>. betweenWs('(', ')') expr
    //             |>> EnumCases.Anon
    //
    //         let enum_call : Parser<EnumCases> =
    //             ident
    //             .>>. betweenWs('(', ')') expr
    //             |>> EnumCases.Call
    //
    //         let enum_case =
    //             [ enum_names; anon_enum; enum_call ]
    //             |> choice
    //             .>> ws
    //
    //         sepBy enum_case (skip "++")
    //
    //     pipe(
    //         ident,
    //         annotations,
    //         (opt_or [] (skip '=' >>. enum_cases)),
    //         fun name anns cases ->
    //             { Name = name
    //             ; Annotations = anns
    //             ; Cases = cases }
    //             |> Item.Enum)
    //
    // // <base-ti-expr>
    // let base_ti_expr_atom : Parser<TypeInst> =
    //
    //     let is_var =
    //         choice
    //             [ Keyword.VAR ==> true
    //             ; Keyword.PAR ==> false
    //             ; preturn false ]
    //
    //     let is_opt =
    //         Keyword.OPT ==> true
    //         <|> preturn false
    //
    //     let is_set =
    //         skip1 Keyword.SET
    //         >>. skip Keyword.OF
    //         >>% true
    //         <|> preturn false
    //
    //     pipe(
    //         is_var,
    //         is_set,
    //         is_opt,
    //         base_ti_expr_tail,
    //         fun inst set opt typ ->
    //             { Type = typ
    //             ; Name = ""
    //             ; IsOptional = opt
    //             ; IsSet = set
    //             ; IsArray = false
    //             ; Annotations = []
    //             ; PostFix = []
    //             ; IsVar = inst
    //             ; Value = None
    //             ; IsInstanced = false }
    //     )
    //     <!> "base-ti"
    //
    // let generic_ident =
    //     let options = IdentifierOptions(isAsciiIdStart = fun c -> c = '$' || Char.IsLetter c )
    //     skipChar '$'
    //     >>. identifier options
    //
    // let base_ti_expr : Parser<TypeInst> =
    //
    //     let any_ti : Parser<TypeInst> =
    //         let error = expected "ti-variable: eg $$x"
    //         skip1 "any"
    //         >>. generic_ident
    //         |>> (fun name -> { TypeInst.Empty with Type = Type.Generic name  })
    //
    //     // Combine the given TypeInsts
    //     let foldTypeInsts (a: TypeInst) _ (b: TypeInst) =
    //
    //         let ty =
    //             match a.Type, b.Type with
    //
    //             | Type.Record ra, Type.Record rb when a.IsSingleton && b.IsSingleton ->
    //                 Type.Record (ra @ rb)
    //
    //             | Type.Tuple ta, Type.Tuple tb when a.IsSingleton && b.IsSingleton ->
    //                 Type.Tuple (ta @ tb)
    //
    //             // TODO - handle somehow
    //             | Type.Concat xs, _ ->
    //                 Type.Concat (xs @ [b])
    //
    //             | _, _ ->
    //                 Type.Concat [a; b]
    //
    //         let ti = { a with Type = ty }
    //         let ti = resolveTypeInst ti
    //         ti
    //
    //     any_ti
    //     <|>
    //     Inline.SepBy(
    //       stateFromFirstElement = id,
    //       foldState = foldTypeInsts,
    //       resultFromState = id,
    //       elementParser = (base_ti_expr_atom .>> ws),
    //       separatorParser = skip "++",
    //       separatorMayEndSequence = true
    //     )
    //
    // // <array-ti-expr>
    // let array_ti_expr : Parser<TypeInst> =
    //
    //     let list_ti : Parser<TypeInst> =
    //         fun stream ->
    //             let error = expected "list-ti"
    //             if stream.Skip("list ") then
    //                 stream.SkipWhitespace()
    //                 if stream.Skip("of") then
    //                     stream.SkipWhitespace()
    //                     base_ti_expr stream
    //                 else
    //                     Reply(ReplyStatus.Error, error)
    //             else
    //                 Reply(ReplyStatus.Error, error)
    //
    //     let array_dim_expr =
    //         ti_expr |>> (fun ti -> ti.Type)
    //
    //     let array_dims =
    //         array_dim_expr
    //         |> commaSep
    //         |> betweenWs ('[', ']')
    //         <!> "array-dims"
    //
    //     let array_ti =
    //         pipe(
    //             skip Keyword.ARRAY,
    //             array_dims,
    //             skip Keyword.OF,
    //             base_ti_expr,
    //             fun _ dims _ ti ->
    //                 (dims, resolveTypeInst ti))
    //         >>= (fun (dims, ti) ->
    //                 let arr ty =
    //                     preturn
    //                         { TypeInst.Empty with
    //                             IsArray = true
    //                             IsVar = ti.IsVar
    //                             Type = ty }
    //
    //                 match dims with
    //
    //                 | [i] ->
    //                     arr <| Type.Array1D (i, ti)
    //                 | [i;j] ->
    //                     arr <| Type.Array2D (i, j, ti)
    //                 | [i;j;k]->
    //                     arr <| Type.Array3D (i, j, k, ti)
    //                 | [i;j;k;l]->
    //                     arr <| Type.Array4D (i, j, k, l, ti)
    //                 | [i;j;k;l;m] ->
    //                     arr <| Type.Array5D (i, j, k, l, m, ti)
    //                 | [i;j;k;l;m;n] ->
    //                     arr <| Type.Array6D (i, j, k, l, m, n, ti)
    //                 | xs ->
    //                     fail $"Number of array dimension must be between 1 and 6 (got {xs.Length})."
    //         )
    //     list_ti
    //     <|> array_ti
    //     <!> "array-ti"
    //
    // // <ti-expr>
    // ti_expr_ref.contents <-
    //     [ array_ti_expr
    //     ; base_ti_expr  ]
    //     |> choice
    //     |> withContext Context.TypeInst
    //     <!> "ti"
    //
    // // <tuple-ti-expr-tail>
    // let tuple_ti : Parser<Type> =
    //     ti_expr
    //     |> commaSep1
    //     |> betweenWs ('(', ')')
    //     |>> Type.Tuple
    //     <!> "tuple-ti"
    //
    // // <record-ti-expr-tail>
    // let record_ti : Parser<Type> =
    //     ti_expr_and_id
    //     |> commaSep1
    //     |> betweenWs('(', ')')
    //     |>> Type.Record
    //     <!> "record-ti"
    //
    // let left_open_range =
    //     skip ".."
    //     >>. expr
    //     |>> Expr.LeftOpenRange
    //
    // // <base-ti-expr-tail>
    // base_ti_expr_tail_ref.contents <-
    //
    //     // TypeInst declared with an expression
    //     // eg: ```1..(1+3)```
    //     let expr_ti : Parser<Type> =
    //         fun stream ->
    //             let reply = expr stream
    //             if reply.Status = Ok then
    //                 Reply(Type.Expr reply.Result)
    //             else
    //                 Reply(reply.Status, reply.Error)
    //
    //     let generic =
    //         generic_ident
    //         |>> Type.Generic
    //
    //     fun stream ->
    //         let stateTag = stream.StateTag
    //         let context = stream.UserState.Context
    //
    //         let reply = ident_kw stream
    //         let struct(id, keyword) = reply.Result
    //
    //         if reply.Status = Ok then
    //             match keyword with
    //             | Keyword.INT ->
    //                 Reply(Type.Int)
    //             | Keyword.BOOL ->
    //                 Reply(Type.Bool)
    //             | Keyword.FLOAT ->
    //                 Reply(Type.Float)
    //             | Keyword.STRING ->
    //                 Reply(Type.String)
    //             | Keyword.ANN ->
    //                 Reply(Type.Ann)
    //             | Keyword.ANNOTATION ->
    //                 Reply(Type.Annotation)
    //             | Keyword.RECORD ->
    //                 record_ti stream
    //             | Keyword.TUPLE ->
    //                 tuple_ti stream
    //             | _ ->
    //                 // Must be an expression (eg: `Foo`, `{1,2,3}`)
    //                 stream.Seek(stream.Index - (int64)id.Length)
    //                 expr_ti stream
    //
    //         elif reply.Status = Error && stateTag = stream.StateTag then
    //           match stream.Peek() with
    //           | '$' -> generic stream
    //           | _ -> expr_ti stream
    //
    //         else
    //           Reply(reply.Status, reply.Error)
    //
    //     //
    //     // |>> (fun (name, args) ->
    //     //     // Handle some special known cases of function application
    //     //     match name, args with
    //     //     | (Id "array1d"), [i; Expr.Array1DLit arr] ->
    //     //         Expr.Array1D (i, arr)
    //     //     | (Id "array2d"), [i; j; Expr.Array1DLit arr] ->
    //     //         Expr.Array2D (i, j, arr)
    //     //     | (Id "array3d"), [i; j; k; Expr.Array1DLit arr] ->
    //     //         Expr.Array3D (i, j, k, arr)
    //     //     | (Id "array4d"), [i; j; k; l; Expr.Array1DLit arr] ->
    //     //         Expr.Array4D (i, j, k, l, arr)
    //     //     | (Id "array5d"), [i; j; k; l; m; Expr.Array1DLit arr] ->
    //     //         Expr.Array5D (i, j, k, l, m, arr)
    //     //     | (Id "array6d"), [i; j; k;l;m;n; Expr.Array1DLit arr] ->
    //     //         Expr.Array6D (i, j, k, l, m, n, arr)
    //     //     | _, _ ->
    //     //         Expr.Call(name,args)
    //     // )
    //     // <!> "call-expr"
    //
    // let wildcard : Parser<WildCard> =
    //     skipChar '_'
    //     >>. notFollowedBy letter
    //     >>% WildCard.WildCard
    //
    // let absent : Parser<Absent> =
    //     attempt (skip "<>")
    //     >>% Absent
    //
    // /// Generators
    // /// Used in generator calls and list or set comprehensions
    // /// eg: ```sum(i in 1..3 where i < 2)(i)```
    // let generators : Parser<Generator list> =
    //
    //     let gen_var =
    //         [ wildcard |>> IdentOr.Other
    //         ; ident    |>> IdentOr.Ident ]
    //         |> choice
    //         <!> "gen-var"
    //
    //     let gen_vars =
    //         gen_var
    //         |> commaSep1
    //         <!> "gen-vars"
    //
    //     let gen_where =
    //         skip "where "
    //         >>. expr
    //         <!> "gen-where"
    //
    //     let generator =
    //         pipe(
    //             gen_vars,
    //             skip Keyword.IN,
    //             expr,
    //             opt gen_where,
    //             (fun idents _ source filter ->
    //                 { Yields = idents
    //                 ; From = source
    //                 ; Where = filter })
    //         )
    //         <!> "generator"
    //
    //     generator
    //     |> commaSep1
    //
    // // <declare-item>
    // let declare_item : Parser<Item> =
    //
    //     let declare_any : Parser<Item> =
    //         skip "any"
    //         >>. skip ':'
    //         >>. pipe(
    //             ident,
    //             annotations,
    //             assign_tail,
    //             fun name anns expr ->
    //                 Item.Declare
    //                  { TypeInst.Empty with
    //                     Type = Type.Any
    //                     Name = name
    //                     Annotations = anns
    //                     Value = Some expr }
    //             )
    //         <!> "declare-any"
    //
    //     declare_any
    //     <|>
    //     pipe(
    //         ti_expr_and_id,
    //         opt parameters,
    //         annotations,
    //         opt assign_tail,
    //         annotations,
    //         fun ti args ti_anns expr expr_anns ->
    //             let ti = resolveTypeInst ti
    //             match args with
    //             // No arguments, not a function
    //             | None ->
    //                 Item.Declare
    //                     { ti with
    //                         Annotations = ti_anns
    //                         PostFix = expr_anns
    //                         Value=expr }
    //             // Arguments, must be a function
    //             | Some args ->
    //                 Item.Function
    //                     { Name = ti.Name
    //                     ; Returns = {ti with Name = ""}
    //                     ; Ann = ""
    //                     ; Annotations = ti_anns
    //                     ; Parameters = args
    //                     ; Body = expr}
    //     )
    //     <!> "declare-item"
    //
    // let constraint_tail : Parser<ConstraintExpr> =
    //     pipe(
    //         expr,
    //         annotations,
    //         fun expr anns ->
    //           { Expr = expr
    //           ; Annotations = anns }
    //     )
    //
    // // <constraint-item>
    // let constraint_item =
    //     constraint_tail |>> Item.Constraint
    //
    // // <let-item>
    // let let_item : Parser<LetLocal> =
    //
    //     let let_declare =
    //         declare_item
    //         >>= function
    //           | Item.Declare x ->
    //               preturn (Choice1Of2 x)
    //           | other ->
    //               fail $"{other} not allowed in let locals"
    //
    //     let let_constraint =
    //         skip Keyword.CONSTRAINT
    //         >>. constraint_tail
    //         |>> Choice2Of2
    //
    //     // Let constraint must go first
    //     let_constraint <|> let_declare
    //
    // // <let-expr>
    // let let_expr_tail : Parser<LetExpr> =
    //
    //     let item =
    //         let_item .>>> opt (anyOf ";,")
    //
    //     let items =
    //         many1 (item .>> ws)
    //         |> betweenWs('{', '}')
    //
    //     ws >>. pipe(items, skip "in", expr,
    //         fun items _b body ->
    //             let declares, constraints =
    //                 items
    //                 |> List.fold (fun (ds, cs) item ->
    //                     match item with
    //                     | Choice1Of2 x -> (x::ds, cs)
    //                     | Choice2Of2 x -> (ds, x::cs)
    //                 ) ([], [])
    //
    //             // let nameSpace : NameSpace =
    //             //     (NameSpace.empty, declares)
    //             //     ||> List.fold (fun ns decl -> ns.Add decl)
    //             //
    //             { Declares = declares
    //             ; Constraints = constraints
    //             ; Body=body }
    //             )
    //
    //     |> withContext Context.LetExpr
    //     <!> "let-expr"
    //
    // // <if-then-else-expr>
    // let if_else_expr_tail : Parser<IfThenElseExpr> =
    //
    //     let else_case =
    //         skip Keyword.ELSE
    //         >>. expr
    //
    //     let elseif_case =
    //         skip Keyword.ELSEIF
    //         >>. expr
    //
    //     let then_case =
    //         skip Keyword.THEN
    //         >>. expr
    //
    //     let elseif_then_case =
    //         tuple(elseif_case, then_case)
    //
    //     let elseif_cases =
    //         many elseif_then_case
    //
    //     fun stream ->
    //
    //         let mutable ite =
    //             { If = Unchecked.defaultof<Expr>
    //               Then = Unchecked.defaultof<Expr>
    //               ElseIf = []
    //               Else = None }
    //
    //         stream.SkipWhitespace()
    //         let mutable reply =
    //             Reply(ReplyStatus.Error, ite, NoErrorMessages)
    //
    //         // If-Case
    //         let ifReply = expr stream
    //         if ifReply.Status = Ok then
    //             stream.SkipWhitespace()
    //             ite <- { ite with If = ifReply.Result }
    //
    //             // Then-case
    //             let thenReply = then_case stream
    //             if thenReply.Status = Ok then
    //                 stream.SkipWhitespace()
    //                 ite <- { ite with Then = thenReply.Result }
    //                 let stateTag = stream.StateTag
    //
    //                 // ElseIf-Cases
    //                 let elseIfReply = elseif_cases stream
    //                 if (elseIfReply.Status = Ok || stateTag = stream.StateTag) then
    //                     if elseIfReply.Status = Ok then
    //                         stream.SkipWhitespace()
    //                         ite <- { ite with ElseIf =  elseIfReply.Result }
    //                     let stateTag = stream.StateTag
    //
    //                     // Else-Case
    //                     let elseReply = else_case stream
    //                     if (elseReply.Status = Ok || stream.StateTag = stateTag) then
    //                         if elseReply.Status = Ok then
    //                             stream.SkipWhitespace()
    //                             ite <- {ite with Else = Some elseReply.Result }
    //                         if stream.Skip("endif") then
    //                             reply.Status <- Ok
    //                             reply.Result <- ite
    //                         else
    //                             reply.Status <- ReplyStatus.Error
    //                             reply.Error <- expected "endif"
    //                     else
    //                         reply.Status <- elseIfReply.Status
    //                         reply.Error <- elseIfReply.Error
    //                 else
    //                     reply.Status <- elseIfReply.Status
    //                     reply.Error <- elseIfReply.Error
    //             else
    //                 reply.Status <- thenReply.Status
    //                 reply.Error <- thenReply.Error
    //         else
    //             reply.Status <- ifReply.Status
    //             reply.Error <- ifReply.Error
    //
    //         reply
    //
    // let tuple_access_tail =
    //     attempt (skip '.' >>. puint8)
    //     <!> "tuple-access"
    //
    // let record_access_tail =
    //     attempt (skip '.' >>. ident)
    //     <!> "record-access"
    //
    // let array_access_tail =
    //
    //     let item =
    //         stringReturn ".." ValueNone
    //         <|>
    //         (expr |>> ValueSome)
    //
    //     item
    //     |> commaSep1
    //     |> betweenWs ('[', ']')
    //
    // // <expr-atom-tail>
    // let rec expr_atom_tail expr : Parser<Expr> =
    //
    //     let tup =
    //         tuple_access_tail
    //         |>> fun field -> Expr.TupleAccess(expr, field)
    //
    //     let record =
    //         record_access_tail
    //         |>> fun field -> Expr.RecordAccess(expr, field)
    //
    //     let array_access =
    //        array_access_tail
    //        |>> fun slice -> Expr.ArrayAccess(expr, slice)
    //
    //     let tail =
    //         choice [ tup; record; array_access ]
    //
    //     (tail >>== expr_atom_tail)
    //     <|>
    //     preturn expr
    //
    // [<Struct>]
    // type private BracketState =
    //     | Empty
    //     | Expr of expr: Expr
    //     | Tuple of tuple: List<Expr>
    //     | Record of record: List<NamedExpr>
    //
    // let expr_in_brackets : Parser<Expr> =
    //
    //     fun stream ->
    //         stream.SkipWs('(')
    //         let state = BracketState.Empty
    //         let mutable error = expected "Bracketed expr, tuple, or record"
    //         let mutable comma = false
    //         let rec loop state =
    //             match stream.Peek() with
    //             | ')' ->
    //                 stream.Skip()
    //                 match state with
    //                 | Expr expr when comma ->
    //                     Reply(Expr.Tuple [expr])
    //                 | Expr expr ->
    //                     Reply(Expr.Bracketed expr)
    //                 | Empty ->
    //                     Reply(ReplyStatus.Error, error)
    //                 | Record fields ->
    //                     fields
    //                     |> (List.ofSeq >> Expr.Record >> Reply)
    //                 | Tuple fields ->
    //                     fields
    //                     |> (List.ofSeq >> Expr.Tuple >> Reply)
    //
    //             | ',' ->
    //                 comma <- true
    //                 stream.SkipWs(1)
    //                 loop state
    //
    //             | _ ->
    //                 let reply = expr stream
    //                 if reply.Status <> Ok then
    //                     Reply(reply.Status, reply.Error)
    //                 else
    //                     stream.SkipWhitespace()
    //                     comma <- false
    //                     match reply.Result with
    //
    //                     // Record items eg: `a:2`
    //                     | Expr.Ident id when stream.Peek() = ':' ->
    //                         stream.SkipWs(1)
    //                         match expr stream with
    //                         | r when r.Status = Ok ->
    //                             let value = r.Result
    //                             stream.SkipWhitespace()
    //                             match state with
    //                             | Empty ->
    //                                 let fields = List<NamedExpr>()
    //                                 fields.Add(id => value)
    //                                 loop (BracketState.Record fields)
    //                             | Record xs ->
    //                                 xs.Add(id => value)
    //                                 loop (BracketState.Record xs)
    //                             | _ ->
    //                                 Reply(ReplyStatus.Error, error)
    //                         | r ->
    //                             Reply(r.Status, r.Error)
    //
    //                     | expr ->
    //                         match state with
    //                         | Empty ->
    //                             loop (Expr expr)
    //                         | Tuple fields ->
    //                             fields.Add(expr)
    //                             loop (Tuple fields)
    //                         | Expr first ->
    //                             let fields = ResizeArray<Expr>(4)
    //                             fields.Add(first)
    //                             fields.Add(expr)
    //                             loop (Tuple fields)
    //                         | _ ->
    //                             Reply(ReplyStatus.Error, error)
    //
    //         let reply = loop Empty
    //         reply
    //
    // type [<Struct>] Array3DState =
    //     | Begin
    //     | End
    //     | ParseElement of optional:bool
    //     | Element of expr:Expr
    //     | NoElement
    //
    // let bool_lit =
    //     stringReturn "true" true
    //     <|>
    //     stringReturn "false" false
    //     |>> Expr.Bool
    //
    // let int_lit =
    //     pint32
    //     |>> Expr.Int
    //
    // let float_lit =
    //     number_lit
    //
    // [<Struct>]
    // type private ArrayParseState =
    //     | ParseElement
    //     | RetryParse
    //     | Element of Expr
    //     | Continue
    //
    // // Parse a 1D array literal using the provided element parser
    // // as the first priority.  Internally this uses an
    // // (element <|> expr) to still allow complex expressions
    // let private array1d_lit_of (element: ValueOption<Parser<Expr>>): Parser<List<Expr>> =
    //     let delimError = expected ", or ]"
    //     let elemError = expected "array element"
    //
    //     fun stream ->
    //         let mutable pElement = ValueOption.defaultValue expr element
    //         let mutable inferred = element.IsSome
    //         let mutable i = 0
    //
    //         let elements = List<Expr>()
    //
    //         let rec loop (state: ArrayParseState) =
    //             match state with
    //
    //             // Parse an element and use it to guess an optimal parser
    //             | ParseElement when not inferred  ->
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     match reply.Result with
    //                     | Expr.Bool _ ->
    //                          pElement <- bool_lit
    //                          inferred <- true
    //                     | Expr.Int _ ->
    //                         pElement <- int_lit
    //                         inferred <- true
    //                     | Expr.Float _ ->
    //                         pElement <- float_lit
    //                         inferred <- true
    //                     | _ ->
    //                         pElement <- expr
    //                         inferred <- false
    //                     loop (Element reply.Result)
    //                 else
    //                     reply.Error
    //
    //             // Parse with the inferred parser
    //             | ParseElement ->
    //                 let tag = stream.StateTag
    //                 let idx = stream.Index
    //                 let reply = pElement stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     if not inferred then
    //                         loop (Element reply.Result)
    //                     // Check ahead in case the element on matched partially
    //                     // for example it could succeed but be part of a binary
    //                     // operation.
    //                     else
    //                         match stream.Peek() with
    //                         | ',' | '|'  ->
    //                             loop (Element reply.Result)
    //                         | _c ->
    //                             stream.Seek(idx)
    //                             loop RetryParse
    //
    //                 // Failed and changed state
    //                 elif stream.StateTag <> tag then
    //                     reply.Error
    //
    //                 // Parser failed but we can retry
    //                 elif inferred then
    //                     loop RetryParse
    //
    //                 // Parser failed and we cannot retry
    //                 else
    //                     reply.Error
    //
    //             // Retry with the backup parser
    //             | RetryParse ->
    //                 let peek = stream.Peek2()
    //                 match peek.Char0, peek.Char1 with
    //                 // Check for wildcard
    //                 | '_', c when not (Char.IsLetter c) ->
    //                     stream.Skip(1)
    //                     loop (Element <| Expr.WildCard WildCard)
    //                 // Check for absent
    //                 | '<', '>' ->
    //                     stream.Skip(2)
    //                     loop (Element <| Expr.Absent Absent)
    //                 | _, _ ->
    //                     let reply = expr stream
    //                     // Retry succeded
    //                     if reply.Status = Ok then
    //                         loop (Element <| reply.Result)
    //                     // Retry failed
    //                     else
    //                         reply.Error
    //
    //             | Element e ->
    //                 elements.Add(e)
    //                 loop Continue
    //
    //             // Check for end or separator
    //             | Continue ->
    //                 if stream.SkipWs(',') then
    //                     if stream.Skip(']') then
    //                         NoErrorMessages
    //                     else
    //                         loop ParseElement
    //                 elif stream.Skip(']') then
    //                     NoErrorMessages
    //                 else
    //                     elemError
    //
    //         let error = loop Continue
    //         if error = NoErrorMessages then
    //             Reply(elements)
    //         else
    //             Reply(ReplyStatus.Error, error)
    //
    // // <array-literal-2d>
    // let array2d_lit : Parser<Expr[,]> =
    //     let delimiterError = expected "\",\" or \"|\" or \"|]\""
    //     let elementError = expected "array element"
    //     fun stream ->
    //         let mutable pElement = expr
    //         let mutable pBackup  = expr
    //         let mutable retry = false
    //         let mutable infer = true
    //         let mutable i = 0 // Current array row
    //         let mutable j = 0 // Current array column
    //         let mutable n = 0 // Current array size
    //         let elements = List<Expr>()
    //
    //         let rec loop (state: ArrayParseState) =
    //             match state with
    //
    //             // Parse an element and use it to guess an optimal parser
    //             | ParseElement when infer ->
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     match reply.Result with
    //                     | Expr.Bool _ ->
    //                          pElement <- bool_lit
    //                          retry <- true
    //                          infer <- false
    //                     | Expr.Int _ ->
    //                         pElement <- int_lit
    //                         retry <- true
    //                         infer <- false
    //                     | Expr.Float _ ->
    //                         pElement <- float_lit
    //                         retry <- true
    //                         infer <- false
    //                     | _ ->
    //                         pElement <- expr
    //                         retry <- false
    //                     loop (Element reply.Result)
    //                 else
    //                     reply.Error
    //
    //             // Parse with the inferred parser
    //             | ParseElement ->
    //                 let tag = stream.StateTag
    //                 let idx = stream.Index
    //                 let reply = pElement stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     if not retry then
    //                         loop (Element reply.Result)
    //                     // Check ahead in case the element on matched partially
    //                     // for example it could succeed but be part of a binary
    //                     // operation.
    //                     else
    //                         match stream.Peek() with
    //                         | ',' | '|'  ->
    //                             loop (Element reply.Result)
    //                         | _c ->
    //                             stream.Seek(idx)
    //                             loop RetryParse
    //
    //                 // Failed and changed state
    //                 elif stream.StateTag <> tag then
    //                     reply.Error
    //
    //                 // Parser failed but we can retry
    //                 elif retry then
    //                     loop RetryParse
    //
    //                 // Parser failed and we cannot retry
    //                 else
    //                     reply.Error
    //
    //             // Retry with the backup parser
    //             | RetryParse ->
    //                 let peek = stream.Peek2()
    //                 match peek.Char0, peek.Char1 with
    //                 // Check for wildcard
    //                 | '_', c when not (Char.IsLetter c) ->
    //                     stream.Skip(1)
    //                     loop (Element <| Expr.WildCard WildCard)
    //                 // Check for absent
    //                 | '<', '>' ->
    //                     stream.Skip(2)
    //                     loop (Element <| Expr.Absent Absent)
    //                 | _, _ ->
    //                     let reply = pBackup stream
    //                     // Retry succeded
    //                     if reply.Status = Ok then
    //                         loop (Element <| reply.Result)
    //                     // Retry failed
    //                     else
    //                         reply.Error
    //
    //             // An element was parses successfully
    //             | Element el ->
    //                 j <- j + 1
    //                 n <- n + 1
    //                 elements.Add(el)
    //                 stream.SkipWhitespace()
    //                 loop Continue
    //
    //             // Check for end or separator
    //             | Continue ->
    //                 let comma = stream.SkipWs(',')
    //                 // End of current row
    //                 if stream.Skip('|') then
    //                     i <- i + 1
    //                     if stream.Skip(']') then
    //                         NoErrorMessages
    //                     elif stream.WsSkip("|]") then
    //                         NoErrorMessages
    //                     else
    //                         j <- 0
    //                         loop ParseElement
    //                 // Current row continues
    //                 elif comma then
    //                     loop ParseElement
    //                 // We expected an element
    //                 else
    //                     elementError
    //
    //         match loop ParseElement with
    //         | NoErrorMessages ->
    //             let I = i
    //             let J = j
    //             let N = n
    //             if I * J <> N then
    //                 let msg = $"Array[{I},{J}] should have {I*J} elements but had {N}"
    //                 Reply(ReplyStatus.Error, messageError msg)
    //             else
    //                 let array = Array2D.zeroCreate I J
    //                 n <- 0
    //                 for i in 0 .. I - 1 do
    //                     for j in 0 .. J - 1 do
    //                         array[i,j] <- elements[n]
    //                         n <- n + 1
    //
    //                 Reply(array)
    //
    //         | errors ->
    //             Reply(ReplyStatus.Error, errors)
    //
    //
    //
    // let array3d_lit : Parser<Expr[,,]> =
    //
    //     let endElementError = messageError "array elements must be followed by ',', '|', '|,' or '| |]"
    //     let expectedArrayElement = expected "Array element expr"
    //     let expectedEndOfArray = expected "|]"
    //     let rowEndError = expected "Array1D row end: | or ','"
    //     let expectedMatrixSep = expected "Array2D separator: |"
    //     let expectedElementOrEndOfArray = expected "Array element or end of array '| |]'"
    //
    //     fun stream ->
    //         let mutable inferred = false
    //         let mutable pElement = expr
    //         let mutable pBackup  = expr
    //         let mutable i = 0 // Current first index
    //         let mutable j = 0 // Current second index
    //         let mutable k = 0 // Current third index
    //         let mutable n = 0 // Total element count
    //
    //         let elements = List<Expr>()
    //
    //         let rec loop (state: ArrayParseState) =
    //             match state with
    //             // Parse an element and use it to guess an optimal parser
    //             | ParseElement when not inferred ->
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     match reply.Result with
    //                     | Expr.Bool _ ->
    //                          pElement <- bool_lit
    //                          inferred <- true
    //                     | Expr.Int _ ->
    //                         pElement <- int_lit
    //                         inferred <- true
    //                     | Expr.Float _ ->
    //                         pElement <- float_lit
    //                         inferred <- true
    //                     | _ ->
    //                         pElement <- expr
    //                     loop (Element reply.Result)
    //                 else
    //                     reply.Error
    //
    //             // Attempt to parse using the infered element parser
    //             | ParseElement ->
    //                 let tag = stream.StateTag
    //                 let idx = stream.Index
    //                 let reply = pElement stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     // We check here in case the inferred parser only partially
    //                     // matches the input.
    //                     let next = stream.Peek()
    //                     if next = ',' || next = '|' then
    //                         loop (Element reply.Result)
    //                     else
    //                         stream.Seek(idx)
    //                         loop RetryParse
    //
    //                 // Failed and changed state
    //                 elif stream.StateTag <> tag then
    //                     reply.Error
    //
    //                 // Parser failed but we can retry
    //                 else
    //                     loop RetryParse
    //
    //             // Retry with the backup parser
    //             | RetryParse ->
    //                 let peek = stream.Peek2()
    //                 match peek.Char0, peek.Char1 with
    //                 // Check for wildcard
    //                 | '_', c when not (Char.IsLetter c) ->
    //                     stream.Skip(1)
    //                     loop (Element <| Expr.WildCard WildCard)
    //                 // Check for absent
    //                 | '<', '>' ->
    //                     stream.Skip(2)
    //                     loop (Element <| Expr.Absent Absent)
    //                 | _, _ ->
    //                     let reply = pBackup stream
    //                     // Retry succeded
    //                     if reply.Status = Ok then
    //                         loop (Element <| reply.Result)
    //                     // Retry failed
    //                     else
    //                         reply.Error
    //
    //             // An element was parses successfully
    //             | Element el ->
    //                 k <- k + 1
    //                 n <- n + 1
    //                 elements.Add(el)
    //                 stream.SkipWhitespace()
    //                 loop Continue
    //
    //             | Continue ->
    //                 let comma = stream.SkipWs(',')
    //                 let pipe = stream.SkipWs('|')
    //
    //                 // End of current 1D array
    //                 if pipe then
    //                     // Start of new 2D array
    //                     if stream.SkipWs(',') then
    //                         if stream.SkipWs('|') then
    //                             k <- 0
    //                             i <- i + 1
    //                             j <- 0
    //                             loop ParseElement
    //                         else
    //                             expectedMatrixSep
    //
    //                     // End of the 3d array
    //                     elif stream.Skip("|]") then
    //                         NoErrorMessages
    //
    //                     // Start of new row in current 2D Array
    //                     else
    //                         k <- 0
    //                         j <- j + 1
    //                         loop ParseElement
    //                 // Continuation of current row
    //                 elif comma then
    //                     loop ParseElement
    //                 // Error
    //                 else
    //                     endElementError
    //
    //         match loop ParseElement with
    //
    //         | NoErrorMessages ->
    //             let I = i + 1
    //             let J = j + 1
    //             let K = k
    //             let N = I * J * K
    //             let array = Array3D.zeroCreate I J K
    //             try
    //                 let mutable z = 0
    //                 for i in 0 .. (I-1) do
    //                     for j in 0 .. (J-1) do
    //                         for k in 0 .. (K-1) do
    //                             array[i,j,k] <- elements[z]
    //                             z <- z + 1
    //
    //                 Reply(array)
    //             with
    //             | exn ->
    //                 Reply(ReplyStatus.Error, messageError exn.Message)
    //
    //         | errors ->
    //             Reply(ReplyStatus.Error, errors)
    //
    //
    // let array_expr : Parser<Expr> =
    //
    //     let error = expectedString "["
    //
    //     let array_comp_tail =
    //         generators .>> ws .>> skipChar ']'
    //
    //     let expectedArrayExpr =
    //         expected "array expression (literal or comp)"
    //
    //     let array3d =
    //         array3d_lit |>> Expr.Array3DLit
    //
    //     let array2d =
    //         array2d_lit |>> Expr.Array2DLit
    //
    //     fun stream ->
    //
    //         if stream.Skip('[') then
    //             // 1D or 2D Array
    //             if stream.SkipWs("|") then
    //                 // Empty 2D Array
    //                 if stream.Skip("|]") then
    //                     Reply(Expr.Array2DLit <| Array2D.zeroCreate 0 0)
    //                 // 3D Array
    //                 elif stream.SkipWs('|') then
    //                     if stream.SkipWs('|') && stream.Skip("|]") then
    //                         Reply(Expr.Array3DLit <| Array3D.zeroCreate 1 0 0)
    //                     else
    //                         array3d stream
    //                 // 2D Array
    //                 else
    //                     array2d stream
    //             // Empty 1D Array
    //             elif stream.WsSkip(']') then
    //                 Reply(Expr.Array1DLit Array.empty)
    //             // Non-empty 1D Array or Comprehension
    //             else
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     // Pipe indicates a comprehension
    //                     if stream.SkipWs('|') then
    //                         match array_comp_tail stream with
    //                         | r when r.Status = Ok ->
    //                             { Yields = reply.Result
    //                             ; IsSet = false
    //                             ; From = r.Result }
    //                             |> Expr.ArrayComp
    //                             |> Reply
    //                         | r ->
    //                             Reply(r.Status, r.Error)
    //
    //                     else
    //                         // Comma separated means array literal
    //                         let first = reply.Result
    //                         let element =
    //                             match first with
    //                             | Expr.Bool _ -> ValueSome bool_lit
    //                             | Expr.Int _ -> ValueSome int_lit
    //                             | Expr.Float _ -> ValueSome float_lit
    //                             | _ -> ValueNone
    //
    //                         match array1d_lit_of element stream with
    //                         | r when r.Status = Ok ->
    //                             let arr = r.Result
    //                             arr.Insert(0, reply.Result)
    //                             let set =
    //                                 arr
    //                                 |> List.ofSeq
    //                                 |> Expr.Set
    //
    //                             Reply set
    //                         | r ->
    //                             Reply(r.Status, r.Error)
    //
    //                 else
    //                     Reply(reply.Status, reply.Error)
    //         else
    //             Reply(ReplyStatus.Error, expectedArrayExpr)
    //
    //
    // // <expr-atom-head>
    // let expr_atom_head : Parser<Expr> =
    //
    //     let wildcard =
    //         wildcard |>> Expr.WildCard
    //
    //     let absent =
    //         absent |>> Expr.Absent
    //
    //     let string_lit =
    //         string_lit |>> Expr.String
    //
    //     let wildcard =
    //         charReturn '_' (Expr.WildCard WildCard)
    //
    //     let absent =
    //         stringReturn "<>" (Expr.Absent Absent)
    //
    //     let id =
    //         ident_kw .>> ws
    //
    //     let set_expr : Parser<Expr> =
    //
    //         let set_lit_tail =
    //             commaSep1 expr .>> skipChar '}'
    //
    //         let set_comp_tail =
    //             generators .>> ws .>> skipChar '}'
    //
    //         fun stream ->
    //             stream.SkipWs('{')
    //
    //             // Empty set literal
    //             if stream.Skip('}') then
    //                 Reply(Expr.Set [])
    //
    //             // Could be set lit or set comp
    //             else
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     // Comma separated means set literal
    //                     if stream.SkipWs(',') then
    //                         match set_lit_tail stream with
    //                         | r when r.Status = Ok ->
    //                             reply.Result :: r.Result
    //                             |> Expr.Set
    //                             |> Reply
    //                         | r ->
    //                             Reply(r.Status, r.Error)
    //
    //                     // Pipe indicated set comprehension
    //                     elif stream.SkipWs('|') then
    //                         match set_comp_tail stream with
    //                         | r when r.Status = Ok ->
    //                             { Yields = reply.Result
    //                             ; IsSet = true
    //                             ; From = r.Result }
    //                             |> Expr.SetComp
    //                             |> Reply
    //                         | r ->
    //                             Reply(r.Status, r.Error)
    //
    //                     // End of set literal
    //                     elif stream.SkipWs('}') then
    //                          Reply(Expr.Set[reply.Result])
    //
    //                     else
    //                         let c = stream.Peek()
    //                         Reply(ReplyStatus.Error, unexpected (string c))
    //
    //                 else
    //                     Reply(reply.Status, reply.Error)
    //
    //     let gen_call_tail =
    //         tuple(
    //             betweenWs('(', ')') generators,
    //             betweenWs('(', ')') expr
    //         )
    //
    //     let let_expr =
    //         let_expr_tail |>> Expr.Let
    //
    //     let if_else_expr =
    //         if_else_expr_tail |>> Expr.IfThenElse
    //
    //     fun stream ->
    //         let tag = stream.StateTag
    //         let reply = ident_kw stream
    //         if reply.Status = Ok then
    //             let struct(id, keyword) = reply.Result
    //             match keyword with
    //
    //             | Keyword.LET ->
    //                 let_expr stream
    //
    //             | Keyword.IF ->
    //                 if_else_expr stream
    //
    //             | Keyword.TRUE ->
    //                 Reply(Expr.Bool true)
    //
    //             | Keyword.FALSE ->
    //                 Reply(Expr.Bool false)
    //
    //             | Keyword.NOT ->
    //                 stream.SkipWhitespace()
    //                 let reply = expr stream
    //                 if reply.Status <> Ok then
    //                     Reply(reply.Status, reply.Error)
    //                 else
    //                     Reply(Expr.UnaryOp (UnOp.Not, reply.Result))
    //
    //             // <ident>, <call-expr>, or <gen-call>
    //             | Keyword.NONE ->
    //                 stream.SkipWhitespace()
    //                 if stream.Peek() = '(' then
    //                     let mutable state = CharStreamState(stream)
    //                     let reply = gen_call_tail stream
    //                     if reply.Status = Ok then
    //                         { Id = id
    //                         ; From = fst reply.Result
    //                         ; Yields = snd reply.Result }
    //                         |> Expr.GenCall
    //                         |> Reply
    //                     else
    //                         stream.BacktrackTo(&state)
    //                         match tupled_args stream with
    //                         | r when r.Status = Ok ->
    //                             Expr.Call (id, r.Result)
    //                             |> Reply
    //                         | r ->
    //                             Reply(r.Status, r.Error)
    //                 else
    //                     Reply(Expr.Ident id)
    //
    //             // TODO This is an error usually except when used as an annotation eg: `:: output`
    //             | word ->
    //                 Reply(Expr.Ident id)
    //
    //         else if stream.StateTag = tag then
    //             match stream.Peek() with
    //             | '(' -> expr_in_brackets stream
    //             | '{' -> set_expr stream
    //             | '[' -> array_expr stream
    //             | '_' -> wildcard stream
    //             | '<' -> absent stream
    //             | '"' -> string_lit stream
    //             | _other -> number_lit stream
    //
    //         else
    //             Reply(reply.Status, reply.Error)
    //
    // // <annotations>
    // annotation_ref.contents <-
    //     skip "::"
    //     >>. expr_atom
    //     <!> "annotation"
    //
    // expr_atom_ref.contents <-
    //     expr_atom_head
    //     >>== expr_atom_tail
    //     <!> "expr-atom"
    //
    // let solve_type : Parser<SolveMethod> =
    //     choice
    //       [ stringReturn "satisfy" SolveMethod.Satisfy
    //       ; stringReturn "minimize" SolveMethod.Minimize
    //       ; stringReturn "maximize" SolveMethod.Maximize ]
    //
    // // <solve-item>
    // let solve_item : Parser<Item> =
    //     pipe(
    //         annotations,
    //         solve_type,
    //         opt expr,
    //         fun anns solveType obj ->
    //             match (solveType, obj) with
    //             | SolveMethod.Maximize, Some exp ->
    //                 SolveItem.Max (exp, anns)
    //             | SolveMethod.Minimize, Some exp ->
    //                 SolveItem.Min (exp, anns)
    //             | _ ->
    //                 SolveItem.Sat anns
    //             |> Item.Solve)
    //
    // // <type-inst-syn-item>
    // let alias_item : Parser<Item> =
    //     pipe(
    //         ident,
    //         annotations,
    //         skip '=',
    //         ti_expr,
    //         fun id anns _ ti ->
    //             { Name = id
    //             ; Annotations = anns
    //             ; TypeInst = ti }
    //             |> Item.Synonym
    //             )
    //
    // // <output-item>
    // let output_item : Parser<Item> =
    //     pipe(
    //         opt string_annotation,
    //         expr,
    //         fun ann expr ->
    //             { Expr = expr
    //             ; Annotation = ann }
    //             |> Item.Output)
    //
    // // <annotation_item>
    // let ann_item : Parser<Item> =
    //     pipe(
    //         ident,
    //         opt_or [] parameters,
    //         opt assign_tail,
    //         fun name pars body ->
    //             { Name = name
    //             ; Body = body
    //             ; Params = pars }
    //             |> Item.Annotation)
    //     <!> "annotation-type"
    //
    // // <item>
    // let item : Parser<Item> =
    //     let err = expected "item"
    //     let idkw = ident_kw .>> ws
    //     fun stream ->
    //         let mutable state = CharStreamState(stream)
    //         let tag = stream.StateTag
    //         let reply = idkw stream
    //         if reply.Status = Ok then
    //             let struct(name, word) = reply.Result
    //             match word with
    //             | Keyword.ENUM ->
    //                 enum_item stream
    //             | Keyword.CONSTRAINT ->
    //                 constraint_item stream
    //             | Keyword.SOLVE ->
    //                 solve_item stream
    //             | Keyword.TYPE ->
    //                 alias_item stream
    //             | Keyword.OUTPUT ->
    //                 output_item stream
    //             | Keyword.PREDICATE ->
    //                 predicate_item stream
    //             | Keyword.FUNCTION ->
    //                 function_item stream
    //             | Keyword.TEST ->
    //                 test_item stream
    //             | Keyword.NONE when stream.Peek() = '=' ->
    //                 let reply = assign_tail stream
    //                 if reply.Status = Ok then
    //                     Reply <| Item.Assign (name => reply.Result)
    //                 else
    //                     Reply(reply.Status, reply.Error)
    //             | _other ->
    //                 stream.BacktrackTo(&state)
    //                 declare_item stream
    //
    //         // Not an identifier, assume part of declare
    //         // eg: 1..10: x;
    //         else
    //           declare_item stream
    //
    // let ast : Parser<Ast> =
    //
    //     let sep = skip ';'
    //
    //     ws
    //     >>. sepEndBy (item .>> ws) sep
    //     .>> eof
    //
    // let private assign_expr : Parser<NamedExpr> =
    //     let error = expected "Assignment (id = expr)"
    //     fun stream ->
    //         let reply = ident stream
    //         if reply.Status = Ok then
    //             stream.SkipWhitespace()
    //             let id = reply.Result
    //             if stream.Skip('=') then
    //                 stream.Skip('=')
    //                 stream.SkipWhitespace()
    //                 let exprReply = expr stream
    //                 if exprReply.Status = Ok then
    //                     stream.SkipWhitespace()
    //                     Reply(id => exprReply.Result)
    //                 else
    //                     Reply(exprReply.Status, exprReply.Error)
    //             else
    //                 Reply(ReplyStatus.Error, error)
    //         else
    //             Reply(reply.Status, reply.Error)
    //
    //
    // let data : Parser<NamedExpr list> =
    //     ws
    //     >>. sepEndBy assign_expr (skip ';')
    //     .>> ws
    //     .>> eof
    private bool XD() => Error("not implemented XD");

    /// <summary>
    /// Parse anything that starts with a '[', this could be:
    ///
    /// - [1,2,3]
    /// - []
    /// - [| 1, 2, 3 | 4, 5, 6 |]
    /// - [ x | x in [ 1, 2, 3 ]]
    /// </summary>
    private bool ParseArrayLikeExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Expect(TokenKind.OpenBracket))
            return false;

        IExpr element;

        // Empty Array
        if (_token.Kind is TokenKind.CloseBracket)
        {
            result = new Array1DLit();
            goto end;
        }

        // Array2D
        if (Skip(TokenKind.Pipe))
        {
            var arr2D = new Array2DLit();
            result = arr2D;
            if (Skip(TokenKind.Pipe))
                goto end;

            array_2d_row:
            arr2D.I = 0;
            while (_token.Kind is not TokenKind.Pipe)
            {
                element = ParseExpr();
                arr2D.Elements.Add(element);
                arr2D.I++;
                if (!Skip(TokenKind.Comma))
                    break;
            }

            arr2D.J++;
            Expect(TokenKind.Pipe);
            if (_token.Kind is TokenKind.CloseBracket)
                goto end;
            goto array_2d_row;
        }

        // Parse the first element
        element = ParseExpr();

        // Array comprehension
        if (Skip(TokenKind.Pipe))
        {
            var comp = new CompExpr
            {
                Yields = element,
                IsSet = false,
                From = ParseGenerators()
            };
            result = comp;
            goto end;
        }

        var arr = new Array1DLit();
        result = arr;
        arr.Elements.Add(element);
        while (_token.Kind is not TokenKind.CloseBracket)
        {
            element = ParseExpr();
            arr.Elements.Add(element);
            if (!Skip(TokenKind.Comma))
                break;
        }

        end:
        return Expect(TokenKind.CloseBracket);
    }

    /// <summary>
    /// Parse anything that starts with a '{', this could be:
    /// - {1,2,3}
    /// - {}
    /// - { x | x in [1,1,1,2,3,]}
    /// </summary>
    private bool ParseSetLikeExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Expect(TokenKind.OpenBrace))
            return false;

        // Empty Set
        if (_token.Kind is TokenKind.CloseBrace)
        {
            result = new SetLit();
            goto end;
        }

        if (!ParseExpr(out var element))
            return false;

        // Set comprehension
        if (Skip(TokenKind.Pipe))
        {
            var comp = new CompExpr { Yields = element, IsSet = true };
            if (!ParseGenerators(comp.From))
                return false;

            result = comp;
            goto end;
        }

        // Set literal
        var set = new SetLit();
        result = set;
        set.Elements.Add(element);
        while (_token.Kind is not TokenKind.CloseBrace)
        {
            if (!ParseExpr(out var item))
                return false;

            set.Elements.Add(item);
            if (!Skip(TokenKind.Comma))
                break;
        }

        end:
        return Expect(TokenKind.CloseBrace);
    }

    private bool ParseGenerators(List<GeneratorExpr> generators)
    {
        return false;
    }
}
