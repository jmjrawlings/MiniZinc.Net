namespace MiniZinc.Parser.Ast;

/// <summary>
/// An empty value used for optional types
/// </summary>
/// <mzn>opt int: x = &lt;&gt;>;</mzn>
public sealed record EmptyExpr : Expr { }
