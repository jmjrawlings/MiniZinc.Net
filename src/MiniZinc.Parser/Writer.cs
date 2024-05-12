namespace MiniZinc.Parser;

using System.Text;
using Ast;

public sealed class Writer
{
    const char FWD_SLASH = '/';
    const char BACK_SLASH = '\\';
    const char STAR = '*';
    const char DELIMITER = ';';
    const char EQUAL = '=';
    const char OPEN_CHEVRON = '<';
    const char CLOSE_CHEVRON = '>';
    const char UP_CHEVRON = '^';
    const char DOT = '.';
    const char PLUS = '+';
    const char DASH = '-';
    const char TILDE = '~';
    const char DOLLAR = '$';
    const char OPEN_BRACKET = '[';
    const char CLOSE_BRACKET = ']';
    const char OPEN_PAREN = '(';
    const char CLOSE_PAREN = ')';
    const char OPEN_BRACE = '{';
    const char CLOSE_BRACE = '}';
    const char PIPE = '|';
    const char PERCENT = '%';
    const char UNDERSCORE = '_';
    const char COMMA = ',';
    const char EXCLAMATION = '!';
    const char SINGLE_QUOTE = '\'';
    const char DOUBLE_QUOTE = '"';
    const char BACKTICK = '`';
    const char SPACE = ' ';
    const char COLON = ':';
    const char NEWLINE = '\n';
    const char RETURN = '\r';
    const char EOF = '\uffff';
    const string ANNOTATION = "annotation";
    const string ANN = "ann";
    const string ANY = "any";
    const string ARRAY = "array";
    const string BOOL = "bool";
    const string CASE = "case";
    const string CONSTRAINT = "constraint";
    const string DEFAULT = "default";
    const string DIFF = "diff";
    const string DIV = "div";
    const string ELSE = "else";
    const string ELSEIF = "elseif";
    const string ENDIF = "endif";
    const string ENUM = "enum";
    const string FALSE = "false";
    const string FLOAT = "float";
    const string FUNCTION = "function";
    const string IF = "if";
    const string IN = "in";
    const string INCLUDE = "include";
    const string INT = "int";
    const string INTERSECT = "intersect";
    const string LET = "let";
    const string LIST = "list";
    const string MAXIMIZE = "maximize";
    const string MINIMIZE = "minimize";
    const string MOD = "mod";
    const string NOT = "not";
    const string OF = "of";
    const string OP = "op";
    const string OPT = "opt";
    const string OUTPUT = "output";
    const string PAR = "par";
    const string PREDICATE = "predicate";
    const string RECORD = "record";
    const string SATISFY = "satisfy";
    const string SET = "set";
    const string SOLVE = "solve";
    const string STRING = "string";
    const string SUBSET = "subset";
    const string SUPERSET = "superset";
    const string SYMDIFF = "symdiff";
    const string TEST = "test";
    const string THEN = "then";
    const string TRUE = "true";
    const string TUPLE = "tuple";
    const string TYPE = "type";
    const string UNION = "union";
    const string VAR = "var";
    const string WHERE = "where";
    const string XOR = "xor";
    const string EOL = ";";

    private readonly StringBuilder _sb;

    public Writer()
    {
        _sb = new StringBuilder();
    }

    private void WriteSpaced(char c)
    {
        Space();
        Write(c);
        Space();
    }

    private void WriteSpaced(string c)
    {
        Space();
        Write(c);
        Space();
    }

    private void Write(char c)
    {
        _sb.Append(c);
    }

    private void Write(in Token token)
    {
        _sb.Append(token.ToString());
    }

    private void Space()
    {
        _sb.Append(SPACE);
    }

    private void Write(string s)
    {
        _sb.Append(s);
    }

