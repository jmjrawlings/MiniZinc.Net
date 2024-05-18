namespace MiniZinc.Parser;

using System.Text;
using Syntax;

/// <summary>
/// Writes a minzinc model.
/// Intended to be used via the
/// SyntaxNode extension method `node.Write()`
/// </summary>
internal sealed class Writer
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
    const string ANON_ENUM = "anon_enum";

    private readonly StringBuilder _sb;
    private readonly WriteOptions _options;
    private readonly bool _minify;

    public Writer(WriteOptions options)
    {
        _sb = new StringBuilder();
        _options = options;
        _minify = _options.Minify;
    }

    public void Write(SyntaxNode? expr)
    {
        if (expr is null)
            return;

        switch (expr)
        {
            case SyntaxTree e:
                WriteTree(e);
                break;

            case IncludeSyntax e:
                WriteInclude(e);
                break;

            case TypeAliasSyntax e:
                WriteAlias(e);
                break;

            case FunctionDeclarationSyntax e:
                WriteFunction(e);
                break;

            case EnumDeclarationSyntax e:
                WriteEnum(e);
                break;

            case TypeSyntax e:
                WriteType(e);
                break;

            case Array1DSyntax e:
                WriteArray1D(e);
                break;

            case Array2dSyntax e:
                WriteArray2D(e);
                break;

            case Array3dSyntax e:
                WriteArray3d(e);
                break;

            case ArrayAccessSyntax e:
                WriteArrayAccess(e);
                break;

            case BinaryOperatorSyntax e:
                WriteBinOp(e);
                break;

            case IntLiteralSyntax e:
                Write(e.Value);
                break;

            case BoolLiteralSyntax e:
                Write(e.Value);
                break;

            case FloatLiteralSyntax e:
                Write(e.Value);
                break;

            case StringLiteralSyntax e:
                Write(DOUBLE_QUOTE);
                Write(e.Value);
                Write(DOUBLE_QUOTE);
                break;

            case CallSyntax e:
                WriteCall(e);
                break;

            case ComprehensionSyntax e:
                WriteComprehension(e);
                break;

            case ConstraintSyntax e:
                WriteConstraint(e);
                break;

            case EmptyExpr e:
                Write(OPEN_CHEVRON);
                Write(CLOSE_CHEVRON);
                break;

            case EnumCasesSyntax e:
                WriteEnumCases(e);
                break;

            case GeneratorCallSyntax e:
                WriteGenCall(e);
                break;

            case GeneratorSyntax e:
                WriteGenerator(e);
                break;

            case IdentifierSyntax e:
                Write(e.Token.StringValue);
                break;

            case IfElseSyntax e:
                WriteIfElse(e);
                break;

            case SolveSyntax e:
                WriteSolve(e);
                break;

            case LetSyntax e:
                WriteLet(e);
                break;

            case RangeLiteralSyntax e:
                if (e.Lower is { } lower)
                    Write(lower);
                Write(DOT);
                Write(DOT);
                if (e.Upper is { } upper)
                    Write(upper);
                break;

            case RecordAccessSyntax e:
                Write(e.Expr);
                Write(DOT);
                Write(e.Field.StringValue);
                break;

            case RecordLiteralSyntax e:
                WriteRecord(e);
                break;

            case SetLiteralSyntax e:
                Write(OPEN_BRACE);
                WriteSep(e.Elements);
                Write(CLOSE_BRACE);
                break;

            case TupleAccessSyntax e:
                Write(e.Expr);
                Write(DOT);
                Write(e.Field.IntValue);
                break;

            case TupleLiteralSyntax e:
                Write(OPEN_PAREN);
                WriteSep(e.Fields);
                Write(CLOSE_PAREN);
                break;

            case UnaryOperatorSyntax e:
                Write(e.Operator);
                Write(SPACE);
                Write(e.Expr);
                break;

            case DeclarationSyntax e:
                WriteDeclare(e);
                break;

            case AssignmentSyntax e:
                WriteAssignment(e);
                break;

            case OutputSyntax e:
                WriteOutput(e);
                break;

            case WildCardExpr e:
                Write(UNDERSCORE);
                break;

            case IndexAndNode e:
                Write(e.Index);
                Write(COLON);
                Write(e.Value);
                break;

            default:
                throw new Exception(expr.GetType().ToString());
        }
    }

    private void WriteAlias(TypeAliasSyntax e)
    {
        Write(TYPE);
        Space();
        Write(e.Name);
        Spaced(EQUAL);
        Write(e.Type);
        EndStatement();
    }

    private void WriteOutput(OutputSyntax e)
    {
        Write(OUTPUT);
        Space();
        Write(e.Expr);
        EndStatement();
    }

    private void WriteAssignment(AssignmentSyntax e)
    {
        Write(e.Name);
        Space();
        Write(EQUAL);
        Space();
        Write(e.Expr);
        EndStatement();
    }

    private void WriteEnumCases(EnumCasesSyntax e)
    {
        switch (e.Type)
        {
            case EnumCaseType.Anon:
                Write(ANON_ENUM);
                Write(OPEN_PAREN);
                Write(e.Expr);
                Write(CLOSE_PAREN);
                break;

            case EnumCaseType.Underscore:
                Write(UNDERSCORE);
                Write(OPEN_PAREN);
                Write(e.Expr);
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
                Write(e.Expr);
                Write(CLOSE_PAREN);
                break;
        }
    }

    private void WriteCall(CallSyntax e)
    {
        Write(e.Name);
        Write(OPEN_PAREN);
        WriteSep(e.Args);
        Write(CLOSE_PAREN);
    }

    private void WriteTree(SyntaxTree e)
    {
        foreach (var node in e.Nodes)
            Write(node);
    }

    private void WriteConstraint(ConstraintSyntax e)
    {
        Write(CONSTRAINT);
        Space();
        Write(e.Expr);
        EndStatement();
    }

    private void WriteComprehension(ComprehensionSyntax e)
    {
        Write(e.IsSet ? OPEN_BRACE : OPEN_BRACKET);
        Write(e.Expr);
        Write(PIPE);
        WriteSep(e.Generators);
        Write(e.IsSet ? CLOSE_BRACE : CLOSE_BRACKET);
    }

    private void WriteInclude(IncludeSyntax e)
    {
        WriteSpace(INCLUDE);
        Write(DOUBLE_QUOTE);
        Write(e.Path.StringValue);
        Write(DOUBLE_QUOTE);
        EndStatement();
    }

    private void WriteFunction(FunctionDeclarationSyntax e)
    {
        Write(FUNCTION);
        Space();
        Write(e.Type);
        Write(COLON);
        Space();
        Write(e.Name);
        WriteParameters(e.Parameters);
        if (e.Body is { } body)
        {
            Write(EQUAL);
            Newline();
            Write(body);
        }
        EndStatement();
    }

    private void WriteEnum(EnumDeclarationSyntax e)
    {
        Write(ENUM);
        Space();
        Write(e.Name);
        WriteAnnotations(e);
        if (e.Cases.Count > 0)
        {
            Write(EQUAL);
            WriteSep(e.Cases, sep: "++");
        }
        EndStatement();
    }

    private void WriteBinOp(BinaryOperatorSyntax e)
    {
        Write(OPEN_PAREN);
        Write(e.Left);
        Write(SPACE);
        if (e.Operator is { } op)
            Write(op);
        else
        {
            Write(BACKTICK);
            Write(e.Infix.StringValue);
            Write(BACKTICK);
        }

        Write(SPACE);
        Write(e.Right);
        Write(CLOSE_PAREN);
    }

    private void WriteSolve(SolveSyntax e)
    {
        WriteSpace(SOLVE);
        WriteAnnotations(e);
        switch (e.Method)
        {
            case SolveMethod.Satisfy:
                Write(SATISFY);
                break;
            case SolveMethod.Maximize:
                WriteSpace(MAXIMIZE);
                Write(e.Objective);
                break;
            case SolveMethod.Minimize:
                WriteSpace(MINIMIZE);
                Write(e.Objective);
                break;
        }
        EndStatement();
    }

    private void WriteGenCall(GeneratorCallSyntax e)
    {
        Write(e.Name);
        Write(OPEN_PAREN);
        WriteSep(e.Generators, WriteGenerator);
        Write(CLOSE_PAREN);
        Write(OPEN_PAREN);
        Write(e.Expr);
        Write(CLOSE_PAREN);
    }

    private void WriteIfElse(IfElseSyntax e)
    {
        Write(IF);
        Space();
        Write(e.If);
        Spaced(THEN);
        Write(e.Then);
        if (e.ElseIfs is { } cases)
        {
            foreach (var (elseif, then) in cases)
            {
                Spaced(ELSEIF);
                Write(elseif);
                Spaced(THEN);
                Write(then);
            }
        }
        Write(ENDIF);
    }

    void WriteArray3d(Array3dSyntax e)
    {
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
                    Write(v);
                    if (k + 1 < e.K)
                        Write(COMMA);
                }
                Write(PIPE);
            }

            if (i + 1 < e.I)
                Write(COMMA);
        }
        Write("|]");
    }

    void WriteDeclare(DeclarationSyntax dec)
    {
        WriteType(dec.Type);
        Write(COLON);
        Write(dec.Name);
        if (dec.Body is { } body)
        {
            Write(EQUAL);
            Write(body);
        }
        EndStatement();
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
                WriteSep(e.Dimensions, Write);
                Write(CLOSE_BRACKET);
                Space();
                Write(OF);
                Space();
                WriteType(e.Items);
                break;

            case ComplexTypeSyntax e:
                WriteSep(e.Types, Write, sep: "++");
                break;

            case ExprType e:
                Write(e.Expr);
                break;

            case ListTypeSyntax e:
                WriteSpace(LIST);
                WriteSpace(OF);
                Write(e.Items);
                break;

            case NameTypeSyntax e:
                Write(e.Name);
                break;

            case RecordTypeSyntax e:
                Write(RECORD);
                WriteParameters(e.Fields);
                break;

            case SetTypeSyntax e:
                WriteSpace(SET);
                Spaced(OF);
                Write(e.Items);
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
                Write(ANN);
                break;

            default:
                throw new NotImplementedException(type.ToString());
        }
    }

    void WriteLet(LetSyntax e)
    {
        Write(LET);
        Write(OPEN_BRACE);
        WriteSep(e.Locals?.Cast<SyntaxNode>());
        Write(CLOSE_BRACE);
        Spaced(IN);
        Write(e.Body);
    }

    void WriteParameters(List<ParameterSyntax>? parameters)
    {
        if (parameters is null)
            return;
        Write(OPEN_PAREN);
        WriteSep(parameters, WriteParameter);
        Write(CLOSE_PAREN);
    }

    void WriteParameter(ParameterSyntax x)
    {
        Write(x.Type);
        if (x.Name is { } name)
        {
            Write(COLON);
            Write(name);
        }
        WriteAnnotations(x);
    }

    void WriteArrayAccess(ArrayAccessSyntax e)
    {
        Write(e.Array);
        Write(OPEN_BRACKET);
        WriteSep(e.Access);
        Write(CLOSE_BRACKET);
    }

    void WriteRecord(RecordLiteralSyntax e)
    {
        Write(OPEN_PAREN);
        for (int i = 0; i < e.Fields.Count; i++)
        {
            var (name, expr) = e.Fields[i];
            Write(name);
            Write(COLON);
            Write(expr);
            if (i < e.Fields.Count - 1)
                Write(COMMA);
        }

        Write(CLOSE_PAREN);
    }

    void WriteArray2D(Array2dSyntax expr)
    {
        Write(OPEN_BRACKET);
        Write(PIPE);
        int x = 0;

        for (int i = 0; i < expr.I; i++)
        {
            for (int j = 0; j < expr.J; j++)
            {
                var v = expr.Elements[x++];
                Write(v);
                if (j < expr.J - 1)
                    Write(COMMA);
            }
            Write(PIPE);
            if (i < expr.I - 1)
                Write(COMMA);
        }
        Write(CLOSE_BRACKET);
    }

    void WriteArray1D(Array1DSyntax e)
    {
        Write(OPEN_BRACKET);
        WriteSep(e.Elements);
        Write(CLOSE_BRACKET);
    }

    void WriteGenerator(GeneratorSyntax gen)
    {
        WriteSep(gen.Names, Write);
        Spaced(IN);
        Write(gen.From);
        if (gen.Where is { } cond)
        {
            Spaced(WHERE);
            Write(cond);
        }
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
        Write(NEWLINE);
    }

    void Write(IdentifierSyntax id) => _sb.Append(id);

    void Space()
    {
        Write(SPACE);
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

    void WriteSep<T>(IEnumerable<T>? nodes, Action<T>? write = null, string sep = ",")
        where T : SyntaxNode
    {
        if (nodes is null)
            return;

        using var enumerator = nodes.GetEnumerator();
        bool first = enumerator.MoveNext();
        if (!first)
            return;

        write ??= Write;
        write(enumerator.Current);
        while (enumerator.MoveNext())
        {
            Write(sep);
            write(enumerator.Current);
        }
    }

    void Write(int i) => _sb.Append(i);

    void Write(double f) => _sb.Append(f);

    void Write(bool b) => _sb.Append(b);

    public void Write(Operator? op)
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
        if (node.Annotations is not { Count: > 0 } anns)
            return;

        foreach (var ann in anns)
        {
            Space();
            Write(COLON);
            Write(COLON);
            Write(ann);
        }
    }

    public static string WriteNode(SyntaxNode node, WriteOptions? options = null)
    {
        var writer = new Writer(options ?? new WriteOptions());
        writer.Write(node);
        var s = writer._sb.ToString();
        return s;
    }
}
