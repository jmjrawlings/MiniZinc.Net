# Changelog

## [0.7.0] - UNRELEASED

I have completely rewritten from F# to C#.

Reasons for this in no particular order:

- `MiniZinc.Net` was always intended to be used from C# and maintaining both an idiomatic F# api and C# api was too much work
- Modern (.NET6+) C# has incorporated just about every feature of F# at this point (ADTs notwithstanding) but the reverse is not true
  - `IAsyncEnumerable` is a core feature of solvers that doesn't seem to be included in F#?
- Tooling and compilation are substantially quicker with C#
- A new job has me developing in C# 8 and enjoying it a lot
- As soon as you want any sort of performance you need to drop into
- While I love `FParsec` I was suffering a bit from not having a separate lexing and parsing step
- I'm aiming for as little dependencies as possible for the library and handing writing a lexer/parser just doesn't seem that hard (we will see)
- C# is slated to get algebraic data types at some point at which point there is almost no reason to prefer F# other than the syntax which I do love


## [0.6.4] - 2023-08-23

More improvements to the array parsers.  I think they are
just about as fast as they are going to get and more than
sufficient for 99.9% of use cases.

### Added
- Added support for [indexed array literals](https://www.minizinc.org/doc-2.7.3/en/spec.html#indexed-array-literals) (!D only)

### Fixed
- Fixed a bug that caused tests not to run properly on linux

### Changed
- Upgraded all projects to .NET7


## [0.6.3] - 2023-08-18

The parser improvements continue with 1D, 2D and 3D array
parsers all rewritten with performance in mind.  The `big array lit` example
from the unit test suite went from taking over a minute to under a second.

### Changed
- The Array1D, Array2D and Array3D parsers have been rewritten

### Fixed
- Some roundtrip errors involving operators
- Several random issue causing unit tests to fail

### Meta
- Tests will only capture traces in `DEBUG` releases
- Tests that use unsupported language features have been Skipped in XUnit


## [0.6.2] - 2023-08-13

The parser has been heavily refactored as per the FParsec Performance Guidelines. While the parsing code has lost a lot its aesthetic qualities and is probably harder to understand at a glance, I feel like the benefits were well worth the change.

Using the lower level CharStream operations has made certain finicky edge cases (there are a lot in MiniZinc) much easier to handle. Debugging these edges cases is also greatly enhanced with access to standard breakpoints in our functions. Many thanks to @stephan-tolksdorf for such a well crafted library.

Just from my manual tests I can feel the parser is much much quicker. I wish I had benchmarked the old implementation to know just how much faster it is but alas. Apart from the obvious motivation of performance always being a feature, I am hoping that a fast parser gives us the flexibility to manipulate models using minizinc strings as opposed to transforming the AST directly if it would be easier.

### Changed
- Refactored the `Item` parser
- Refactored the `Expr` parser
- Refactored the `NumExpr` parser
- Fixed multiple edge cases in the parser as shown by passing tests
- Quoted binop calls are parsed as `Expr.BinaryOp`
- Quoted ident infixes are parsed as `Expr.Call`


## [0.6.1] - 2023-08-01

### Added
- Support for 3D array literals
- Improved testing error messages

## [0.6.0] - 2023-07-26

Further progress towards full test coverage.

### Parser
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

### Model
- Added support for instanced variables `$T` in `TypeInsts`

### Client
- Changed `ILogger` to `ILogger<T>`
- Added basic logging to `compile`
- Changed `CommandResult` and `Arg` to structs

### Bugfixes
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