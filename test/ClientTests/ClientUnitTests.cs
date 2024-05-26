namespace MiniZinc.Tests;

using Client;
using Parser.Syntax;

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
        var solver = Client.SolveModelText("var 10..20: a; var 40..100: b;");
        var sol = await solver.Solution();
        var a = sol.GetInt("a");
        var b = sol.GetInt("b");
        sol.Status.Should().Be(SolveStatus.Satisfied);
    }

    [Fact]
    async void test_solve_unsat()
    {
        var solver = Client.SolveModelText("var 10..20: a; constraint a < 0;");
        var sol = await solver.Solution();
        sol.Status.Should().Be(SolveStatus.Unsatisfiable);
    }

    [Fact]
    async void test_solve_maximize()
    {
        var solver = Client.SolveModelText("var 10..20: a; var 10..20: b; solve maximize a + b;");
        var sol = await solver.Solution();
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
        var mzn = "array[1..10] of var 0..100: xd;";
        var solver = Client.SolveModelText(mzn);
        var sol = await solver.Solution();
        var result = sol.GetArray1D<int>("xd").ToArray();
    }
}
