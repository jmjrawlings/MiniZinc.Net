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
        next:
        if (Skip(TokenKind.EOF))
            return true;

        switch (_kind)
        {
            case TokenKind.INCLUDE:
                if (!ParseIncludeItem(model))
                    return false;
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
                if (!ParseEnumItem(model))
                    return false;
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
                if (!ParseDeclareOrAssignItem(model.NameSpace))
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
    public bool ParseEnumItem(Model model)
    {
        if (!Expect(TokenKind.ENUM))
            return false;

        if (!ParseIdent(out var name))
            return false;

        var @enum = new EnumDeclare { Name = name };
        if (!ParseAnnotations(@enum))
            return false;

        if (Skip(TokenKind.EOL))
            return true;

        if (!Expect(TokenKind.EQUAL))
            return false;

        cases:
        var @case = new EnumCase();

        // Named cases: 'enum Dir = {N,S,E,W};`
        if (Skip(TokenKind.OPEN_BRACE))
        {
            @case.Type = EnumCaseType.Name;
            @case.Names = new List<string>();
            while (_kind is not TokenKind.CLOSE_BRACE)
            {
                if (!ParseIdent(out name))
                    return false;
                @case.Names.Add(name);
                if (!Skip(TokenKind.COMMA))
                    break;
            }

            if (!Expect(TokenKind.CLOSE_BRACE))
                return false;

            goto end;
        }

        // Underscore enum `enum A = _(1..10);`
        if (Skip(TokenKind.UNDERSCORE))
        {
            @case.Type = EnumCaseType.Underscore;
            if (!Expect(TokenKind.OPEN_PAREN))
                return false;
            if (!ParseExpr(out @case.Expr))
                return false;
            if (!Expect(TokenKind.CLOSE_PAREN))
                return false;
            goto end;
        }

        // Anonymous enum: `anon_enum(10);`
        if (Skip(TokenKind.ANONENUM))
        {
            @case.Type = EnumCaseType.Anonymous;
            if (!Expect(TokenKind.OPEN_PAREN))
                return false;
            if (!ParseExpr(out @case.Expr))
                return false;
            if (!Expect(TokenKind.CLOSE_PAREN))
                return false;
            goto end;
        }

        // Complex enum: `C(1..10)`
        if (ParseIdent(out name))
        {
            @case.Type = EnumCaseType.Complex;
            if (!Expect(TokenKind.OPEN_PAREN))
                return false;
            if (!ParseExpr(out @case.Expr))
                return false;
            if (!Expect(TokenKind.CLOSE_PAREN))
                return false;
            goto end;
        }

        return Error("Expected an enum definition");

        end:
        if (Skip(TokenKind.PLUS_PLUS))
            goto cases;

        model.NameSpace.Push(@enum.Name, @enum);
        return EndLine();
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
        return EndLine();
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
        return EndLine();
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
        return EndLine();
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
        switch (_token.Kind)
        {
            case TokenKind.SATISFY:
                Step();
                item.Method = SolveMethod.Satisfy;
                break;
            case TokenKind.MINIMIZE:
                Step();
                item.Method = SolveMethod.Minimize;
                if (!ParseExpr(out item.Objective))
                    return false;
                break;
            case TokenKind.MAXIMIZE:
                Step();
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

    /// <summary>
    /// Parse a constraint
    /// </summary>
    /// <mzn>constraint a > b;</mzn>
    public bool ParseConstraintItem(out ConstraintItem constraint)
    {
        constraint = new ConstraintItem();
        if (!Expect(TokenKind.CONSTRAINT))
            return false;

        if (!ParseExpr(out constraint.Expr))
            return false;

        return true;
    }

    /// <summary>
    /// Parse a variable declaration or variable assignment
    /// </summary>
    /// <mzn>a = 10;</mzn>
    /// <mzn>set of var int: xd;</mzn>
    public bool ParseDeclareOrAssignItem(NameSpace<IExpr> ns)
    {
        var var = new Variable();

        if (ParseIdent(out var name))
        {
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                ns.Push(name, expr);
                return true;
            }
            var.Type = new TypeInst();
            var.Type.Kind = TypeKind.Name;
            var.Type.Name = name;
            var.Type.Flags = TypeFlags.Var;
        }
        else if (ParseType(out var type))
            var.Type = type;
        else
            return false;

        if (!Expect(TokenKind.COLON))
            return false;

        if (!ParseIdent(out name))
            return false;

        var.Name = name;

        if (Skip(TokenKind.EQUAL))
            if (!ParseExpr(out var value))
                return false;
            else
                var.Value = value;

        if (!EndLine())
            return false;

        return true;
    }
}
