namespace MiniZinc.Parser;

using System.Text;
using Syntax;

/// <summary>
/// Writes a minzinc model.
/// Intended to be used via the SyntaxNode extension method `node.Write()`
/// </summary>
public sealed class Writer
{
    private const char FWD_SLASH = '/';
    private const char BACK_SLASH = '\\';
    private const char STAR = '*';
    private const char DELIMITER = ';';
    private const char EQUAL = '=';
    private const char OPEN_CHEVRON = '<';
    private const char CLOSE_CHEVRON = '>';
    private const char UP_CHEVRON = '^';
    private const char DOT = '.';
    private const char PLUS = '+';
    private const char DASH = '-';
    private const char TILDE = '~';
    private const char DOLLAR = '$';
    private const char OPEN_BRACKET = '[';
    private const char CLOSE_BRACKET = ']';
    private const char OPEN_PAREN = '(';
    private const char CLOSE_PAREN = ')';
    private const char OPEN_BRACE = '{';
    private const char CLOSE_BRACE = '}';
    private const char PIPE = '|';
    private const char PERCENT = '%';
    private const char UNDERSCORE = '_';
    private const char COMMA = ',';
    private const char EXCLAMATION = '!';
    private const char SINGLE_QUOTE = '\'';
    private const char DOUBLE_QUOTE = '"';
    private const char BACKTICK = '`';
    private const char SPACE = ' ';
    private const char COLON = ':';
    private const char NEWLINE = '\n';
    private const char RETURN = '\r';
    private const char EOF = '\uffff';
    private const string ANNOTATION = "annotation";
    private const string ANN = "ann";
    private const string ANY = "any";
    private const string ARRAY = "array";
    private const string BOOL = "bool";
    private const string CASE = "case";
    private const string CONSTRAINT = "constraint";
    private const string DEFAULT = "default";
    private const string DIFF = "diff";
    private const string DIV = "div";
    private const string ELSE = "else";
    private const string ELSEIF = "elseif";
    private const string ENDIF = "endif";
    private const string ENUM = "enum";
    private const string FALSE = "false";
    private const string FLOAT = "float";
    private const string FUNCTION = "function";
    private const string IF = "if";
    private const string IN = "in";
    private const string INCLUDE = "include";
    private const string INT = "int";
    private const string INTERSECT = "intersect";
    private const string LET = "let";
    private const string LIST = "list";
    private const string MAXIMIZE = "maximize";
    private const string MINIMIZE = "minimize";
    private const string MOD = "mod";
    private const string NOT = "not";
    private const string OF = "of";
    private const string OP = "op";
    private const string OPT = "opt";
    private const string OUTPUT = "output";
    private const string PAR = "par";
    private const string PREDICATE = "predicate";
    private const string RECORD = "record";
    private const string SATISFY = "satisfy";
    private const string SET = "set";
    private const string SOLVE = "solve";
    private const string STRING = "string";
    private const string SUBSET = "subset";
    private const string SUPERSET = "superset";
    private const string SYMDIFF = "symdiff";
    private const string TEST = "test";
    private const string THEN = "then";
    private const string TRUE = "true";
    private const string TUPLE = "tuple";
    private const string TYPE = "type";
    private const string UNION = "union";
    private const string VAR = "var";
    private const string WHERE = "where";
    private const string XOR = "xor";
    private const char EOL = ';';
    private const string ANON_ENUM = "anon_enum";

    private readonly StringBuilder _sb;
    private readonly WriteOptions _options;
    private readonly bool _minify;
    private readonly bool _prettify;
    private int _indent;
    private int _tabSize;

    public Writer(WriteOptions? options = null, StringBuilder? stringBuilder = null)
    {
        _sb = stringBuilder ?? new StringBuilder();
        _options = options ?? WriteOptions.Default;
        _minify = _options.Minify;
        _prettify = _options.Prettify;
        _indent = 0;
        _tabSize = _options.TabSize;
    }

    public void Clear()
    {
        _sb.Clear();
        _indent = 0;
    }

    public void WriteData(IReadOnlyDictionary<string, ExpressionSyntax> data)
    {
        foreach (var kv in data)
        {
            var name = kv.Key;
            var expr = kv.Value;
            WriteString(name);
            WriteSpace();
            WriteChar('=');
            WriteSpace();
            WriteExpr(expr);
            EndStatement();
        }
    }

