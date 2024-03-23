﻿using System.Runtime.InteropServices.JavaScript;

namespace MiniZinc.Parser.Ast;

public readonly record struct ArrayAccessExpr : INode
{
    public INode Array { get; }
    public List<INode> Access { get; }

    public ArrayAccessExpr(INode array, List<INode> access)
    {
        Array = array;
        Access = access;
    }

    public override string ToString() => $"{Array.Write()}[{Access.Write()}]";
}