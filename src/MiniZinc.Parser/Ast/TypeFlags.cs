namespace MiniZinc.Parser.Ast;

[Flags]
public enum TypeFlags
{
    None = 1 << 0,
    Par = 1 << 1,
    Var = 1 << 2,
    Set = 1 << 3,
    Opt = 1 << 4,
    Array = 1 << 5
}
