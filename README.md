# MiniZinc.Net

Create and solve [MiniZinc](https://www.minizinc.org/) constraint models from C# and F#.

## Immediate Goals

- An API that sparks joy
- Parse models
  - From existing .mzn files
  - From strings
- Solve models
  - All async
  - Stream from stdout
  - Handle timeout / user cancellation
- Create models
  - Think of a nice wa
- Examples

## Stretch Goals / Ideas
- Use a [source generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)? 
  - Have models in separate file and a first class interface to them through C#? that would be great

## Meta Goals
- Promote MiniZinc and constraint solving
- Get this library to a level I would use professionally in production