    public void WriteSyntax(SyntaxNode? syntax)
    {
        switch (syntax)
        {
            case null:
                break;

            case ExpressionSyntax expr:
                WriteExpr(expr);
                break;

            case StatementSyntax statement:
                WriteStatement(statement);
                break;

            case TypeSyntax type:
                WriteType(type);
                break;
        }
    }

    public void WriteStatement(StatementSyntax? syntax)
    {
        switch (syntax)
        {
            case null:
                return;

            case IncludeStatement e:
                WriteKeyword(INCLUDE);
                WriteChar(DOUBLE_QUOTE);
                WriteString(e.Path.StringValue);
                WriteChar(DOUBLE_QUOTE);
                break;

            case DeclareStatement e:
                WriteDeclare(e);
                break;

            case TypeAliasSyntax e:
                WriteKeyword(TYPE);
                WriteIdent(e.Identifier);
                WriteSpace();
                WriteChar(EQUAL);
                WriteSpace();
                WriteType(e.Type);
                break;

            case ConstraintStatement e:
                WriteKeyword(CONSTRAINT);
                Indent();
                Newline();
                WriteExpr(e.Expr);
                WriteAnnotations(e);
                Dedent();
                break;

            case SolveStatement e:
                WriteKeyword(SOLVE);
                WriteAnnotations(e);
                WriteSpace();
                switch (e.Method)
                {
                    case SolveMethod.Satisfy:
                        WriteString(SATISFY);
                        break;
                    case SolveMethod.Maximize:
                        WriteKeyword(MAXIMIZE);
                        Indent();
                        Newline();
                        WriteExpr(e.Objective);
                        Dedent();
                        break;
                    case SolveMethod.Minimize:
                        WriteKeyword(MINIMIZE);
                        Indent();
                        Newline();
                        WriteExpr(e.Objective);
                        Dedent();
                        break;
                }
                break;

            case AssignmentSyntax e:
                WriteIdent(e.Identifier);
                WriteSpace();
                WriteChar(EQUAL);
                WriteSpace();
                WriteExpr(e.Expr);
                break;

            case OutputStatement e when _options.SkipOutput:
                break;

            case OutputStatement e:
                WriteString(OUTPUT);
                WriteSpace();
                WriteExpr(e.Expr);
                break;

            default:
                throw new Exception(syntax.GetType().ToString());
        }
        EndStatement();
    }

