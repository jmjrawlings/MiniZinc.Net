namespace MiniZinc.Parser.Ast;

public readonly record struct RangeExpr(INode? Lower = null, INode? Upper = null) : INode;
