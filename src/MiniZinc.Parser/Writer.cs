namespace MiniZinc.Parser;

using System.Text;
using Syntax;

/// <summary>
/// Writes a MiniZinc model.
/// Intended to be used via the SyntaxNode extension method `node.Write()`
/// </summary>
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
    const char EOL = ';';
    const string ANON_ENUM = "anon_enum";

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

    public void WriteData(MiniZincData data)
    {
        foreach (var (name, value) in data)
        {
            NewLine();
            WriteString(name);
            WriteSpace();
            WriteChar('=');
            WriteSpace();
            WriteValue(value);
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
        NewLine();
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
                WriteToken(e.Name);
                WriteSpace();
                WriteChar(EQUAL);
                WriteSpace();
                WriteType(e.Type);
                break;

            case ConstraintStatement e:
                WriteKeyword(CONSTRAINT);
                Indent();
                NewLine();
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
                        NewLine();
                        WriteExpr(e.Objective);
                        Dedent();
                        break;
                    case SolveMethod.Minimize:
                        WriteKeyword(MINIMIZE);
                        Indent();
                        NewLine();
                        WriteExpr(e.Objective);
                        Dedent();
                        break;
                }
                break;

            case AssignStatement e:
                WriteToken(e.Name);
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

            case IntLiteralSyntax i:
                WriteInt(i);
                break;

            case FloatLiteralSyntax f:
                WriteDecimal(f);
                break;

            case BoolLiteralSyntax b:
                WriteBool(b);
                break;

            case StringLiteralSyntax s:
                WriteChar(DOUBLE_QUOTE);
                WriteString(s);
                WriteChar(DOUBLE_QUOTE);
                break;

            case EmptyLiteralSyntax:
                WriteChar(OPEN_CHEVRON);
                WriteChar(CLOSE_CHEVRON);
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

            case CallSyntax e:
                WriteToken(e.Name);
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

            case GeneratorCallSyntax e:
                WriteToken(e.Name);
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
                    WriteToken(name);
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
                WriteToken(e.Start);
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

    internal void WriteValue(DataNode dataSyntax)
    {
        switch (dataSyntax)
        {
            case IntArrayData x:
                WriteValues(x, WriteInt);
                break;
            case BoolArrayData x:
                WriteValues(x, WriteBool);
                break;
            case FloatArrayData x:
                WriteValues(x, WriteDecimal);
                break;
            case StringArrayData x:
                WriteValues(x, WriteString);
                break;
            case ArrayData x:
                WriteValues(x, WriteValue);
                break;
            case BoolData x:
                WriteBool(x);
                break;
            case EmptyData x:
                WriteChar(OPEN_CHEVRON);
                WriteChar(CLOSE_CHEVRON);
                break;
            case IntData x:
                WriteInt(x);
                break;
            case FloatData x:
                WriteDecimal(x);
                break;
            case RecordData x:
                WriteValues(
                    x,
                    pair =>
                    {
                        WriteString(pair.Key);
                        WriteChar(COLON);
                        WriteValue(pair.Value);
                    },
                    before: OPEN_PAREN,
                    after: CLOSE_PAREN
                );
                break;
            case IntSetData x:
                WriteValues(x, WriteInt, before: OPEN_BRACE, after: CLOSE_BRACE);
                break;
            case FloatSetData x:
                WriteValues(x, WriteDecimal, before: OPEN_BRACE, after: CLOSE_BRACE);
                break;
            case BoolSetData x:
                WriteValues(x, WriteBool, before: OPEN_BRACE, after: CLOSE_BRACE);
                break;
            case SetData x:
                WriteValues(x, WriteValue, before: OPEN_BRACE, after: CLOSE_BRACE);
                break;
            case StringData x:
                WriteChar(DOUBLE_QUOTE);
                WriteString(x.Value);
                WriteChar(DOUBLE_QUOTE);
                break;
            case TupleData x:
                WriteChar(OPEN_PAREN);
                foreach (var item in x)
                {
                    WriteValue(item);
                    WriteChar(COMMA);
                }
                WriteChar(CLOSE_PAREN);
                break;
            case FloatRangeData x:
                WriteDecimal(x.Lower);
                WriteChar(DOT);
                WriteChar(DOT);
                WriteDecimal(x.Upper);
                break;
            case IntRangeData x:
                WriteInt(x.Lower);
                WriteChar(DOT);
                WriteChar(DOT);
                WriteInt(x.Upper);
                break;
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
                WriteType(e.Type!);
                WriteChar(COLON);
                break;
        }

        WriteSpace();
        WriteToken(e.Name);
        if (e.Parameters is { } parameters)
            WriteParameters(parameters);

        if (e.Ann is { } ann)
        {
            WriteSpace();
            WriteString(ANN);
            WriteChar(':');
            WriteSpace();
            WriteToken(ann);
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
                AssignStatement => 1,
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
        var prec = Parser.Precedence(e.Operator);
        var assoc = Parser.Associativity(e.Operator);
        var bracketed = prec < precedence;
        if (bracketed)
            WriteChar(OPEN_PAREN);

        WriteExpr(e.Left, assoc == Parser.Assoc.Left ? prec : prec + 1);
        WriteSpace();
        WriteToken(e.Infix);
        WriteSpace();
        WriteExpr(e.Right, assoc == Parser.Assoc.Right ? prec : prec + 1);
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
                WriteSep(e.Types, WriteType, sep: "++", spaced: true);
                break;

            case ExprTypeSyntax e:
                WriteExpr(e.Expr);
                break;

            case ListTypeSyntax e:
                WriteKeyword(LIST);
                WriteKeyword(OF);
                WriteType(e.Items);
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

            case { Kind: TypeKind.Identifier }:
                WriteToken(type.Name);
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

    void WriteParameter(ParameterSyntax param, int? precedence = null)
    {
        WriteType(param.Type);
        if (param.Name.Kind > TokenKind.ERROR)
        {
            WriteChar(COLON);
            WriteToken(param.Name);
        }
        WriteAnnotations(param);
    }

    void WriteToken(in Token token) => WriteString(token.ToString());

    void WriteArrayAccess(ArrayAccessSyntax e) { }

    void WriteGenerator(GeneratorSyntax gen, int? precedence = null)
    {
        WriteSep(gen.Names, (name, _) => WriteToken(name));
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

    void WriteInt(int i) => _sb.Append(i);

    void WriteDecimal(decimal d) => _sb.Append(d);

    void WriteBool(bool b) => _sb.Append(b ? TRUE : FALSE);

    void NewLine()
    {
        if (_minify)
            return;

        if (_sb.Length is 0)
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
            case TokenKind.IDENTIFIER_GENERIC:
                _sb.Append(id);
                break;
            case TokenKind.IDENTIFIER_GENERIC_SEQUENCE:
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
    }

    void WriteKeyword(string s)
    {
        _sb.Append(s);
        _sb.Append(SPACE);
    }

    void WriteValues<T>(
        IEnumerable<T> nodes,
        Action<T> write,
        char before = OPEN_BRACKET,
        string sep = ",",
        char after = CLOSE_BRACKET
    )
    {
        using var enumerator = nodes.GetEnumerator();
        bool first = enumerator.MoveNext();
        if (!first)
            return;

        write(enumerator.Current);
        while (enumerator.MoveNext())
        {
            WriteString(sep);
            WriteSpace();
            write(enumerator.Current);
        }
    }

    void WriteSep<T>(
        IEnumerable<T>? nodes,
        Action<T, int?> write,
        int prec = 0,
        string sep = ",",
        bool spaced = false
    )
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
            if (spaced)
                WriteSpace();
            WriteString(sep);
            if (spaced)
                WriteSpace();
            write(enumerator.Current, prec);
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