    public void WriteExpr(ExpressionSyntax? expr, int? prec = null)
    {
        switch (expr)
        {
            case null:
                break;

            case Array1dSyntax e:
                WriteChar(OPEN_BRACKET);
                WriteSep(e.Elements, WriteExpr);
                WriteChar(CLOSE_BRACKET);
                break;

            case Array2dSyntax e:
                WriteArray2d(e);
                break;

            case Array3dSyntax e:
                WriteArray3d(e);
                break;

            case ArrayAccessSyntax e:
                WriteExpr(e.Array);
                WriteChar(OPEN_BRACKET);
                WriteSep(e.Access, WriteExpr);
                WriteChar(CLOSE_BRACKET);
                break;

            case BinaryOperatorSyntax e:
                WriteBinOp(e, prec);
                break;

            case IntLiteralSyntax e:
                _sb.Append(e.Value);
                break;

            case BoolLiteralSyntax e:
                WriteString(e.Value ? TRUE : FALSE);
                WriteAnnotations(e);
                break;

            case FloatLiteralSyntax e:
                _sb.Append(e.Value);
                break;

            case StringLiteralSyntax e:
                WriteChar(DOUBLE_QUOTE);
                WriteString(e.Value);
                WriteChar(DOUBLE_QUOTE);
                break;

            case CallSyntax e:
                WriteString(e.Name);
                WriteChar(OPEN_PAREN);
                WriteSep(e.Args, WriteExpr);
                WriteChar(CLOSE_PAREN);
                WriteAnnotations(e);
                break;

            case ComprehensionSyntax e:
                WriteChar(e.IsSet ? OPEN_BRACE : OPEN_BRACKET);
                WriteExpr(e.Expr);
                WriteChar(PIPE);
                WriteSep(e.Generators, WriteExpr);
                WriteChar(e.IsSet ? CLOSE_BRACE : CLOSE_BRACKET);
                WriteAnnotations(e);
                break;

            case EmptyLiteralSyntax e:
                WriteChar(OPEN_CHEVRON);
                WriteChar(CLOSE_CHEVRON);
                WriteAnnotations(e);
                break;

            case GeneratorCallSyntax e:
                WriteString(e.Name);
                WriteChar(OPEN_PAREN);
                WriteSep(e.Generators, WriteGenerator);
                WriteChar(CLOSE_PAREN);
                WriteChar(OPEN_PAREN);
                WriteExpr(e.Expr);
                WriteChar(CLOSE_PAREN);
                WriteAnnotations(e);
                break;

            case GeneratorSyntax e:
                WriteGenerator(e);
                break;

            case IdentifierSyntax e:
                // Could be (Quoted / Normal / Keyword)
                WriteString(e.ToString());
                WriteAnnotations(e);
                break;

            case IfThenSyntax e:
                WriteString(IF);
                WriteSpace();
                WriteExpr(e.If);
                WriteKeywordSpaced(THEN);
                WriteExpr(e.Then);
                if (e.ElseIfs is { } cases)
                {
                    foreach (var (elseif, then) in cases)
                    {
                        WriteKeywordSpaced(ELSEIF);
                        WriteExpr(elseif);
                        WriteKeywordSpaced(THEN);
                        WriteExpr(then);
                    }
                }

                if (e.Else is { } @else)
                {
                    WriteSpace();
                    WriteString(ELSE);
                    WriteSpace();
                    WriteExpr(@else);
                }

                WriteSpace();
                WriteString(ENDIF);
                break;

            case LetSyntax e:
                WriteString(LET);
                WriteChar(OPEN_BRACE);
                if (e.Locals is { } locals)
                    foreach (var local in locals)
                        WriteStatement((StatementSyntax)local);

                WriteChar(CLOSE_BRACE);
                WriteKeywordSpaced(IN);
                WriteExpr(e.Body);
                break;

            case RangeLiteralSyntax e:
                if (e.Lower is { } lower)
                    WriteExpr(lower);
                if (!e.LowerIncusive)
                    WriteChar(OPEN_CHEVRON);
                WriteChar(DOT);
                WriteChar(DOT);
                if (!e.UpperInclusive)
                    WriteChar(OPEN_CHEVRON);
                if (e.Upper is { } upper)
                    WriteExpr(upper);
                WriteAnnotations(e);
                break;

            case RecordAccessSyntax e:
                WriteExpr(e.Expr);
                WriteChar(DOT);
                WriteString(e.Field.StringValue);
                WriteAnnotations(e);
                break;

            case RecordLiteralSyntax e:
                WriteChar(OPEN_PAREN);
                for (int i = 0; i < e.Fields.Count; i++)
                {
                    var (name, body) = e.Fields[i];
                    WriteIdent(name);
                    WriteChar(COLON);
                    WriteExpr(body);
                    if (i < e.Fields.Count - 1)
                        WriteChar(COMMA);
                }

                WriteChar(CLOSE_PAREN);
                break;

            case SetLiteralSyntax e:
                WriteChar(OPEN_BRACE);
                WriteSep(e.Elements, WriteExpr);
                WriteChar(CLOSE_BRACE);
                WriteAnnotations(e);
                break;

            case TupleAccessSyntax e:
                WriteExpr(e.Expr);
                WriteChar(DOT);
                _sb.Append(e.Index);
                WriteAnnotations(e);
                break;

            case TupleLiteralSyntax e:
                WriteChar(OPEN_PAREN);
                WriteSep(e.Fields, WriteExpr);
                WriteChar(COMMA);
                WriteChar(CLOSE_PAREN);
                WriteAnnotations(e);
                break;

            case UnaryOperatorSyntax e:
                WriteOperator(e.Operator);
                WriteChar(SPACE);
                if (e.Expr is BinaryOperatorSyntax)
                {
                    WriteChar(OPEN_PAREN);
                    WriteExpr(e.Expr, prec: 0);
                    WriteChar(CLOSE_PAREN);
                }
                else
                {
                    WriteExpr(e.Expr, prec: 0);
                }
                break;

            case IndexAndNode e:
                WriteExpr(e.Index);
                WriteChar(COLON);
                WriteExpr(e.Value);
                break;

            default:
                throw new Exception(expr.GetType().ToString());
        }
    }

