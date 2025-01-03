﻿namespace MiniZinc.Parser;

public sealed class IntExpr : MiniZincExpr
{
    public int Value { get; }

    public IntExpr(in Token start)
        : base(start)
    {
        Value = start.IntValue;
    }

    public IntExpr(in Token start, int value)
        : base(start)
    {
        Value = value;
    }

    public static implicit operator int(IntExpr expr) => expr.Value;

    public override string ToString() => Value.ToString();
}