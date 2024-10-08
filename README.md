# MiniZinc.Net

Create, parse and solve [MiniZinc](https://www.minizinc.org/) constraint models using C# and .NET

> ** This is work in progress **

## Examples

Models can be created at any time but can only be solved using an instance of `MiniZincClient`.

```csharp
var minizinc = MiniZincClient.Create();
```

### Solve a model from a minizinc string
```csharp
var result = await minizinc.Solve("var 10..20: a; solve minimize a;");
result.GetInt("a"); // 20
result.Objective; // 20
result.Status; // SolveStatus.Optimal
```

### Create and solve a minizinc model

var model = new IntModel();
var a = model.AddInt("a", 10, 20);
var b = model.AddInt("b", 10, 20);
model.AddConstraint(a < b);
model.Minimize(a + b);
var result = await minizinc.Solve(model);
result.GetInt(a); // 10
result.GetInt(b); // 11
result.Objective; // 21
result.Status; // SolveStatus.Optimal

### Create and solve a minizinc model

var model = new IntModel();
var a = model.AddInt("a", 10, 20);
var b = model.AddInt("b", 10, 20);
model.AddConstraint(a < b);
model.Minimize(a + b);
var result = await minizinc.Solve(model);
result.GetInt(a); // 10
result.GetInt(b); // 11
result.Objective; // 21
result.Status; // SolveStatus.Optimal







## Developer Guide 

To develop and test the codebase you need will need to have access to an environment where both the .NET8 SDK and MiniZinc toolchain installed.  You can install these dependencies manually or use a preconfigured docker container.

### Option 1 - Manual Setup

- Install the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) 
- Install [MiniZinc](https://www.minizinc.org/downloads/) toolchain
- `git clone https://github.com/jmjrawlings/MiniZinc.Net.git`
- `cd MiniZinc.Net`
- `dotnet tool restore`
- `dotnet test`
 

### Option 2 - Devcontainer

- Install [Docker](https://www.docker.com/)
- Install [VSCode](https://code.visualstudio.com/)
- `git clone https://github.com/jmjrawlings/MiniZinc.Net.git`
- `code MiniZinc.Net`
- "Reopen in container" when prompted
- `dotnet test` from within the container


## Design

TODO


## Examples

TODO


## References

- [MiniZinc Installation](https://www.minizinc.org/doc-latest/en/installation.html)
- [MiniZinc Tutorial](https://www.minizinc.org/doc-latest/en/part_2_tutorial.html)
- [MiniZinc User Manual](https://www.minizinc.org/doc-latest/en/part_3_user_manual.html)
- [MiniZinc Reference](https://www.minizinc.org/doc-latest/en/part_4_reference.html)
- [Basic Modelling Course 1](https://www.coursera.org/learn/basic-modeling)
- [Advanced Modelling Course 2](https://www.coursera.org/learn/advanced-modeling)
- [libminizinc repository](https://github.com/MiniZinc/libminizinc)
- [Modelling for Combinatorial Optimisation [M4CO]"](https://user.it.uu.se/~pierref/courses/COCP/slides/)