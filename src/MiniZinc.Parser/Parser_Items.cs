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
        next:
        if (Skip(TokenKind.EOF))
            return true;

        switch (_kind)
        {
            case TokenKind.INCLUDE:
                if (!ParseIncludeItem(model))
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
                break;

            default:
                if (!ParseDeclareOrAssignItem(out var declare, out var assign))
                    return false;
                break;
        }

        goto next;
    }

    /// <summary>
    /// Parse an enumeration declaration
    /// </summary>
    /// <example>enum Dir = {N,S,E,W};</example>
    /// <example>enum Z = anon_enum(10);</example>
    /// <example>enum X = Q({1,2});</example>
    public bool ParseEnumItem(out EnumDeclare @enum)
    {
        @enum = default!;
        if (!Expect(TokenKind.ENUM))
            return false;

        if (!ParseIdent(out var name))
            return false;

        @enum = new EnumDeclare { Name = name };
        if (!ParseAnnotations(@enum))
            return false;

        if (Skip(TokenKind.EOL))
            return true;

        if (!Expect(TokenKind.EQUAL))
            return false;

        cases:
        IEnumCases @case;

        // Named cases: 'enum Dir = {N,S,E,W};`
        if (Skip(TokenKind.OPEN_BRACE))
        {
            var names = new NamedEnumCases(new List<string>());
            while (_kind is not TokenKind.CLOSE_BRACE)
            {
                if (!ParseIdent(out name))
                    return false;
                names.Names.Add(name);

                if (!Skip(TokenKind.COMMA))
                    break;
            }

            if (!Expect(TokenKind.CLOSE_BRACE))
                return false;
            @case = names;
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
            @case = new ComplexEnumCase(expr, EnumCaseType.Names);
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
            @case = new ComplexEnumCase(expr, EnumCaseType.Anon);
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
            @case = new ComplexEnumCase(expr, EnumCaseType.Complex);
            goto end;
        }

        return Error("Expected an enum definition");

        end:
        @enum.Cases.Add(@case);
        
        if (Skip(TokenKind.PLUS_PLUS))
            goto cases;

        return Expect(TokenKind.EOL);
    }

    /// <summary>
    /// Parse an Output Item
    /// </summary>
    /// <mzn>output ["The result is \(result)"];</mzn>
    public bool ParseOutputItem(Model model)
    {
        if (!Expect(TokenKind.OUTPUT))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        var output = new OutputItem { Expr = expr };
        model.Outputs.Add(output);
        return Expect(TokenKind.EOL);
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
        return Expect(TokenKind.EOL);
    }

    /// <summary>
    /// Parse an include item
    /// </summary>
    /// <mzn>include "utils.mzn"</mzn>
    public bool ParseIncludeItem(Model model)
    {
        if (!Expect(TokenKind.INCLUDE))
            return false;

        if (!ParseString(out var path))
            return false;

        model.Includes.Add(path);
        return Expect(TokenKind.EOL);
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

        var item = new SolveItem();

        if (!ParseAnnotations(item))
            return false;

        INode objective = Expr.Null;
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
        return Expect(TokenKind.EOL);
    }

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    public bool ParseConstraintItem(out ConstraintItem constraint)
    {
        constraint = new ConstraintItem();

        if (!Skip(TokenKind.CONSTRAINT))
            return false;

        if (!ParseExpr(out var expr))
            return false;

        constraint.Expr = expr;
        return Expect(TokenKind.EOL);
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    public bool ParseDeclareOrAssignItem(out Variable? var, out Assignment? assign)
    {
        var = null;
        assign = null;

        if (ParseIdent(out var name))
        {
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = Expr.Assign(name, expr);
                return Expect(TokenKind.EOL);
            }

            var = new Variable
            {
                Type = new TypeInst
                {
                    Kind = TypeKind.Name,
                    Name = name,
                    Flags = TypeFlags.Var
                }
            };
            if (!Expect(TokenKind.COLON))
                return false;
        }
        else if (Skip(TokenKind.FUNCTION))
        {
            if (!ParseType(out var type))
                return false;

            var = new Variable();
            var.Name = name;
            var.Type = type;
            var.IsFunction = true;
            if (!Expect(TokenKind.COLON))
                return false;
        }
        else if (Skip(TokenKind.PREDICATE))
        {
            if (!ParseIdent(out name))
                return false;

            var = new Variable();
            var.Type = new TypeInst { Kind = TypeKind.Bool, Flags = TypeFlags.Var };
            var.IsFunction = true;
        }
        else if (ParseType(out var type))
        {
            var = new Variable { Type = type };
            if (!Expect(TokenKind.COLON))
                return false;
        }
        else
        {
            return Error("Expected a variable declaration or assignment");
        }

        if (!ParseIdent(out name))
            return false;
        var.Name = name;

        if (var.IsFunction)
            if (!ParseParameters(out var pars))
                return false;
            else
                var.Parameters = pars;

        if (!ParseAnnotations(var))
            return false;

        // Declaration only
        if (Skip(TokenKind.EOL))
            return true;

        // Assignment
        if (!Expect(TokenKind.EQUAL))
            return false;

        if (!ParseExpr(out var value))
            return false;

        var.Body = value;
        return Expect(TokenKind.EOL);
    }
}