    void WriteSep<T>(IEnumerable<T>? exprs, Action<T> write, char sep = COMMA)
    {
        if (exprs is null)
            return;

        using var enumerator = exprs.GetEnumerator();
        bool first = enumerator.MoveNext();
        if (!first)
            return;
        write(enumerator.Current);
        while (enumerator.MoveNext())
        {
            Write(sep);
            write(enumerator.Current);
        }
    }

    void Write(int i) => _sb.Append(i);

    void Write(double f) => _sb.Append(f);

    void WriteExprs<T>(IEnumerable<T>? exprs, char sep = COMMA)
        where T : SyntaxNode => WriteSep(exprs, WriteExpr, sep);

    public void WriteOp(Operator? op)
    {
        switch (op)
        {
            case null:
                break;

            case Operator.Equivalent:
                Write(EQUAL);
                break;
            case Operator.Implies:
                Write(DASH);
                Write(CLOSE_CHEVRON);
                break;
            case Operator.ImpliedBy:
                Write(OPEN_CHEVRON);
                Write(DASH);
                break;
            case Operator.Or:
                Write(BACK_SLASH);
                Write(FWD_SLASH);
                break;
            case Operator.Xor:
                Write(XOR);
                break;
            case Operator.And:
                Write(FWD_SLASH);
                Write(BACK_SLASH);
                break;
            case Operator.LessThan:
                Write(OPEN_CHEVRON);
                break;
            case Operator.GreaterThan:
                Write(CLOSE_CHEVRON);
                break;
            case Operator.LessThanEqual:
                Write(OPEN_CHEVRON);
                Write(EQUAL);
                break;
            case Operator.GreaterThanEqual:
                Write(CLOSE_CHEVRON);
                Write(EQUAL);
                break;
            case Operator.Equal:
                Write(EQUAL);
                break;
            case Operator.NotEqual:
                Write(EXCLAMATION);
                Write(EQUAL);
                break;
            case Operator.In:
                Write(IN);
                break;
            case Operator.Subset:
                Write(SUBSET);
                break;
            case Operator.Superset:
                Write(SUPERSET);
                break;
            case Operator.Union:
                Write(UNION);
                break;
            case Operator.Diff:
                Write(DIFF);
                break;
            case Operator.SymDiff:
                Write(SYMDIFF);
                break;
            case Operator.Range:
                Write(DOT);
                Write(DOT);
                break;
            case Operator.Add:
                Write(PLUS);
                break;
            case Operator.Subtract:
                Write(DASH);
                break;
            case Operator.Multiply:
                Write(STAR);
                break;
            case Operator.Div:
                Write(DIV);
                break;
            case Operator.Mod:
                Write(MOD);
                break;
            case Operator.Divide:
                Write(FWD_SLASH);
                break;
            case Operator.Intersect:
                Write(INTERSECT);
                break;
            case Operator.Exponent:
                Write(UP_CHEVRON);
                break;
            case Operator.Default:
                Write(DEFAULT);
                break;
            case Operator.Concat:
                Write(PLUS);
                Write(PLUS);
                break;
            case Operator.Positive:
                Write(PLUS);
                break;
            case Operator.Negative:
                Write(DASH);
                break;
            case Operator.Not:
                Write(NOT);
                break;
            case Operator.TildeNotEqual:
                Write(TILDE);
                Write(EXCLAMATION);
                Write(EQUAL);
                break;
            case Operator.TildeEqual:
                Write(TILDE);
                Write(EQUAL);
                break;
            case Operator.TildeAdd:
                Write(TILDE);
                Write(PLUS);
                break;
            case Operator.TildeSubtract:
                Write(TILDE);
                Write(DASH);
                break;
            case Operator.TildeMultiply:
                Write(TILDE);
                Write(STAR);
                break;
        }
    }

    void WriteAnnotations(SyntaxNode node)
    {
        if (node.Annotations is not { } anns)
            return;

        foreach (var ann in anns)
        {
            Write(COLON);
            Write(COLON);
            WriteExpr(ann);
            Space();
        }
    }

