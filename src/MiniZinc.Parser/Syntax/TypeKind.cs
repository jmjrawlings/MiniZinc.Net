﻿namespace MiniZinc.Parser.Syntax;

public enum TypeKind
{
    Any,
    Int,
    Bool,
    String,
    Float,
    Name,
    Annotation,
    Generic,
    GenericSeq,
    Expr,
    Tuple,
    Record,
    Array,
    List,
    Complex,
    Set
}