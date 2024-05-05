namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    /// <summary>
    /// Parse a model
    /// </summary>
    /// <mzn>var 1..10: a; var 10..20: b; constraint a = b;</mzn>
    public bool ParseModel(out SyntaxTree tree)
    {
        tree = new SyntaxTree();
        while (true)
        {
            var token = Step();
            switch (token.Kind)
            {
                case TokenKind.INCLUDE:
                    if (!ParseIncludeStatement(token, out var inc))
                        return false;
                    tree.Includes.Add(inc);
                    break;

                case TokenKind.CONSTRAINT:
                    if (!ParseConstraintItem(token, out var cons))
                        return false;
                    tree.Constraints.Add(cons);
                    break;

                case TokenKind.SOLVE:
                    if (!ParseSolveItem(token, out var solve))
                        return false;
                    tree.SolveItems.Add(solve);
                    break;

                case TokenKind.OUTPUT:
                    if (!ParseOutputItem(token, out var output))
                        return false;
                    tree.Outputs.Add(output);
                    break;

                case TokenKind.ENUM:
                    if (!ParseEnumItem(token, out var @enum))
                        return false;
                    tree.Nodes.Add(@enum);
                    break;

                case TokenKind.TYPE:
                    if (!ParseAliasItem(tree))
                        return false;
                    break;

                case TokenKind.BLOCK_COMMENT:
                case TokenKind.LINE_COMMENT:
                    Step();
                    continue;

                case TokenKind.EOF:
                    return true;

                default:
                    if (!ParseDeclareOrAssignItem(token, out var declare, out var assign))
                        return false;
                    break;
            }

            if (Skip(TokenKind.EOL))
                continue;

            if (token.Kind is not TokenKind.EOF)
                return Expected("; or end of file");
        }
    }

    /// <summary>
    /// Parse an enumeration declaration
    /// </summary>
    /// <example>enum Dir = {N,S,E,W};</example>
    /// <example>enum Z = anon_enum(10);</example>
    /// <example>enum X = Q({1,2});</example>
    public bool ParseEnumItem(in Token start, out EnumDeclarationSyntax @enum)
    {
        @enum = null!;
        if (!Expect(TokenKind.ENUM))
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
    public bool ParseOutputItem(in Token start, out OutputSyntax node)
    {
        node = null!;

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
    public bool ParseAliasItem(SyntaxTree syntaxTree)
    {
        if (!Expect(TokenKind.TYPE))
            return false;
        if (!ParseString(out var name))
            return false;
        if (!Expect(TokenKind.EQUAL))
            return false;
        if (!ParseType(out var type))
            return false;
        syntaxTree.Nodes.Add(type);
        return true;
    }

    /// <summary>
    /// Parse an include item
    /// </summary>
    /// <mzn>include "utils.mzn"</mzn>
    public bool ParseIncludeStatement(in Token token, out IncludeSyntax node)
    {
        node = null!;
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
    public bool ParseSolveItem(in Token start, out SolveSyntax node)
    {
        node = null!;

        if (!ParseAnnotations(out var anns))
            return false;

        SyntaxNode? objective = null;
        SolveMethod method = SolveMethod.Satisfy;

        var token = Step();
        switch (token.Kind)
        {
            case TokenKind.SATISFY:
                method = SolveMethod.Satisfy;
                break;

            case TokenKind.MINIMIZE:
                method = SolveMethod.Minimize;
                if (!ParseExpr(out objective))
                    return false;
                break;

            case TokenKind.MAXIMIZE:
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
    public bool ParseConstraintItem(in Token start, out ConstraintSyntax constraint)
    {
        constraint = null!;

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
    public bool ParseDeclareOrAssignItem(
        in Token token,
        out VariableDeclarationSyntax? var,
        out VariableAssignmentSyntax? assign
    )
    {
        var = null;
        assign = null;
        var kind = token.Kind;
        Token name;

        if (kind is TokenKind.IDENT or TokenKind.QUOTED_IDENT)
        {
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = new VariableAssignmentSyntax(token, expr);
                return true;
            }

            var = new VariableDeclarationSyntax(token)
            {
                Type = new TypeInstSyntax(_token) { Kind = TypeKind.Name, Name = token }
            };
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.GENERIC or TokenKind.GENERIC_SEQ)
        {
            var id = _token.StringValue;
            Step();
            if (Skip(TokenKind.EQUAL))
            {
                if (!ParseExpr(out var expr))
                    return false;
                assign = new VariableAssignmentSyntax(token, expr);
                return true;
            }

            var = new VariableDeclarationSyntax(token)
            {
                Type = new TypeInstSyntax(_token)
                {
                    Kind = _kind is TokenKind.GENERIC ? TypeKind.Generic : TypeKind.GenericSeq,
                    Name = token
                }
            };
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.FUNCTION)
        {
            if (!ParseType(out var type))
                return false;

            var = new VariableDeclarationSyntax(token) { Type = type };
            Expect(TokenKind.COLON);
        }
        else if (kind is TokenKind.PREDICATE)
        {
            if (!ParseIdent(out name))
                return false;

            var = new VariableDeclarationSyntax(token)
            {
                Type = new TypeInstSyntax(name) { Kind = TypeKind.Bool }
            };
        }
        else if (kind is TokenKind.TEST)
        {
            if (!ParseIdent(out name))
                return false;

            var = new VariableDeclarationSyntax(token)
            {
                Type = new TypeInstSyntax(name) { Kind = TypeKind.Bool }
            };
        }
        else if (kind is TokenKind.ANNOTATION)
        {
            if (!ParseIdent(out name))
                return false;

            var = new VariableDeclarationSyntax(token)
            {
                Type = new TypeInstSyntax(name) { Kind = TypeKind.Annotation }
            };
        }
        else if (kind is TokenKind.ANY)
        {
            if (!ParseIdent(out name))
                return false;

            var = new VariableDeclarationSyntax(token)
            {
                Type = new TypeInstSyntax(name) { Kind = TypeKind.Any }
            };
        }
        else if (ParseType(out var type))
        {
            var = new VariableDeclarationSyntax(token) { Type = type };
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

        var!.Name = name;

        // Function call
        if (_kind is TokenKind.OPEN_PAREN)
        {
            if (!ParseParameters(out var pars))
                return false;
            var.Parameters = pars;
            var.IsFunction = true;
        }

        if (!ParseAnnotations(out var anns))
            return false;

        var.Annotations = anns;

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
