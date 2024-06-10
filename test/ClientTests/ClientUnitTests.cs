namespace MiniZinc.Tests;

using Client;

public class ClientUnitTests : TestBase, IClassFixture<ClientFixture>
{
    public readonly MiniZincClient Client;

    public ClientUnitTests(ClientFixture fixture, ITestOutputHelper output)
        : base(output)
    {
        Client = fixture.Client;
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
    async void test_solve_satisfy()
    {
        var model = new Model();
        model.Var("a", "10..20");
        model.Var("b", "10..20");
        var sol = await Client.Solve(model);
        var a = sol.GetInt("a");
        var b = sol.GetInt("b");
        sol.Status.Should().Be(SolveStatus.Satisfied);
    }

    [Fact]
    async void test_solve_unsat()
    {
        var model = Model.FromString("var 10..20: a; constraint a < 0;");
        var solution = await Client.Solve(model);
        solution.Status.Should().Be(SolveStatus.Unsatisfiable);
    }

    [Fact]
    async void test_solve_maximize()
    {
        var model = new Model();
        model.Var("a", "10..20");
        model.Var("b", "10..20");
        model.Maximize("a + b");
        var sol = await Client.Solve(model);
        sol.Status.Should().Be(SolveStatus.Optimal);
        var a = sol.GetInt("a");
        var b = sol.GetInt("b");
        a.Should().Be(20);
        b.Should().Be(20);
        sol.Objective.Should().Be(40);
    }

    [Fact]
    async void test_solve_return_array()
    {
        var model = Model.FromString("array[1..10] of var 0..100: xd;");
        var sol = await Client.Solve(model);
        var result = sol.GetArray1D<int>("xd").ToArray();
    }
}
