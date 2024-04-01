namespace MiniZinc.Parser.Ast;

/// <summary>
/// An empty value used for optional types
/// </summary>
/// <mzn>opt int: x = <>;</mzn>
public readonly record struct EmptyExpr : IExpr { }