    private void WriteDeclare(DeclareStatement e)
    {
        switch (e.Kind)
        {
            case DeclareKind.Function:
                WriteKeyword(FUNCTION);
                WriteType(e.Type!);
                WriteChar(COLON);
                break;
            case DeclareKind.Annotation:
                WriteKeyword(ANNOTATION);
                break;
            case DeclareKind.Test:
                WriteKeyword(TEST);
                break;
            case DeclareKind.Predicate:
                WriteKeyword(PREDICATE);
                break;
            case DeclareKind.Enum:
                WriteKeyword(ENUM);
                break;
            case DeclareKind.TypeAlias:
                WriteKeyword(TYPE);
                break;
            default:
                WriteType(e.Type);
                WriteChar(COLON);
                break;
        }

        WriteSpace();
        WriteIdent(e.Identifier);
        if (e.Parameters is { } parameters)
            WriteParameters(e.Parameters);

        if (e.Ann is { } ann)
        {
            WriteSpace();
            WriteString(ANN);
            WriteChar(':');
            WriteSpace();
            WriteIdent(ann);
            WriteSpace();
        }

        WriteAnnotations(e);

        if (e.Body is { } body)
        {
            WriteSpace();
            WriteChar(EQUAL);
            WriteSpace();
            WriteExpr(body);
        }
    }

    private void WriteArray3d(Array3dSyntax arr)
    {
        WriteString("[|");
        var array = arr.Elements;
        var index = -1;
        for (int i = 0; i < arr.I; i++)
        {
            WriteChar(PIPE);
            for (int j = 0; j < arr.J; j++)
            {
                for (int k = 0; k < arr.K; k++)
                {
                    var v = array[index++];
                    WriteExpr(v);
                    if (k + 1 < arr.K)
                        WriteChar(COMMA);
                }
                WriteChar(PIPE);
            }

            if (i + 1 < arr.I)
                WriteChar(COMMA);
        }
        WriteString("|]");
    }

    public void WriteModel(ModelSyntax e)
    {
        IEnumerable<StatementSyntax> statements;
        if (_prettify)
        {
            var sorted = new List<StatementSyntax>(e.Statements);
            sorted.Sort(_prettyPrintComparer);
            statements = sorted;
        }
        else
        {
            statements = e.Statements;
        }
        foreach (var statement in statements)
            WriteStatement(statement);

        return;
    }

    static PrettyPrintNodeComparer _prettyPrintComparer = new();

    /// <summary>
    /// Used for ordering nodes for pretty printing
    /// </summary>
    class PrettyPrintNodeComparer : IComparer<StatementSyntax>
    {
        static int Order(SyntaxNode? node) =>
            node switch
            {
                IncludeStatement => 0,
                DeclareStatement => 1,
                AssignmentSyntax => 1,
                ConstraintStatement => 2,
                OutputStatement => 4,
                SolveStatement => 3,
                _ => 10
            };

        public int Compare(StatementSyntax? x, StatementSyntax? y)
        {
            int i = Order(x);
            int j = Order(y);
            return i.CompareTo(j);
        }
    }

    private void WriteBinOp(BinaryOperatorSyntax e, int? precedence = null)
    {
        var (op, assoc, prec) = Parser.Precedence(e.Infix.Kind);
        var bracketed = prec < precedence;
        if (bracketed)
            WriteChar(OPEN_PAREN);

        WriteExpr(e.Left, assoc == Assoc.Left ? prec : prec + 1);
        WriteSpace();

        if (op is Operator.Identifier)
        {
            WriteChar(BACKTICK);
            WriteString(e.Infix.StringValue);
            WriteChar(BACKTICK);
        }
        else
        {
            WriteOperator(op);
        }
        WriteSpace();
        WriteExpr(e.Right, assoc == Assoc.Right ? prec : prec + 1);
        if (bracketed)
            WriteChar(CLOSE_PAREN);
    }

