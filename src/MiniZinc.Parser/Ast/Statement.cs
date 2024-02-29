namespace MiniZinc.Parser;

using Annotation = IExpr;

public interface IStatement { }

public sealed record OutputStatement : IStatement
{
    public IExpr Expr { get; set; }
    public Annotation? Annotation { get; set; }
}

public sealed record AnnotationDef : IBinding
{
    public string Name { get; set; }

    public List<TypeInst>? Params { get; set; }

    public IExpr? Body { get; set; }
}

public sealed record Comment : IStatement
{
    public string String { get; set; }
}

public record EnumDef : IBinding, IStatement, IAnnotations
{
    public string Name { get; set; }
    public List<Annotation>? Annotations { get; set; }
}

public sealed record NamedEnumDef : EnumDef
{
    public List<string> Members { get; set; } = new();
}

public sealed record AnonEnumDef : EnumDef
{
    public IExpr Expr { get; set; }
}

public sealed class FunctionDef : IBinding, IAnnotations
{
    public string Name { get; set; }

    public TypeInst Returns { get; set; }
    public List<Annotation>? Annotations { get; set; }

    public List<TypeInst> Parameters { get; set; }

    public string Ann { get; set; }

    public IExpr? Body { get; set; }
}

public sealed record AssignStatement : IStatement
{
    public string Name { get; set; }
    public IExpr Expr { get; set; }
}

public sealed record DeclareStatement : IStatement
{
    public TypeInst TypeInst { get; set; }
}

public record SolveStatement : IStatement, IAnnotations
{
    public SolveMethod SolveMethod { get; set; }
    public List<Annotation>? Annotations { get; set; }
    public IExpr? Expr { get; set; }
}

public sealed record TestStatement : IStatement, IAnnotations
{
    public string Name { get; set; }
    public List<TypeInst> Params { get; set; }
    public List<Annotation>? Annotations { get; set; }
    public IExpr? Body { get; set; }
}

public sealed record IncludeStatement : IStatement
{
    public string Path { get; set; }
    public FileInfo? File { get; set; }
}