    public void WriteExpr(SyntaxNode? expr)
    {
        if (expr is null)
            return;

        switch (expr)
        {
            case Array1DSyntax e:
                WriteArray1D(e);
                break;
            case Array2dSyntax e:
                WriteArray2D(e);
                break;
            case ArrayAccessSyntax e:
                WriteArrayAccess(e);
                break;
            case ArrayTypeSyntax e:
                break;
            case BinaryOperatorSyntax e:
                WriteExpr(e.Left);
                if (e.Name is { } name)
                {
                    Write(BACKTICK);
                    Write(BACKTICK);
                }
                else
                {
                    WriteOp(e.Op!);
                }

                WriteExpr(e.Right);
                break;
            case BoolSyntax boolLit:
                _sb.Append(boolLit.Value);
                break;
            case CallSyntax e:
                Write(e.Name);
                Write(OPEN_PAREN);
                WriteExprs(e.Args);
                Write(CLOSE_PAREN);
                break;
            case ComprehensionSyntax e:
                Write(e.IsSet ? OPEN_BRACE : OPEN_BRACKET);
                WriteExpr(e.Expr);
                Write(PIPE);
                WriteExprs(e.Generators);
                Write(e.IsSet ? CLOSE_BRACE : CLOSE_BRACKET);
                break;
            case ConstraintSyntax e:
                Write(CONSTRAINT);
                Space();
                WriteExpr(e.Expr);
                Write(EOL);
                break;
            case EmptyExpr e:
                Write(OPEN_CHEVRON);
                Write(CLOSE_CHEVRON);
                break;
            case EnumDeclarationSyntax e:
                Write(ENUM);
                Space();
                Write(e.Name);
                WriteAnnotations(e);
                if (e.Cases.Count == 0)
                {
                    Write(EOL);
                    break;
                }
                Write(EQUAL);
                WriteExprs(e.Cases);
                Write(EOL);
                break;
            case ExprType e:
                break;
            case FloatLiteralSyntax e:
                Write(e.Value);
                break;
            case GeneratorCallSyntax e:
                Write(e.Name);
                Write(OPEN_PAREN);
                WriteSep(e.Generators, WriteGenerator);
                Write(CLOSE_PAREN);
                Write(OPEN_PAREN);
                WriteExpr(e.Expr);
                Write(CLOSE_PAREN);
                break;
            case GeneratorSyntax e:
                WriteGenerator(e);
                break;
            case IdentifierSyntax e:
                Write(e.Token);
                break;
            case IfElseSyntax e:
                Write(IF);
                Space();
                WriteExpr(e.If);
                WriteSpaced(THEN);
                WriteExpr(e.Then);
                if (e.ElseIfs is { } cases)
                {
                    foreach (var (elseif, then) in cases)
                    {
                        WriteSpaced(ELSEIF);
                        WriteExpr(elseif);
                        WriteSpaced(THEN);
                        WriteExpr(then);
                    }
                }
                Write(ENDIF);
                break;
            case IntLiteralSyntax e:
                _sb.Append(e);
                break;
            case LetSyntax e:
                Write(LET);
                Write(OPEN_BRACE);
                var locals = e.Locals?.Select(x =>
                    x switch
                    {
                        ConstraintSyntax c => (SyntaxNode)c,
                        AssignmentSyntax a => a,
                        DeclarationSyntax v => v,
                        _ => null!
                    }
                );

                WriteExprs(locals);
                Write(CLOSE_BRACE);
                WriteSpaced(IN);
                WriteExpr(e.Body);
                Write(EOL);
                break;
            case RangeLiteralSyntax e:
                if (e.Lower is { } lower)
                    WriteExpr(lower);
                Write(DOT);
                Write(DOT);
                if (e.Upper is { } upper)
                    WriteExpr(upper);
                break;
            case RecordAccessSyntax e:
                WriteExpr(e.Expr);
                Write(DOT);
                Write(e.Field);
                break;
            case RecordLiteralSyntax e:
                WriteRecord(e);
                break;
            case RecordTypeSyntax e:
                Write(RECORD);
                Write(OPEN_PAREN);
                WriteParameters(e.Fields);
                Write(CLOSE_PAREN);
                break;
            case SetLiteralSyntax e:
                Write(OPEN_BRACE);
                WriteExprs(e.Elements);
                Write(CLOSE_BRACE);
                break;
            case StringLiteralSyntax s:
                Write(DOUBLE_QUOTE);
                Write(s);
                Write(DOUBLE_QUOTE);
                break;
            case TupleAccessSyntax e:
                WriteExpr(e.Expr);
                Write(DOT);
                Write(e.Field);
                break;
            case TupleLiteralSyntax e:
                Write(OPEN_PAREN);
                WriteExprs(e.Fields);
                Write(CLOSE_PAREN);
                break;
            case TupleTypeSyntax e:
                Write(TUPLE);
                Write(OPEN_PAREN);
                WriteExprs(e.Items);
                Write(CLOSE_PAREN);
                break;
            case TypeSyntax e:
                break;
            case UnaryOperatorSyntax e:
                WriteOp(e.Operator);
                WriteExpr(e.Expr);
                break;
            case DeclarationSyntax e:
                break;
            case WildCardExpr e:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expr));
        }
    }

    private void WriteParameters(List<ParameterSyntax> parameters) =>
        WriteSep(parameters, WriteParameter);

    private void WriteParameter(ParameterSyntax x)
    {
        WriteExpr(x.Type);
        if (x.Name is { } name)
        {
            Write(COLON);
            Write(name);
        }
        WriteAnnotations(x);
    }
    
    private void WriteArrayAccess(ArrayAccessSyntax e)
    {
        WriteExpr(e.Array);
        Write(OPEN_BRACKET);
        WriteExprs(e.Access);
        Write(CLOSE_BRACKET);
    }

    private void WriteRecord(RecordLiteralSyntax e)
    {
        Write(OPEN_PAREN);
        for (int i = 0; i < e.Fields.Count; i++)
        {
            var (name, expr) = e.Fields[i];
            Write(name);
            Write(COLON);
            WriteExpr(expr);
            if (i < e.Fields.Count - 1)
                Write(COMMA);
        }

        Write(CLOSE_PAREN);
    }

    private void WriteArray2D(Array2dSyntax expr)
    {
        Write(OPEN_BRACKET);
        Write(PIPE);
        int x = 0;

        for (int i = 0; i < expr.I; i++)
        {
            for (int j = 0; j < expr.J; j++)
            {
                var v = expr.Elements[x++];
                WriteExpr(v);
                if (j < expr.J - 1)
                    Write(COMMA);
            }
            Write(PIPE);
            if (i < expr.I - 1)
                Write(COMMA);
        }
        Write(CLOSE_BRACKET);
    }

    private void WriteArray1D(Array1DSyntax e)
    {
        Write(OPEN_BRACKET);
        WriteExprs(e.Elements);
        Write(CLOSE_BRACKET);
    }

    private void WriteIdent(IdentifierSyntax id) => _sb.Append(id);

    private void WriteGenerator(GeneratorSyntax gen)
    {
        WriteSep(gen.Names, WriteIdent);
    }

    public override string ToString() => _sb.ToString();

    public static string WriteNode(SyntaxNode node)
    {
        var writer = new Writer();
        writer.WriteExpr(node);
        var s = writer.ToString();
        return s;
    }
}

public static class NodeExtensions
{
    public static string Write(this SyntaxNode node)
    {
        var s = Writer.WriteNode(node);
        return s;
    }

    public static string Write(this IEnumerable<SyntaxNode> nodes, string sep = ",")
    {
        var s = String.Join(sep, nodes.Select(Write));
        return s;
    }
}