    void WriteType(TypeSyntax type, int? prec = null)
    {
        if (type.Var)
            WriteKeyword(VAR);

        if (type.Opt)
            WriteKeyword(OPT);

        switch (type)
        {
            case ArrayTypeSyntax e:
                WriteString(ARRAY);
                WriteChar(OPEN_BRACKET);
                WriteSep(e.Dimensions, WriteType);
                WriteChar(CLOSE_BRACKET);
                WriteSpace();
                WriteKeyword(OF);
                WriteType(e.Items);
                break;

            case CompositeTypeSyntax e:
                WriteSep(e.Types, WriteType, sep: "++");
                break;

            case ExprType e:
                WriteExpr(e.Expr);
                break;

            case ListTypeSyntax e:
                WriteKeyword(LIST);
                WriteKeyword(OF);
                WriteType(e.Items);
                break;

            case IdentifierTypeSyntax e:
                WriteIdent(e.Identifier);
                break;

            case RecordTypeSyntax e:
                WriteString(RECORD);
                WriteParameters(e.Fields);
                break;

            case SetTypeSyntax e:
                WriteString(SET);
                WriteKeywordSpaced(OF);
                WriteType(e.Items);
                break;

            case TupleTypeSyntax e:
                WriteString(TUPLE);
                WriteChar(OPEN_PAREN);
                WriteSep(e.Items, WriteType);
                WriteChar(CLOSE_PAREN);
                break;

            case { Kind: TypeKind.Bool }:
                WriteString(BOOL);
                break;

            case { Kind: TypeKind.Float }:
                WriteString(FLOAT);
                break;

            case { Kind: TypeKind.Int }:
                WriteString(INT);
                break;

            case { Kind: TypeKind.String }:
                WriteString(STRING);
                break;

            case { Kind: TypeKind.Annotation }:
                WriteString(ANNOTATION);
                break;

            case { Kind: TypeKind.Ann }:
                WriteString(ANN);
                break;

            case { Kind: TypeKind.Any }:
                WriteString(ANY);
                break;

            default:
                throw new NotImplementedException(type.ToString());
        }
    }

    void WriteParameters(IReadOnlyList<ParameterSyntax>? parameters)
    {
        WriteChar(OPEN_PAREN);
        WriteSep(parameters, WriteParameter);
        WriteChar(CLOSE_PAREN);
    }

    void WriteParameter(ParameterSyntax x, int? precedence = null)
    {
        WriteType(x.Type);
        if (x.Identifier is { } name)
        {
            WriteChar(COLON);
            WriteIdent(name);
        }
        WriteAnnotations(x);
    }

    void WriteArrayAccess(ArrayAccessSyntax e) { }

    void WriteGenerator(GeneratorSyntax gen, int? precedence = null)
    {
        WriteSep(gen.Names, WriteExpr);
        WriteKeywordSpaced(IN);
        WriteExpr(gen.From);
        if (gen.Where is { } cond)
        {
            WriteKeywordSpaced(WHERE);
            WriteExpr(cond);
        }
        WriteAnnotations(gen);
    }

    void WriteKeywordSpaced(string c)
    {
        _sb.Append(SPACE);
        WriteString(c);
        _sb.Append(SPACE);
    }

    void WriteChar(char c)
    {
        _sb.Append(c);
    }

    void Newline()
    {
        if (_minify)
            return;

        _sb.AppendLine();
        _sb.Append(SPACE, _tabSize * _indent);
    }

    void Indent()
    {
        _indent++;
    }

    void Dedent()
    {
        _indent--;
    }

    void WriteArray2d(Array2dSyntax arr)
    {
        WriteChar(OPEN_BRACKET);
        WriteChar(PIPE);
        switch (arr.RowIndexed, arr.ColIndexed)
        {
            case (false, false):
                int x = 0;
                for (int i = 0; i < arr.I; i++)
                {
                    for (int j = 0; j < arr.J; j++)
                    {
                        var v = arr.Elements[x++];
                        WriteExpr(v);
                        if (j < arr.J - 1)
                            WriteChar(COMMA);
                    }
                    WriteChar(PIPE);
                }

                break;
        }
        WriteChar(CLOSE_BRACKET);
    }

    void WriteIdent(IdentifierSyntax id)
    {
        switch (id.Kind)
        {
            case TokenKind.IDENTIFIER:
                _sb.Append(id);
                break;
            case TokenKind.GENERIC:
                _sb.Append(id);
                break;
            case TokenKind.GENERIC_SEQUENCE:
                _sb.Append(id);
                break;
            default:
                throw new Exception();
        }
    }

    /// <summary>
    /// Writes (optional) whitespace.
    /// This will be ignored when writing with
    /// `minify` enabled.
    /// </summary>
    void WriteSpace()
    {
        // We assume that 'WriteSpace' always indicates
        // optional whitespace and therefore
        if (_sb.Length is 0)
            return;
        if (_minify)
            return;
        if (_sb[^1] is not SPACE)
            _sb.Append(SPACE);
    }

    void WriteString(string s)
    {
        _sb.Append(s);
    }

    void EndStatement()
    {
        WriteChar(EOL);
        Newline();
    }

    void WriteKeyword(string s)
    {
        _sb.Append(s);
        _sb.Append(SPACE);
    }

