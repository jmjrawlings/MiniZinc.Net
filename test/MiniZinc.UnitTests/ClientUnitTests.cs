using MiniZinc;
using MiniZinc.Client;
using MiniZinc.Command;
using MiniZinc.Parser;
using static System.Console;

public sealed class ManualClientTests
{
    private readonly MiniZincClient Client;

    public ManualClientTests()
    {
        Client = MiniZincClient.Autodetect();
    }

    [Test]
    public async Task test_gecode_installed()
    {
        var gecode = Client.GetSolver(MiniZincSolver.GECODE);
        gecode.Name.ShouldBe("Gecode");
    }

    [Test]
    public async Task test_chuffed_installed()
    {
        var chuffed = Client.GetSolver(MiniZincSolver.CHUFFED);
    }

    [Test]
    public async Task test_solve_satisfy_result()
    {
        var model = new MiniZincModel();
        var a = model.AddInt("a", 10, 20);
        var b = model.AddInt("b", 10, 20);
        model.AddConstraint(a < b);
        var result = await Client.Solution(model);
        int aval = result.Data.Get<IntExpr>(a);
        int bval = result.Data.Get<IntExpr>(b);
        aval.ShouldBeLessThan(bval);
        result.Status.ShouldBe(SolveStatus.Satisfied);
    }

    [Test]
    public async Task test_solve_unsat_result()
    {
        var model = MiniZincModel.ParseString("var 10..20: a; constraint a < 0;");
        var solution = await Client.Solution(model);
        solution.Status.ShouldBe(SolveStatus.Unsatisfiable);
    }

    [Test]
    public async Task test_solve_maximize_result()
    {
        var model = new MiniZincModel();
        model.AddVariable("a", "10..20");
        model.AddVariable("b", "10..20");
        model.Maximize("a + b");
        var result = await Client.Solution(model);
        result.Status.ShouldBe(SolveStatus.Optimal);
        int a = result.Data.Get<IntExpr>("a");
        int b = result.Data.Get<IntExpr>("b");
        a.ShouldBe(20);
        b.ShouldBe(20);
        // result.Objective.ShouldBe(40);
    }

    [Test]
    public async Task test_solve_return_array()
    {
        var model = MiniZincModel.ParseString("array[1..10] of var 0..100: xd;");
        var result = await Client.Solution(model);
        var arr = result.Data.Get<Array1dExpr>("xd");
    }

    [Test]
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

    [Test]
    public async Task test_model_replace_objective()
    {
        var model = new MiniZincModel();
        model.AddInt("a", 0, 10);
        model.AddInt("b", 0, 10);
        model.Minimize("a+b");
        var minimum = await Client.Solution(model);
        // minimum.Objective.ShouldBe(0);

        model.Maximize("a+b");
        var maximum = await Client.Solution(model);
        // maximum.Objective.ShouldBe(20);
    }

    [Test]
    public async Task test_solve_unsat_foreach()
    {
        var model = MiniZincModel.ParseString("var 10..20: a; constraint a < 0;");
        await foreach (var result in Client.Solve(model))
        {
            result.Status.ShouldBe(SolveStatus.Unsatisfiable);
        }
    }

    [Test]
    public async Task test_solve_maximize_foreach()
    {
        var model = new MiniZincModel();
        var a = model.AddVariable("a", "10..20");
        var b = model.AddVariable("b", "10..20");
        model.Maximize("a+b");
        await foreach (var result in Client.Solve(model))
        {
            result.Status.ShouldBeOneOf(SolveStatus.Optimal, SolveStatus.Satisfied);
        }
    }

    [Test]
    public async Task test_solve_nqueens_with_timeout()
    {
        var model = MiniZincModel.ParseString(
            """
            int: n = 25;
            array [1..n] of var 1..n: q; % queen in column i is in row q[i]
            include "alldifferent.mzn";
            constraint alldifferent(q);                       % distinct rows
            constraint alldifferent([ q[i] + i | i in 1..n]); % distinct diagonals
            constraint alldifferent([ q[i] - i | i in 1..n]); % upwards+downwards
            % search
            solve :: int_search(q, first_fail, indomain_min) 
                minimize sum(q);
            """
        );

        var timeout = TimeSpan.FromSeconds(1);
        var cts = new CancellationTokenSource(timeout);
        var msg = await Client.Solution(model, "gecode", cts.Token);
        msg.Status.ShouldBe(SolveStatus.Cancelled);
    }

    [Test]
    public async Task test_solve_already_cancelled()
    {
        var model = MiniZincModel.ParseString(
            """
            int: n = 8;
            array [1..n] of var 1..n: q; % queen in column i is in row q[i]
            include "alldifferent.mzn";
            constraint alldifferent(q);                       % distinct rows
            constraint alldifferent([ q[i] + i | i in 1..n]); % distinct diagonals
            constraint alldifferent([ q[i] - i | i in 1..n]); % upwards+downwards
            % search
            solve :: int_search(q, first_fail, indomain_min)
            satisfy;
            """
        );
        var cnc = new CancellationToken(true);
        var msg = await Client.Solution(model, null, cnc);
        msg.Status.ShouldBe(SolveStatus.Cancelled);
    }

    [Test]
    public async Task test_solve_unsat_model()
    {
        var model = MiniZincModel.ParseString(
            """
            int: a = 2;
            int: b = 2;
            constraint a > b;
            """
        );
        var msg = await Client.Solution(model);
        msg.Status.ShouldBe(SolveStatus.Unsatisfiable);
    }

    [Test]
    public async Task test_solve_extra_args()
    {
        var model = MiniZincModel.ParseString(
            """
            var 1..2: a;
            """
        );
        var msg = await Client.Solution(model, default, default, "--no-optimize");

        msg.Status.ShouldBe(SolveStatus.Satisfied);
    }
}
