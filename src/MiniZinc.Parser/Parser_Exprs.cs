namespace MiniZinc.Parser;

using Ast;

public partial class Parser
{
    public bool ParseExpr(out IExpr expr)
    {
        expr = null;

        switch (_token.Kind)
        {
            case TokenKind.Plus:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.UnOp(Operator.Plus, expr);
                break;

            case TokenKind.Minus:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.UnOp(Operator.Minus, expr);
                break;

            case TokenKind.KeywordNot:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.UnOp(Operator.Not, expr);
                break;

            case TokenKind.DotDot:
                Step();
                if (!ParseExpr(out expr))
                    return false;
                expr = Expr.Range(upper: expr);
                break;

            case TokenKind.Underscore:
                Step();
                expr = new WildCardExpr();
                break;

            case TokenKind.KeywordTrue:
                Step();
                expr = Expr.Bool(true);
                break;

            case TokenKind.KeywordFalse:
                Step();
                expr = Expr.Bool(false);
                break;

            case TokenKind.IntLiteral:
                Step();
                expr = Expr.Int(_token.Int);
                break;

            case TokenKind.FloatLiteral:
                Step();
                expr = Expr.Float(_token.Double);
                break;

            case TokenKind.StringLiteral:
                Step();
                expr = Expr.String(_token.String!);
                break;

            case TokenKind.OpenParen:
                if (!ParseParenExpr(out expr))
                    return false;
                break;

            case TokenKind.OpenBrace:
                if (!ParseBraceExpr(out expr))
                    return false;
                break;

            case TokenKind.OpenBracket:
                if (!ParseBracketExpr(out expr))
                    return false;
                break;

            case TokenKind.KeywordIf:
                if (!ParseIfElseExpr(out var ite))
                    return false;
                expr = ite;
                break;

            case TokenKind.KeywordLet:
                if (!ParseLetExpr(out var let))
                    return false;
                expr = let;
                break;

            case TokenKind.Identifier:
                if (!ParseIdentExpr(out expr))
                    return false;
                break;

            default:
                return Error();
        }

        return true;
    }

    /// <summary>
    /// Parses a function call or generator call
    /// </summary>
    private bool ParseIdentExpr(out IExpr result)
    {
        result = Expr.Null;

        if (!ParseString(out var name))
            return false;

        // Simple identifier
        if (!Skip(TokenKind.OpenParen))
        {
            result = Expr.Ident(name);
            return true;
        }

        // Parse the initial comma sep args so
        var args = new List<IExpr>();
        while (true)
        {
            if (!ParseExpr(out var expr))
                return false;
            args.Add(expr);
            if (!Skip(TokenKind.Comma))
                break;
        }

        // Standard call `max(1,2)`
        if (!Skip(TokenKind.KeywordIn))
        {
            result = new CallExpr { Name = name, Args = args };
            return Expect(TokenKind.CloseParen);
        }

        return true;
    }

    /// <summary>
    /// Parse anything that starts with a '[', which will
    /// be an array or array comprehension
    /// </summary>
    /// <mzn>[1,2,3]</mzn>
    /// <mzn>[ x | x in [a,b,c]]</mzn>
    private bool ParseBracketExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Skip(TokenKind.OpenBracket))
            return false;

        IExpr element;

        // Empty Array
        if (_token.Kind is TokenKind.CloseBracket)
        {
            result = new Array1DLit();
            return Expect(TokenKind.CloseBracket);
        }

        // 2D array literal [| 1, 2, 3 | 4, 5, 6 |]
        if (Skip(TokenKind.Pipe))
        {
            var arr2D = new Array2DLit();
            result = arr2D;
            if (Skip(TokenKind.Pipe))
                return Expect(TokenKind.CloseBracket);

            array_2d_row:
            arr2D.I = 0;
            while (_token.Kind is not TokenKind.Pipe)
            {
                if (!ParseExpr(out element))
                    return false;
                arr2D.Elements.Add(element);
                arr2D.I++;
                if (!Skip(TokenKind.Comma))
                    break;
            }

            arr2D.J++;
            Expect(TokenKind.Pipe);
            if (Skip(TokenKind.CloseBracket))
                return true;
            goto array_2d_row;
        }

        // 1D array literal
        // `[1, 2, 3]`

        // Parse the first element
        if (!ParseExpr(out element))
            return false;

        // Array comprehension
        if (Skip(TokenKind.Pipe))
        {
            var comp = new CompExpr
            {
                Yields = element,
                IsSet = false,
                From = new List<GeneratorExpr>()
            };
            result = comp;
            if (!ParseGenerators(comp.From))
                return false;
            return Expect(TokenKind.CloseBracket);
        }

        // 1D Array literal
        var arr = new Array1DLit();
        result = arr;
        arr.Elements.Add(element);
        while (_token.Kind is not TokenKind.CloseBracket)
        {
            if (!ParseExpr(out element))
                return false;
            arr.Elements.Add(element);
            if (!Skip(TokenKind.Comma))
                break;
        }

        return Expect(TokenKind.CloseBracket);
    }

    /// <summary>
    /// Parse anything that starts with a '{', this could be a
    /// set literal or set comprehension
    /// </summary>
    /// <mzn>{1,2,3}</mzn>
    /// <mzn>{ x | x in [1,2,3]}</mzn>
    private bool ParseBraceExpr(out IExpr result)
    {
        result = Expr.Null;
        if (!Skip(TokenKind.OpenBrace))
            return false;

        // Empty Set
        if (_token.Kind is TokenKind.CloseBrace)
        {
            result = new SetLit();
            return Expect(TokenKind.CloseBrace);
        }

        // Parse first expression
        if (!ParseExpr(out var element))
            return false;

        // Set comprehension
        if (Skip(TokenKind.Pipe))
        {
            var comp = new CompExpr { Yields = element, IsSet = true };
            if (!ParseGenerators(comp.From))
                return false;

            result = comp;
            return Expect(TokenKind.CloseBrace);
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
        return Expect(TokenKind.CloseBrace);
    }

    private bool ParseGenerators(List<GeneratorExpr> generators)
    {
        return Error();
    }

    private bool ParseLetExpr(out LetExpr let)
    {
        let = default;

        if (!Expect(TokenKind.KeywordLet))
            return false;

        if (!Expect(TokenKind.OpenBrace))
            return false;

        let = new LetExpr();
        while (_token.Kind is not TokenKind.CloseBrace)
        {
            if (!ParseType(out var type))
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
    private bool ParseParenExpr(out IExpr result)
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
}
