using MiniZinc;
using MiniZinc.Client;
using MiniZinc.Parser;
using Shouldly;
using Xunit;

public sealed class ManualClientTests
{
    private readonly MiniZincClient Client;

    public ManualClientTests()
    {
        Client = MiniZincClient.Autodetect();
    }

    [Fact]
    public async Task test_gecode_installed()
    {
        var gecode = Client.GetSolver(Solver.Gecode);
        gecode.Name.ShouldBe("Gecode");
    }

    [Fact]
    public async Task test_chuffed_installed()
    {
        var chuffed = Client.GetSolver(Solver.Chuffed);
    }

    [Fact]
    public async Task test_solve_satisfy_result()
    {
        var model = new MiniZincModel();
        var a = model.AddInt("a", 10, 20);
        var b = model.AddInt("b", 10, 20);
        model.AddConstraint(a < b);
        var result = await Client.Solve(model);
        int aval = result.Data.Get<IntExpr>(a);
        int bval = result.Data.Get<IntExpr>(b);
        aval.ShouldBeLessThan(bval);
        result.Status.ShouldBe(SolveStatus.Satisfied);
    }

    [Fact]
    public async Task test_solve_unsat_result()
    {
        var model = MiniZincModel.FromString("var 10..20: a; constraint a < 0;");
        var solution = await Client.Solve(model);
        solution.Status.ShouldBe(SolveStatus.Unsatisfiable);
    }

    [Fact]
    public async Task test_solve_maximize_result()
    {
        var model = new MiniZincModel();
        model.AddVariable("a", "10..20");
        model.AddVariable("b", "10..20");
        model.Maximize("a + b");
        var result = await Client.Solve(model);
        result.Status.ShouldBe(SolveStatus.Optimal);
        int a = result.Data.Get<IntExpr>("a");
        int b = result.Data.Get<IntExpr>("b");
        a.ShouldBe(20);
        b.ShouldBe(20);
        // result.Objective.ShouldBe(40);
    }

    [Fact]
    public async Task test_solve_return_array()
    {
        var model = MiniZincModel.FromString("array[1..10] of var 0..100: xd;");
        var result = await Client.Solve(model);
        var arr = result.Data.Get<Array1dExpr>("xd");
    }

    [Fact]
    public async Task test_solve_satisfy_foreach()
    {
        var model = new MiniZincModel();
        model.AddVariable("a", "10..20");
        model.AddVariable("b", "10..20");
        await foreach (var result in Client.Solve(model))
        {
            int a = result.Data.Get<IntExpr>("a");
            int b = result.Data.Get<IntExpr>("b");
            result.Status.ShouldBe(SolveStatus.Satisfied);
        }
    }

    [Fact]
    public async Task test_model_replace_objective()
    {
        var model = new MiniZincModel();
        model.AddInt("a", 0, 10);
        model.AddInt("b", 0, 10);
        model.Minimize("a+b");
        var minimum = await Client.Solve(model);
        // minimum.Objective.ShouldBe(0);

        model.Maximize("a+b");
        var maximum = await Client.Solve(model);
        // maximum.Objective.ShouldBe(20);
    }

    [Fact]
    public async Task test_solve_unsat_foreach()
    {
        var model = MiniZincModel.FromString("var 10..20: a; constraint a < 0;");
        await foreach (var result in Client.Solve(model))
        {
            result.Status.ShouldBe(SolveStatus.Unsatisfiable);
        }
    }

    [Fact]
    public async Task test_solve_maximize_foreach()
    {
        var model = new MiniZincModel();
        var a = model.AddVariable("a", "10..20");
        var b = model.AddVariable("b", "10..20");
        // model.Maximize(a + b);
        await foreach (var result in Client.Solve(model))
        {
            result.Status.ShouldBeOneOf(SolveStatus.Optimal, SolveStatus.Satisfied);
        }
    }
}
