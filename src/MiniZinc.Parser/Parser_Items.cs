namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    /// <summary>
    /// Parse a model
    /// </summary>
    /// <mzn>var 1..10: a; var 10..20: b; constraint a = b;</mzn>
    public bool ParseModel(out Model model)
    {
        model = new Model();
        Step();

        while (true)
        {
            switch (_kind)
            {
                case TokenKind.INCLUDE:
                    if (!ParseIncludeStatement(model))
                        return false;
                    break;

                case TokenKind.CONSTRAINT:
                    if (!ParseConstraintItem(out var cons))
                        return false;
                    model.Constraints.Add(cons);
                    break;

                case TokenKind.SOLVE:
                    if (!ParseSolveItem(model))
                        return false;
                    break;

                case TokenKind.OUTPUT:
                    if (!ParseOutputItem(model))
                        return false;
                    break;

                case TokenKind.ENUM:
                    if (!ParseEnumItem(out var @enum))
                        return false;
                    model.NameSpace.Push(@enum.Name, @enum);
                    break;

                case TokenKind.TYPE:
                    if (!ParseAliasItem(model))
                        return false;
                    break;

                case TokenKind.BLOCK_COMMENT:
                case TokenKind.LINE_COMMENT:
                    Step();
                    continue;

                case TokenKind.EOF:
                    return true;

                default:
                    if (!ParseDeclareOrAssignItem(out var declare, out var assign))
                        return false;
                    break;
            }

            if (Skip(TokenKind.EOL))
                continue;

            if (_kind is not TokenKind.EOF)
                return Error("Expected ; or end of file");
        }
    }

    /// <summary>
    /// Parse an enumeration declaration
    /// </summary>
    /// <example>enum Dir = {N,S,E,W};</example>
    /// <example>enum Z = anon_enum(10);</example>
    /// <example>enum X = Q({1,2});</example>
    public bool ParseEnumItem(out EnumStatement @enum)
    {
        @enum = default!;
        if (!Expect(TokenKind.ENUM))
            return false;

        if (!ParseIdent(out var name))
            return false;

        @enum = new EnumStatement { Name = name };
        if (!ParseAnnotations(@enum))
            return false;

        if (_kind is TokenKind.EOL)
            return true;

        if (!Expect(TokenKind.EQUAL))
            return false;

        cases:
        EnumCases @case;

        // Named cases: 'enum Dir = {N,S,E,W};`
        if (Skip(TokenKind.OPEN_BRACE))
        {
            var names = new List<string>();
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
            @case = new EnumCases { Type = EnumCaseType.Names, Names = names };
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
            @case = new EnumCases { Type = EnumCaseType.Underscore, Expr = expr };
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
            @case = new EnumCases { Type = EnumCaseType.Anon, Expr = expr };
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
            @case = new EnumCases { Type = EnumCaseType.Complex, Expr = expr };
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
    public bool ParseOutputItem(Model model)
    {
        if (!Expect(TokenKind.OUTPUT))
            return false;

        var item = new OutputStatement();

        if (!ParseStringAnnotations(item))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        item.Expr = expr;
        model.Outputs.Add(item);
        return true;
    }

    /// <summary>
    /// Parse a type alias
    /// </summary>
    /// <mzn>type X = 1 .. 10;</mzn>
    public bool ParseAliasItem(Model model)
    {
        if (!Expect(TokenKind.TYPE))
            return false;
        if (!ParseString(out var name))
            return false;
        if (!Expect(TokenKind.EQUAL))
            return false;
        if (!ParseType(out var type))
            return false;
        model.NameSpace.Push(name, type);
        return true;
    }

    /// <summary>
    /// Parse an include item
    /// </summary>
    /// <mzn>include "utils.mzn"</mzn>
    public bool ParseIncludeStatement(Model model)
    {
        if (!Expect(TokenKind.INCLUDE))
            return false;

        if (!ParseString(out var path))
            return false;

        var item = new IncludeStatement { Path = path };
        model.Includes.Add(item);
        return true;
    }

    /// <summary>
    /// Parse a solve item
    /// </summary>
    /// <mzn>solve satisfy;</mzn>
    /// <mzn>solve maximize a;</mzn>
    public bool ParseSolveItem(Model model)
    {
        if (!Expect(TokenKind.SOLVE))
            return false;

        var item = new SolveStatement();

        if (!ParseAnnotations(item))
            return false;

        Expr objective = Expr.Null;
        switch (_token.Kind)
        {
            case TokenKind.SATISFY:
                Step();
                item.Method = SolveMethod.Satisfy;
                item.Objective = Expr.Null;
                break;

            case TokenKind.MINIMIZE:
                Step();
                item.Method = SolveMethod.Minimize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            case TokenKind.MAXIMIZE:
                Step();
                item.Method = SolveMethod.Maximize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            default:
                return Error("Expected satisfy, minimize, or maximize");
        }

        item.Objective = objective;
        model.SolveItems.Add(item);
        return true;
    }

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    public bool ParseConstraintItem(out ConstraintStatement constraint)
    {
        constraint = default!;

        if (!Skip(TokenKind.CONSTRAINT))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        constraint = new ConstraintStatement { Expr = expr };

        if (!ParseStringAnnotations(constraint))
            return false;

        constraint.Expr = expr;
        return Okay;
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    public bool ParseDeclareOrAssignItem(out DeclareStatement? var, out AssignStatement? assign)
    {
        var = null;
        assign = null;

        if (ParseIdent(out var name))
        {
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = new AssignStatement(name, expr);
                return true;
            }

            var = new DeclareStatement
            {
                Type = new TypeInst { Kind = TypeKind.Name, Name = name }
            };
            Expect(TokenKind.COLON);
        }
        else if (_kind is TokenKind.POLYMORPHIC)
        {
            var id = _token.StringValue;
            Step();
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = new AssignStatement(name, expr);
                return true;
            }

            var = new DeclareStatement
            {
                Type = new TypeInst { Kind = TypeKind.PolyMorphic, Name = name }
            };
            Expect(TokenKind.COLON);
        }
        else if (Skip(TokenKind.FUNCTION))
        {
            if (!ParseType(out var type))
                return false;

            var = new DeclareStatement { Type = type };
            Expect(TokenKind.COLON);
        }
        else if (Skip(TokenKind.PREDICATE))
        {
            if (!ParseIdent(out name))
                return false;

            var = new DeclareStatement { Type = new TypeInst { Kind = TypeKind.Bool } };
        }
        else if (Skip(TokenKind.TEST))
        {
            if (!ParseIdent(out name))
                return false;

            var = new DeclareStatement { Type = new TypeInst { Kind = TypeKind.Bool } };
        }
        else if (Skip(TokenKind.ANNOTATION))
        {
            if (!ParseIdent(out name))
                return false;

            var = new DeclareStatement { Type = new TypeInst { Kind = TypeKind.Annotation } };
        }
        else if (Skip(TokenKind.ANY))
        {
            if (!ParseIdent(out name))
                return false;

            var = new DeclareStatement { Type = new TypeInst { Kind = TypeKind.Any } };
        }
        else if (ParseType(out var type))
        {
            var = new DeclareStatement { Type = type };
            Expect(TokenKind.COLON);
        }
        else
        {
            Error("Expected a variable declaration or assignment");
        }

        if (ErrorString is not null)
            return Error();

        if (!ParseIdent(out name))
            return false;

        var!.Name = name;

        // Function call
        if (_kind is TokenKind.OPEN_PAREN)
        {
            if (!ParseParameters(out var pars))
                return false;
            var.Parameters = pars;
            var.IsFunction = true;
        }

        if (!ParseAnnotations(var))
            return false;

        // Declaration only
        if (!Skip(TokenKind.EQUAL))
            return true;

        // Assignment right hand side
        if (!ParseExpr(out var value))
            return false;

        var.Body = value;
        return true;
    }
}
