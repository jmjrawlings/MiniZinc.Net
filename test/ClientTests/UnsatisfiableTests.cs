/*
<auto-generated>
This file has been auto generated by the following command:
dotnet run --project ./build/Make/Make.csproj --make-client-tests
</auto-generated>
*/
#nullable enable

public class UnsatisfiableTests : IClassFixture<ClientFixture>
{
	private readonly MiniZincClient MiniZinc;
	private readonly ITestOutputHelper _output;

	public UnsatisfiableTests(ClientFixture fixture, ITestOutputHelper output)
	{
		MiniZinc = fixture.MiniZinc;
		_output = output;
	}

	async Task TestUnsatisfiable(string path, string solver, List<string>? args)
	{
		_output.WriteLine(path);
		_output.WriteLine(new string('-',80));

		var model = Model.FromFile(path);
		_output.WriteLine(model.SourceText);
		_output.WriteLine(new string('-',80));

		var options = SolveOptions.Create(solverId:solver).AddArgs(args);;

		var result = await MiniZinc.Solve(model, options);
		result.IsSuccess.Should().BeFalse();
		result.Status.Should().Be(SolveStatus.Unsatisfiable);
	}

	[Fact(DisplayName="unit/general/bind_par_opt.mzn")]
	public async Task test_solve_unit_general_bind_par_opt()
	{
		var path = "unit/general/bind_par_opt.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/regression/bind-defines-var.mzn")]
	public async Task test_solve_unit_regression_bind_defines_var()
	{
		var path = "unit/regression/bind-defines-var.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-G std",
		};
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/regression/github_661_part1.mzn")]
	public async Task test_solve_unit_regression_github_661_part1()
	{
		var path = "unit/regression/github_661_part1.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/regression/github_666.mzn")]
	public async Task test_solve_unit_regression_github_666()
	{
		var path = "unit/regression/github_666.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-G std",
		};
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/regression/github_765.mzn")]
	public async Task test_solve_unit_regression_github_765()
	{
		var path = "unit/regression/github_765.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/regression/github_785.mzn")]
	public async Task test_solve_unit_regression_github_785()
	{
		var path = "unit/regression/github_785.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-G std",
		};
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/regression/github_798.mzn")]
	public async Task test_solve_unit_regression_github_798()
	{
		var path = "unit/regression/github_798.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-G gecode_presolver",
		};
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/types/struct_domain_5.mzn")]
	public async Task test_solve_unit_types_struct_domain_5()
	{
		var path = "unit/types/struct_domain_5.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/types/struct_domain_6.mzn")]
	public async Task test_solve_unit_types_struct_domain_6()
	{
		var path = "unit/types/struct_domain_6.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestUnsatisfiable(path, solver, args);	}

	[Fact(DisplayName="unit/types/struct_return_ti_4.mzn")]
	public async Task test_solve_unit_types_struct_return_ti_4()
	{
		var path = "unit/types/struct_return_ti_4.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestUnsatisfiable(path, solver, args);	}

}

