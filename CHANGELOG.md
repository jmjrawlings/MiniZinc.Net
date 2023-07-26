# Changelog

## [0.6.0] - 2023-07-26

Further progress towards full test coverage.

## Parser
- Added support for Tuple and Record literals
- Added support for tuple and record field access
- Combined `keyword1` and `keyword` parsing
- Added support for instanced variables `$T` in `TypeInsts`
- Simplified enum parsing
- Removed some unused parsers and AST cases
- Added support for annotations on Output expressions
- Added support for bodies in Annotation types
- Replaced identifier parsing with the FParsec's `identifier`
- Replaced integer and float parsing with FParsec's `numberLiteral`
- Removed the `NumExpr` class entirely as it was just a subset of `Expr`
- Rewrote `parseComments` entirely to correctly handle line comments, block comments, escaped quotes etc

## Model
- Added support for instanced variables `$T` in `TypeInsts`
 
## Client
- Changed `ILogger` to `ILogger<T>`
- Added basic logging to `compile`
- Changed `CommandResult` and `Arg` to structs

## Bugfixes
- Fixed several bugs related to comment preprocessing


## [0.5.0] - 2023-07-16

The major milestone of this release is (almost) full integration with the [libminizinc test suite](https://github.com/MiniZinc/libminizinc/tree/master/tests/spec).

As part of the [Build Project](./build/build.fsproj) we now do the following:
- Clone libminizinc
- Copy the test spec directory
- Generate our own F# test cases for the Model/Parser
- Generate our own F# test cases for the Client

The functions responsible for generating the F# tests can be found here:
- [ModelTests.fs](./build/ModelTests.fs)
- [ClientTests.fs](./build/ClientTests.fs)

The resulting integration tests are:
- [MiniZinc.ModelTests/IntegrationTests.fs](./tests/MiniZinc.ModelTests/IntegrationTests.fs)
- [MiniZinc.ClientTests/IntegrationTests.fs](./tests/MiniZinc.ClientTests/IntegrationTests.fs)

We are **NOT** passing all of these tests yet.  The unit tests and examples test every strange dark corner and 
murky edge case of MiniZinc syntax which is exactly what we want but is going to take some time to get
100% coverage. 

I'll be plugging away on failing tests with a goal of 100% compliance for the next minor release. 


### Model
- Removed distinction between `Function` and `Predicate` items
- Removed distinction between `Array` and `List` types
- Unified parsing into `Parse.fs`
- Unified encoding into `Encode.fs`
- `let-exprs` now contain a `NameSpace`
- Added `ArrayDim` union for array dimensions
- Added the absent value `<>`
- Added annotations for `ConstraintItems`
- Added `EnumCases` union to capture valid types

### Parser
- Parser no longer accepts invalid array dimensions
- Added support for the absent value `<>`
- Added support for `annotations`
- Added a `NamedTypeInst` to capture (Id, TypeInst, Annotations)
- Added support for complex enum constructors

### Client
- Added `Solve` and `SolveSync` methods on `MiniZincClient`
- Renamed `Command.Exec` to `Run` and `RunSync`
- Added support for `--model-interface-only` which returns a `ModelInterface`
- Added support for `--model-types-only` which returns a `ModelTypes`
- Split `Client.fs` out into several other files such that each one extends the client in a contained manner
  - `ModelInterface.fs`
  - `ModelTypes.fs`
  - `Solve.fs`
- Added `SolveOptions` to capture command line flags
- Added a `compile` step between the model and the solver

### Bugfixes
- Fixed a bug where `let-expr` that contained constraints would not parse properly
- Fixed a bug where the parser would fail for `call-expr` with no arguments
- Fixed a bug where the parser required `else` cases on `if-then-expr`
- Fixed a bug where the parser would fail for empty `enum` declarations
- Fixed some inconsistencies in the parser test suite

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