    void WriteSep<T>(IEnumerable<T>? nodes, Action<T, int?> write, int prec = 0, string sep = ",")
        where T : SyntaxNode
    {
        if (nodes is null)
            return;

        using var enumerator = nodes.GetEnumerator();
        bool first = enumerator.MoveNext();
        if (!first)
            return;

        write(enumerator.Current, prec);
        while (enumerator.MoveNext())
        {
            WriteString(sep);
            WriteSpace();
            write(enumerator.Current, prec);
        }
    }

    public void WriteOperator(Operator? op)
    {
        switch (op)
        {
            case null:
                break;

            case Operator.Equivalent:
                WriteChar(OPEN_CHEVRON);
                WriteChar(DASH);
                WriteChar(CLOSE_CHEVRON);
                break;
            case Operator.Implies:
                WriteChar(DASH);
                WriteChar(CLOSE_CHEVRON);
                break;
            case Operator.ImpliedBy:
                WriteChar(OPEN_CHEVRON);
                WriteChar(DASH);
                break;
            case Operator.Or:
                WriteChar(BACK_SLASH);
                WriteChar(FWD_SLASH);
                break;
            case Operator.Xor:
                WriteString(XOR);
                break;
            case Operator.And:
                WriteChar(FWD_SLASH);
                WriteChar(BACK_SLASH);
                break;
            case Operator.LessThan:
                WriteChar(OPEN_CHEVRON);
                break;
            case Operator.GreaterThan:
                WriteChar(CLOSE_CHEVRON);
                break;
            case Operator.LessThanEqual:
                WriteChar(OPEN_CHEVRON);
                WriteChar(EQUAL);
                break;
            case Operator.GreaterThanEqual:
                WriteChar(CLOSE_CHEVRON);
                WriteChar(EQUAL);
                break;
            case Operator.Equal:
                WriteChar(EQUAL);
                WriteChar(EQUAL);
                break;
            case Operator.NotEqual:
                WriteChar(EXCLAMATION);
                WriteChar(EQUAL);
                break;
            case Operator.In:
                WriteString(IN);
                break;
            case Operator.Subset:
                WriteString(SUBSET);
                break;
            case Operator.Superset:
                WriteString(SUPERSET);
                break;
            case Operator.Union:
                WriteString(UNION);
                break;
            case Operator.Diff:
                WriteString(DIFF);
                break;
            case Operator.SymDiff:
                WriteString(SYMDIFF);
                break;
            case Operator.Add:
                WriteChar(PLUS);
                break;
            case Operator.Subtract:
                WriteChar(DASH);
                break;
            case Operator.Multiply:
                WriteChar(STAR);
                break;
            case Operator.Div:
                WriteString(DIV);
                break;
            case Operator.Mod:
                WriteString(MOD);
                break;
            case Operator.Divide:
                WriteChar(FWD_SLASH);
                break;
            case Operator.Intersect:
                WriteString(INTERSECT);
                break;
            case Operator.Exponent:
                WriteChar(UP_CHEVRON);
                break;
            case Operator.Default:
                WriteString(DEFAULT);
                break;
            case Operator.Concat:
                WriteChar(PLUS);
                WriteChar(PLUS);
                break;
            case Operator.Positive:
                WriteChar(PLUS);
                break;
            case Operator.Negative:
                WriteChar(DASH);
                break;
            case Operator.Not:
                WriteString(NOT);
                break;
            case Operator.TildeNotEqual:
                WriteChar(TILDE);
                WriteChar(EXCLAMATION);
                WriteChar(EQUAL);
                break;
            case Operator.TildeEqual:
                WriteChar(TILDE);
                WriteChar(EQUAL);
                break;
            case Operator.TildeAdd:
                WriteChar(TILDE);
                WriteChar(PLUS);
                break;
            case Operator.TildeSubtract:
                WriteChar(TILDE);
                WriteChar(DASH);
                break;
            case Operator.TildeMultiply:
                WriteChar(TILDE);
                WriteChar(STAR);
                break;
        }
    }

    void WriteAnnotations(SyntaxNode node)
    {
        if (node.Annotations is not { Count: > 0 } anns)
            return;

        foreach (var ann in anns)
        {
            WriteSpace();
            WriteChar(COLON);
            WriteChar(COLON);
            WriteExpr(ann);
        }

        return;
    }

    public override string ToString()
    {
        var mzn = _sb.ToString();
        return mzn;
    }
}
