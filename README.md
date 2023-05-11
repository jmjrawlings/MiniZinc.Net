# MiniZinc.Net

Create and solve [MiniZinc](https://www.minizinc.org/) constraint models from C# and F#.

## Immediate Goals

- Parse models
  - From existing .mzn files
  - From strings
  - Full EBNF support?
- Solve models
  - All async
  - Stream from stdout
  - Handle timeout / user cancellation
- Create models
  - Come up with a nice DSL
  - Computational Expressions?

## Stretch Goals / Ideas
- Use a [source generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)? 
  - Have models in separate file and a first class interface to them through C#? that would be great

## Meta Goals
- Be world class, make constraint solving feel like a first class addition to .NET
- Promote MiniZinc and constraint solving
- Get this library to a level I would use professionally in production