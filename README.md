# MiniZinc.Net

Create, parse and solve [MiniZinc](https://www.minizinc.org/) constraint models from C# and F#.

> *PLEASE NOTE*
> This is very much a work in progress.  I'm only making it public at this point because the parser is pretty good and I'm
> sure others can benefit from it.

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
    - Use a [source generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) 
    - Have models in separate file and a first class interface to them through C#? that would be great
  - F#
    - Use a 

- Comprehensive examples and walkthroughs
 

## Motivation

- Give back to the MiniZinc community
- Share 
- Promote MiniZinc and constraint solving  
- Make modelling and solving CP problems a joyful experience
- Get this library to a level I would use professionally in production
