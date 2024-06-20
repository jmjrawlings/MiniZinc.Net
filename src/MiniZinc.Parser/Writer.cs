namespace MiniZinc.Parser;

using System.Text;
using Syntax;

/// <summary>
/// Writes a minzinc model.
/// Intended to be used via the SyntaxNode extension method `node.Write()`
/// </summary>
internal sealed class Writer
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
    private const string EOL = ";";
    private const string ANON_ENUM = "anon_enum";

    private readonly StringBuilder _sb;
    private readonly WriteOptions _options;
    private readonly bool _minify;
    private readonly bool _prettify;
    private int _indent;
    private int _tabSize;

    public Writer(WriteOptions options)
    {
        _sb = new StringBuilder();
        _options = options;
        _minify = _options.Minify;
        _prettify = options.Prettify;
        _indent = 0;
        _tabSize = options.TabSize;
    }

    private void WriteNode(SyntaxNode? node, Assoc assoc = 0, int? prec = null)
    {
        if (node is null)
            return;

        switch (node)
        {
            case SyntaxTree e:
                WriteTree(e);
                break;

            case IncludeSyntax e:
                WriteSpace(INCLUDE);
                Write(DOUBLE_QUOTE);
                Write(e.Path.StringValue);
                Write(DOUBLE_QUOTE);
                EndStatement();
                break;

            case DeclarationSyntax e:
                if (e.IsFunction)
                    WriteSpace(FUNCTION);

                WriteType(e.Type);
                if (!e.IsAnnotation)
                    Write(COLON);

                Space();
                Write(e.Identifier);
                if (e.IsFunction)
                {
                    WriteParameters(e.Parameters);
                    if (e.Ann is { } ann)
                    {
                        Space();
                        Write(ANN);
                        Write(':');
                        Space();
                        Write(ann);
                        Space();
                    }
                }

                WriteAnnotations(e);

                if (e.Body is { } body)
                {
                    Write(EQUAL);
                    Space();
                    WriteNode(body);
                }
                EndStatement();
                break;

            case ConstraintSyntax e:
                WriteSpace(CONSTRAINT);
                Indent();
                Newline();
                WriteNode(e.Expr);
                WriteAnnotations(e);
                Dedent();
                EndStatement();
                break;

            case SolveSyntax e:
                WriteSpace(SOLVE);
                WriteAnnotations(e);
                switch (e.Method)
                {
                    case SolveMethod.Satisfy:
                        Write(SATISFY);
                        break;
                    case SolveMethod.Maximize:
                        WriteSpace(MAXIMIZE);
                        Indent();
                        Newline();
                        WriteNode(e.Objective);
                        Dedent();
                        break;
                    case SolveMethod.Minimize:
                        WriteSpace(MINIMIZE);
                        Indent();
                        Newline();
                        WriteNode(e.Objective);
                        Dedent();
                        break;
                }
                EndStatement();
                break;

            case AssignmentSyntax e:
                WriteAssignment(e);
                break;

            case OutputSyntax e when _options.SkipOutput:
                break;

            case OutputSyntax e:
                Write(OUTPUT);
                Space();
                WriteNode(e.Expr);
                EndStatement();
                break;

            case TypeAliasSyntax e:
                WriteAlias(e);
                break;

            case EnumDeclarationSyntax e:
                Write(ENUM);
                Space();
                Write(e.Identifier);
                WriteAnnotations(e);
                if (e.Cases.Count > 0)
                {
                    Spaced(EQUAL);
                    WriteSep(e.Cases, sep: " ++ ");
                }
                EndStatement();
                break;

            case TypeSyntax e:
                WriteType(e);
                break;

            case Array1DSyntax e:
                Write(OPEN_BRACKET);
                WriteSep(e.Elements);
                Write(CLOSE_BRACKET);
                break;

            case Array2dSyntax expr:
                Write(OPEN_BRACKET);
                Write(PIPE);
                int x = 0;
                for (int i = 0; i < expr.I; i++)
                {
                    for (int j = 0; j < expr.J; j++)
                    {
                        var v = expr.Elements[x++];
                        WriteNode(v);
                        if (j < expr.J - 1)
                            Write(COMMA);
                    }
                    Write(PIPE);
                    // if (i < expr.I - 1)
                    //     Write(COMMA);
                }
                Write(CLOSE_BRACKET);
                break;

            case Array3dSyntax e:
                Write("[|");
                var arr = e.Elements.ToArray();
                var index = -1;
                for (int i = 0; i < e.I; i++)
                {
                    Write(PIPE);
                    for (int j = 0; j < e.J; j++)
                    {
                        for (int k = 0; k < e.K; k++)
                        {
                            var v = arr[index++];
                            WriteNode(v);
                            if (k + 1 < e.K)
                                Write(COMMA);
                        }
                        Write(PIPE);
                    }

                    if (i + 1 < e.I)
                        Write(COMMA);
                }
                Write("|]");
                break;

            case ArrayAccessSyntax e:
                WriteNode(e.Array);
                Write(OPEN_BRACKET);
                WriteSep(e.Access);
                Write(CLOSE_BRACKET);
                break;

            case BinaryOperatorSyntax e:
                WriteBinOp(e, assoc, prec);
                break;

            case IntLiteralSyntax e:
                WriteInt(e.Value);
                break;

            case BoolLiteralSyntax e:
                WriteBool(e.Value);
                break;

            case FloatLiteralSyntax e:
                WriteFloat(e.Value);
                break;

            case StringLiteralSyntax e:
                Write(DOUBLE_QUOTE);
                Write(e.Value);
                Write(DOUBLE_QUOTE);
                break;

            case CallSyntax e:
                Write(e.Name);
                Write(OPEN_PAREN);
                WriteSep(e.Args);
                Write(CLOSE_PAREN);
                WriteAnnotations(e);
                break;

            case ComprehensionSyntax e:
                Write(e.IsSet ? OPEN_BRACE : OPEN_BRACKET);
                WriteNode(e.Expr);
                Write(PIPE);
                WriteSep(e.Generators);
                Write(e.IsSet ? CLOSE_BRACE : CLOSE_BRACKET);
                WriteAnnotations(e);
                break;

            case EmptyLiteralSyntax e:
                Write(OPEN_CHEVRON);
                Write(CLOSE_CHEVRON);
                WriteAnnotations(e);
                break;

            case EnumCasesSyntax e:
                switch (e.Type)
                {
                    case EnumCaseType.Anon:
                        Write(ANON_ENUM);
                        Write(OPEN_PAREN);
                        WriteNode(e.Expr);
                        Write(CLOSE_PAREN);
                        break;

                    case EnumCaseType.Underscore:
                        Write(UNDERSCORE);
                        Write(OPEN_PAREN);
                        WriteNode(e.Expr);
                        Write(CLOSE_PAREN);
                        break;

                    case EnumCaseType.Names:
                        Write(OPEN_BRACE);
                        WriteSep(e.Names);
                        Write(CLOSE_BRACE);
                        break;

                    case EnumCaseType.Complex:
                        Write(e.Constructor!);
                        Write(OPEN_PAREN);
                        WriteNode(e.Expr);
                        Write(CLOSE_PAREN);
                        break;
                }
                break;

            case GeneratorCallSyntax e:
                Write(e.Name);
                Write(OPEN_PAREN);
                WriteSep(e.Generators, WriteGenerator);
                Write(CLOSE_PAREN);
                Write(OPEN_PAREN);
                WriteNode(e.Expr);
                Write(CLOSE_PAREN);
                WriteAnnotations(e);
                break;

            case GeneratorSyntax e:
                WriteGenerator(e);
                break;

            case IdentifierSyntax e:
                // Could be (Quoted / Normal / Keyword)
                Write(e.ToString());
                WriteAnnotations(e);
                break;

            case IfElseSyntax e:
                Write(IF);
                Space();
                WriteNode(e.If);
                Spaced(THEN);
                WriteNode(e.Then);
                if (e.ElseIfs is { } cases)
                {
                    foreach (var (elseif, then) in cases)
                    {
                        Spaced(ELSEIF);
                        WriteNode(elseif);
                        Spaced(THEN);
                        WriteNode(then);
                    }
                }

                if (e.Else is { } @else)
                {
                    Space();
                    Write(ELSE);
                    Space();
                    WriteNode(@else);
                }

                Space();
                Write(ENDIF);
                break;

            case LetSyntax e:
                Write(LET);
                Write(OPEN_BRACE);
                if (e.Locals is { } locals)
                    foreach (var local in locals)
                        WriteNode((SyntaxNode)local);

                Write(CLOSE_BRACE);
                Spaced(IN);
                WriteNode(e.Body);
                break;

            case RangeLiteralSyntax e:
                if (e.Lower is { } lower)
                    WriteNode(lower);
                Write(DOT);
                Write(DOT);
                if (e.Upper is { } upper)
                    WriteNode(upper);
                WriteAnnotations(e);
                break;

            case RecordAccessSyntax e:
                WriteNode(e.Expr);
                Write(DOT);
                Write(e.Field.StringValue);
                WriteAnnotations(e);
                break;

            case RecordLiteralSyntax e:
                Write(OPEN_PAREN);
                for (int i = 0; i < e.Fields.Count; i++)
                {
                    var (name, expr) = e.Fields[i];
                    Write(name);
                    Write(COLON);
                    WriteNode(expr);
                    if (i < e.Fields.Count - 1)
                        Write(COMMA);
                }

                Write(CLOSE_PAREN);
                break;

            case SetLiteralSyntax e:
                Write(OPEN_BRACE);
                WriteSep(e.Elements);
                Write(CLOSE_BRACE);
                WriteAnnotations(e);
                break;

            case TupleAccessSyntax e:
                WriteNode(e.Expr);
                Write(DOT);
                WriteInt(e.Index);
                WriteAnnotations(e);
                break;

            case TupleLiteralSyntax e:
                Write(OPEN_PAREN);
                WriteSep(e.Fields);
                Write(COMMA);
                Write(CLOSE_PAREN);
                WriteAnnotations(e);
                break;

            case UnaryOperatorSyntax e:
                WriteOperator(e.Operator);
                Write(SPACE);
                WriteNode(e.Expr, Assoc.Left, prec: 0);
                break;

            case WildCardSyntax e:
                Write(UNDERSCORE);
                break;

            case IndexAndNode e:
                WriteNode(e.Index);
                Write(COLON);
                WriteNode(e.Value);
                break;

            default:
                throw new Exception(node.GetType().ToString());
        }
    }

    private void WriteFloat(decimal f)
    {
        _sb.Append(f);
    }

    private void WriteBool(bool b)
    {
        if (b)
            Write(TRUE);
        else
            Write(FALSE);
    }

    private void WriteInt(int i)
    {
        _sb.Append(i);
    }

    private void WriteAlias(TypeAliasSyntax e)
    {
        Write(TYPE);
        Space();
        Write(e.Identifier);
        Spaced(EQUAL);
        WriteNode(e.Type);
        EndStatement();
    }

    private void WriteAssignment(AssignmentSyntax e)
    {
        Write(e.Identifier);
        Spaced(EQUAL);
        WriteNode(e.Expr);
        EndStatement();
    }

    private void WriteTree(SyntaxTree e)
    {
        var nodes = e.Nodes;
        if (_prettify)
        {
            nodes = new List<SyntaxNode>(e.Nodes);
            nodes.Sort(_prettyPrintComparer);
        }

        foreach (var node in nodes)
            WriteNode(node);
        return;
    }

    static PrettyPrintNodeComparer _prettyPrintComparer = new();

    /// <summary>
    /// Used for ordering nodes for pretty printing
    /// </summary>
    class PrettyPrintNodeComparer : IComparer<SyntaxNode>
    {
        static int Order(SyntaxNode? node) =>
            node switch
            {
                IncludeSyntax => 0,
                DeclarationSyntax => 1,
                AssignmentSyntax => 1,
                ConstraintSyntax => 2,
                OutputSyntax => 4,
                SolveSyntax => 3,
                _ => 10
            };

        public int Compare(SyntaxNode? x, SyntaxNode? y)
        {
            int i = Order(x);
            int j = Order(y);
            return i.CompareTo(j);
        }
    }

    private void WriteBinOp(BinaryOperatorSyntax e, Assoc associativity = 0, int? precedence = null)
    {
        var (op, assoc, prec) = Parser.Precedence(e.Infix.Kind);

        var bracketed = associativity switch
        {
            Assoc.None when (assoc is Assoc.None) && prec == precedence => true,

            // We are on the right hand side of the tree
            Assoc.Left when prec >= precedence
                => true,
            Assoc.None when prec >= precedence => true,

            // We are on the left hand side of the tree
            Assoc.None when prec > precedence
                => true,
            Assoc.Right when prec > precedence => true,

            _ => false
        };

        if (bracketed)
            Write(OPEN_PAREN);

        // Write the left hand side of the tree
        WriteNode(e.Left, assoc is Assoc.None ? Assoc.None : Assoc.Right, prec);
        Space();

        if (op is Operator.Identifier)
        {
            Write(BACKTICK);
            Write(e.Infix.StringValue);
            Write(BACKTICK);
        }
        else
        {
            WriteOperator(op);
        }
        Space();
        WriteNode(e.Right, assoc is Assoc.None ? Assoc.None : Assoc.Left, prec);

        if (bracketed)
            Write(CLOSE_PAREN);
    }

    void WriteType(TypeSyntax type)
    {
        if (type.Var)
            WriteSpace(VAR);

        if (type.Opt)
            WriteSpace(OPT);

        switch (type)
        {
            case ArrayTypeSyntax e:
                Write(ARRAY);
                Write(OPEN_BRACKET);
                WriteSep(e.Dimensions, WriteNode);
                Write(CLOSE_BRACKET);
                Space();
                Write(OF);
                Space();
                WriteType(e.Items);
                break;

            case CompositeTypeSyntax e:
                WriteSep(e.Types, WriteNode, sep: " ++ ");
                break;

            case ExprType e:
                WriteNode(e.Expr);
                break;

            case ListTypeSyntax e:
                WriteSpace(LIST);
                WriteSpace(OF);
                WriteNode(e.Items);
                break;

            case NameTypeSyntax e:
                Write(e.Name);
                break;

            case RecordTypeSyntax e:
                Write(RECORD);
                WriteParameters(e.Fields);
                break;

            case SetTypeSyntax e:
                Write(SET);
                Spaced(OF);
                WriteNode(e.Items);
                break;

            case TupleTypeSyntax e:
                Write(TUPLE);
                Write(OPEN_PAREN);
                WriteSep(e.Items);
                Write(CLOSE_PAREN);
                break;

            case { Kind: TypeKind.Bool }:
                Write(BOOL);
                break;

            case { Kind: TypeKind.Float }:
                Write(FLOAT);
                break;

            case { Kind: TypeKind.Int }:
                Write(INT);
                break;

            case { Kind: TypeKind.String }:
                Write(STRING);
                break;

            case { Kind: TypeKind.Annotation }:
                Write(ANNOTATION);
                break;

            case { Kind: TypeKind.Ann }:
                Write(ANN);
                break;

            case { Kind: TypeKind.Any }:
                Write(ANY);
                break;

            default:
                throw new NotImplementedException(type.ToString());
        }
    }

    void WriteParameters(List<ParameterSyntax>? parameters)
    {
        Write(OPEN_PAREN);
        WriteSep(parameters, WriteParameter);
        Write(CLOSE_PAREN);
    }

    void WriteParameter(ParameterSyntax x, Assoc assoc, int? precedence = null)
    {
        WriteNode(x.Type);
        if (x.Name is { } name)
        {
            Write(COLON);
            Write(name);
        }
        WriteAnnotations(x);
    }

    void WriteArrayAccess(ArrayAccessSyntax e) { }

    void WriteGenerator(GeneratorSyntax gen, Assoc assoc = Assoc.None, int? precedence = null)
    {
        WriteSep(gen.Names, WriteNode);
        Spaced(IN);
        WriteNode(gen.From);
        if (gen.Where is { } cond)
        {
            Spaced(WHERE);
            WriteNode(cond);
        }
        WriteAnnotations(gen);
    }

    void Spaced(char c)
    {
        Space();
        Write(c);
        Space();
    }

    void Spaced(string c)
    {
        Space();
        Write(c);
        Space();
    }

    void Write(char c)
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

    void Write(IdentifierSyntax id)
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

    void Space()
    {
        _sb.Append(SPACE);
    }

    void Write(string s)
    {
        _sb.Append(s);
    }

    void EndStatement()
    {
        Write(EOL);
        if (!_minify)
            Newline();
    }

    void WriteSpace(string s)
    {
        _sb.Append(s);
        _sb.Append(SPACE);
    }

    void WriteSep<T>(
        IEnumerable<T>? nodes,
        Action<T, Assoc, int?>? write = null,
        Assoc assoc = Assoc.None,
        int prec = 0,
        string sep = ","
    )
        where T : SyntaxNode
    {
        if (nodes is null)
            return;

        using var enumerator = nodes.GetEnumerator();
        bool first = enumerator.MoveNext();
        if (!first)
            return;

        write ??= WriteNode;
        write(enumerator.Current, assoc, prec);
        while (enumerator.MoveNext())
        {
            Write(sep);
            write(enumerator.Current, assoc, prec);
        }
    }

    public void WriteOperator(Operator? op)
    {
        switch (op)
        {
            case null:
                break;

            case Operator.Equivalent:
                Write(OPEN_CHEVRON);
                Write(DASH);
                Write(CLOSE_CHEVRON);
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

    bool WriteAnnotations(SyntaxNode node)
    {
        if (node.Annotations is not { Count: > 0 } anns)
            return false;

        foreach (var ann in anns)
        {
            Space();
            Write(COLON);
            Write(COLON);
            WriteNode(ann);
        }

        return true;
    }

    /// <summary>
    /// Write the given node to a string
    /// </summary>
    public static string Write(SyntaxNode node, WriteOptions? options = null)
    {
        var writer = new Writer(options ?? WriteOptions.Default);
        writer.WriteNode(node);

        // Trim trailing whitespace
        var sb = writer._sb;
        var n = sb.Length;
        while (char.IsWhiteSpace(sb[--n])) { }
        sb.Length = n + 1;

        var text = writer._sb.ToString();
        return text;
    }
}
