namespace MiniZinc.Tests;

using Client;
using Compiler;

public class ClientUnitTests : TestBase, IClassFixture<ClientFixture>
{
    public readonly MiniZincClient Client;

    public ClientUnitTests(ClientFixture fixture, ITestOutputHelper output)
        : base(output)
    {
        Client = fixture.MiniZinc;
    }

    [Fact]
    void test_create_client() { }

    [Fact]
    void test_gecode_installed()
    {
        var gecode = Client.GetSolver(Solver.Gecode);
    }

    [Fact]
    void test_chuffed_installed()
    {
        var chuffed = Client.GetSolver(Solver.Chuffed);
    }

    [Fact]
    async void test_solve_satisfy_result()
    {
        var model = new IntModel();
        var a = model.AddInt("a", 10, 20);
        var b = model.AddInt("b", 10, 20);
        model.AddConstraint(a < b);
        var result = await Client.Solve(model);
        int aval = result.Data.Get<IntData>(a);
        int bval = result.Data.Get<IntData>(b);
        aval.Should().BeLessThan(bval);
        result.Status.Should().Be(SolveStatus.Satisfied);
    }

    [Fact]
    async void test_solve_unsat_result()
    {
        var model = Model.FromString("var 10..20: a; constraint a < 0;");
        var solution = await Client.Solve(model);
        solution.Status.Should().Be(SolveStatus.Unsatisfiable);
    }

    [Fact]
    async void test_solve_maximize_result()
    {
        var model = new IntModel();
        model.AddVariable("a", "10..20");
        model.AddVariable("b", "10..20");
        model.Maximize("a + b");
        var result = await Client.Solve(model);
        result.Status.Should().Be(SolveStatus.Optimal);
        int a = result.Data.Get<IntData>("a");
        int b = result.Data.Get<IntData>("b");
        a.Should().Be(20);
        b.Should().Be(20);
        result.Objective.Should().Be(40);
    }

    [Fact]
    async void test_solve_return_array()
    {
        var model = Model.FromString("array[1..10] of var 0..100: xd;");
        var result = await Client.Solve(model);
        var arr = result.Data.Get<IntArray1d>("xd");
    }

    [Fact]
    async void test_solve_satisfy_foreach()
    {
        var model = new IntModel();
        model.AddVariable("a", "10..20");
        model.AddVariable("b", "10..20");
        await foreach (var result in Client.Solve(model))
        {
            int a = result.Data.Get<IntData>("a");
            int b = result.Data.Get<IntData>("b");
            result.Status.Should().Be(SolveStatus.Satisfied);
        }
    }

    [Fact]
    async void test_model_replace_objective()
    {
        var model = new IntModel();
        model.AddInt("a", 0, 10);
        model.AddInt("b", 0, 10);
        model.Minimize("a+b");
        var minimum = await Client.Solve(model);
        minimum.Objective.Should().Be(0);

        model.Maximize("a+b");
        var maximum = await Client.Solve(model);
        maximum.Objective.Should().Be(20);
    }

    [Fact]
    async void test_solve_unsat_foreach()
    {
        var model = Model.FromString("var 10..20: a; constraint a < 0;");
        await foreach (var result in Client.Solve(model))
        {
            result.Status.Should().Be(SolveStatus.Unsatisfiable);
        }
    }

    [Fact]
    async void test_solve_maximize_foreach()
    {
        var model = new IntModel();
        model.AddVariable("a", "10..20");
        model.AddVariable("b", "10..20");
        model.Maximize("a + b");
        await foreach (var result in Client.Solve(model))
        {
            result.Status.Should().BeOneOf(SolveStatus.Optimal, SolveStatus.Satisfied);
        }
    }
}
