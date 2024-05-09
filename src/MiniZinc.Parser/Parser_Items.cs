namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    /// <summary>
    /// Parse a model
    /// </summary>
    /// <mzn>var 1..10: a; var 10..20: b; constraint a = b;</mzn>
    internal bool Parse(out SyntaxTree tree)
    {
        tree = new SyntaxTree();
        while (true)
        {
            switch (_kind)
            {
                case TokenKind.INCLUDE:
                    if (!ParseIncludeStatement(out var inc))
                        return false;
                    tree.Includes.Add(inc);
                    break;

                case TokenKind.CONSTRAINT:
                    if (!ParseConstraintItem(out var cons))
                        return false;
                    tree.Constraints.Add(cons);
                    break;

                case TokenKind.SOLVE:
                    if (!ParseSolveItem(out var solve))
                        return false;
                    tree.SolveItems.Add(solve);
                    break;

                case TokenKind.OUTPUT:
                    if (!ParseOutputItem(out var output))
                        return false;
                    tree.Outputs.Add(output);
                    break;

                case TokenKind.ENUM:
                    if (!ParseEnumItem(out var @enum))
                        return false;
                    tree.Enums.Add(@enum);
                    break;

                case TokenKind.TYPE:
                    if (!ParseAliasItem(out var alias))
                        return false;
                    tree.Aliases.Add(alias);
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
                    else if (declare!.IsAnnotation)
                        tree.Annotations.Add(declare);
                    else if (declare.IsFunction)
                        tree.Functions.Add(declare);
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
    public bool ParseEnumItem(out EnumDeclarationSyntax @enum)
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
    public bool ParseOutputItem(out OutputSyntax node)
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
    public bool ParseAliasItem(out TypeAliasSyntax alias)
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

        alias = new TypeAliasSyntax(start, type);
        return true;
    }

    /// <summary>
    /// Parse an include item
    /// </summary>
    /// <mzn>include "utils.mzn"</mzn>
    public bool ParseIncludeStatement(out IncludeSyntax node)
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
    public bool ParseSolveItem(out SolveSyntax node)
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
    public bool ParseConstraintItem(out ConstraintSyntax constraint)
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
    public bool ParseDeclarationOrAssignment(
        out DeclarationSyntax? variable,
        out VariableAssignmentSyntax? assign
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
                assign = new VariableAssignmentSyntax(name, expr);
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
                assign = new VariableAssignmentSyntax(token, expr);
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
            if (!ParseType(out type))
                return false;

            variable = new DeclarationSyntax(token, type);
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.PREDICATE)
        {
            if (!ParseIdent(out name))
                return false;

            type = new TypeSyntax(name) { Kind = TypeKind.Bool };
            variable = new DeclarationSyntax(token, type);
        }
        else if (kind is TokenKind.TEST)
        {
            if (!ParseIdent(out name))
                return false;

            type = new TypeSyntax(name) { Kind = TypeKind.Bool };
            variable = new DeclarationSyntax(token, type);
        }
        else if (kind is TokenKind.ANNOTATION)
        {
            type = new TypeSyntax(name) { Kind = TypeKind.Annotation };
            variable = new DeclarationSyntax(token, type);
        }
        else if (kind is TokenKind.ANY)
        {
            if (!ParseIdent(out name))
                return false;

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
            variable.IsFunction = true;
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
}
