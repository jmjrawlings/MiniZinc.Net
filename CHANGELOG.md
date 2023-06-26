# Changelog

## Unreleased

### Model
- Removed distinction between `Function` and `Predicate` items
- Removed distinction between `Array` and `List` types
- Removed `Ast.fs` 
- Combine `Ast.fs` and `Model.fs`
- Unified parsing into `Parse.fs`
- Unified encoding into `Encode.fs`
- `Encode.fs` and `Parse.fs` now use type and module extensions to augment the `Model` type
- Let exprs now contain a `NameSpace`
- Added `ArrayDim` union for array dimensions

### Parser
- Parser no longer accepts invalid array dimensions

### Client
- Added `Solve` and `SolveSync` methods on `MiniZincClient`
- Renamed `Command.Exec` to `Run` and `RunSync`
- Added support for `--model-interface-only` which returns a `ModelInterface`
- Added support for `--model-types-only` which returns a `ModelTypes`
- Split `Client.fs` out into several other files such that each one extends the client in a contained manner
  - `ModelInterface.fs`
  - `ModelTypes.fs`
  - `Solve.fs`
 
### Tests
- Refactor client tests to use a fixture
- Client integration tests now actually solve the models

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