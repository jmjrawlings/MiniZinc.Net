# Changelog

## [0.4.2] - 2023-06-18
- Fixed multiple encoding bugs
- Simplified some `Parser` types
- New `NameSpace` object
- Removed `AST` class
- Refactored `Model` object

## [0.4.1] - 2023-06-11
- Scaffold Client integration tests
- Unified Arg/Command/Args constructors around obj[]
- `MiniZincClient` instead of `MiniZinc` module
- `ILogger` can be passed into MiniZinc
- Basic logging of commands
- Renamed `Solve` to `Client` modules throughout
- Models can be serialized with `Encode` module

## [0.4.0] - 2023-06-01
- Shared `MiniZinc.Tests` library
- libminizinc `TestSuites` are now parsed from yaml embedded in example models
- `TestSuite.load` method

## [0.3.0] - 2023-05-27
- Recursive loading of included models
 
## [0.2.0] - 2023-05-24
- Integrate libminizinc test suite 
- Parser tested against libminizinc
- Model type and parsing methods

## [0.1.0] - 2023-05-19 
- MiniZinc AST
- MiniZinc command line runner
- MiniZinc parser with manual tests