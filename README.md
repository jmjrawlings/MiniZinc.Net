# MiniZinc.Net

Create, parse and solve [MiniZinc](https://www.minizinc.org/) constraint models from C# and F#.

> **IMPORTANT**
> 
> This is pre-release software.
> 
> I'm only making it public at this point because I'm quite happy with the parser and I'm sure others can benefit from it.
> 
> This is written in F# but the goal is to have an idiomatic C# API also


## Immediate Goals

- Parse models
  - From model string
  - From `.mzn` file
  - Full AST as per the [Specification](https://www.minizinc.org/doc-2.7.3/en/spec.html#full-grammar)

- Solve models
  - All async iterators
  - Stream from stdout, stderr
  - Handle timeout / user cancellation

## Wishlist

- Create models
  - Come up with a nice DSL
  - Computational Expressions?

- Augment models
  - Parse then edit?

- Compile time execution

  - C#
    - Use a [Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) 
      - Have models in separate file and a first class interface to them through C#? that would be great
  - F#
    - Use a [Type Provider](https://learn.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/)? 
      - They are pretty clunky honestly and not fun to build

- Comprehensive examples and walkthroughs
 
## Quickstart

- Download [.NET6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Clone the repo
- `dotnet tool restore`
- `dotnet test`


## Motivation

- Give back to the MiniZinc community
- Share what I've learned from solving problems in industry
- Promote MiniZinc and constraint solving  
- Make modelling and solving CP problems a joyful experience
- Get this library to a level I would use professionally in production


## References

- [MiniZinc Installation](https://www.minizinc.org/doc-2.7.4/en/installation.html)
- [MiniZinc Tutorial](https://www.minizinc.org/doc-2.7.4/en/part_2_tutorial.html)
- [MiniZinc User Manual](https://www.minizinc.org/doc-2.7.4/en/part_3_user_manual.html)
- [MiniZinc Reference](https://www.minizinc.org/doc-2.7.4/en/part_4_reference.html)
- [Basic Modelling Course 1](https://www.coursera.org/learn/basic-modeling)
- [Advanced Modelling Course 2](https://www.coursera.org/learn/advanced-modeling)
- [libminizinc repository](https://github.com/MiniZinc/libminizinc)
