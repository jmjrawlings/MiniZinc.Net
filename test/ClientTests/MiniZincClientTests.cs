namespace MiniZinc.Tests;

using Client;
using Parser.Syntax;

public class MiniZincClientTests : TestBase, IClassFixture<ClientFixture>
{
    public readonly MiniZincClient Client;

    public MiniZincClientTests(ClientFixture fixture, ITestOutputHelper output)
        : base(output)
    {
        Client = fixture.Client;
    }

    [Fact]
    void test_create_client() { }

    [Fact]
    void test_gecode_installed()
    {
        var info = Client.Gecode;
    }

    [Fact]
    void test_chuffed_installed()
    {
        var info = Client.Chuffed;
    }

    [Fact]
    async void test_solve_satisfy()
    {
        var solver = Client.SolveModelText("var 10..20: a; var 40..100: b;");
        var sol = await solver.Wait();
        var a = (IntLiteralSyntax)sol.Variables["a"];
        var b = (IntLiteralSyntax)sol.Variables["b"];
        sol.Status.Should().Be(SolveStatus.Satisfied);
    }

    [Fact]
    async void test_solve_unsat()
    {
        var solver = Client.SolveModelText("var 10..20: a; constraint a < 0;");
        var sol = await solver.Wait();
        sol.Status.Should().Be(SolveStatus.Unsatisfiable);
    }

    [Fact]
    async void test_solve_maximize()
    {
        var solver = Client.SolveModelText("var 10..20: a; var 10..20: b; solve maximize a + b;");
        var sol = await solver.Wait();
        sol.Status.Should().Be(SolveStatus.Optimal);
        var a = (IntLiteralSyntax)sol.Variables["a"];
        var b = (IntLiteralSyntax)sol.Variables["b"];
        a.Value.Should().Be(20);
        b.Value.Should().Be(20);
        sol.Objective.Should().Be(40);
    }
}
