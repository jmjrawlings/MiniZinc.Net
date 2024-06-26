/*
<auto-generated>
This file has been auto generated by the following command:
dotnet run --project ./build/Make/Make.csproj --make-client-tests
</auto-generated>
*/
#nullable enable

public class ClientUnsatisfiableTests : IClassFixture<ClientFixture>
{
	private readonly MiniZincClient MiniZinc;
	private readonly ITestOutputHelper _output;

	public ClientUnsatisfiableTests(ClientFixture fixture, ITestOutputHelper output)
	{
		MiniZinc = fixture.MiniZinc;
		_output = output;
	}

	async Task Test(string path, string solver, params string[] args)
	{
		_output.WriteLine(path);
		_output.WriteLine(new string('-',80));

		var model = Model.FromFile(path);
		_output.WriteLine(model.SourceText);
		_output.WriteLine(new string('-',80));

		var options = SolveOptions.Create(solverId:solver);
		options = options.AddArgs(args);

		var result = await MiniZinc.Solve(model, options);
		result.IsSuccess.Should().BeFalse();
		result.Status.Should().Be(SolveStatus.Unsatisfiable);
	}

	[Fact(DisplayName="unit/regression/bind-defines-var.mzn")]
	public async Task test_solve_unit_regression_bind_defines_var()
	{
		var path = "unit/regression/bind-defines-var.mzn";
		var solver = "gecode";
		await Test(path, solver,"-G std");
	}

	[Fact(DisplayName="unit/regression/github_661_part1.mzn")]
	public async Task test_solve_unit_regression_github_661_part1()
	{
		var path = "unit/regression/github_661_part1.mzn";
		var solver = "gecode";
		await Test(path, solver);
	}

	[Fact(DisplayName="unit/regression/github_666.mzn")]
	public async Task test_solve_unit_regression_github_666()
	{
		var path = "unit/regression/github_666.mzn";
		var solver = "gecode";
		await Test(path, solver,"-G std");
	}

	[Fact(DisplayName="unit/regression/github_765.mzn")]
	public async Task test_solve_unit_regression_github_765()
	{
		var path = "unit/regression/github_765.mzn";
		var solver = "gecode";
		await Test(path, solver);
	}

	[Fact(DisplayName="unit/regression/github_785.mzn")]
	public async Task test_solve_unit_regression_github_785()
	{
		var path = "unit/regression/github_785.mzn";
		var solver = "gecode";
		await Test(path, solver,"-G std");
	}

	[Fact(DisplayName="unit/types/struct_domain_5.mzn")]
	public async Task test_solve_unit_types_struct_domain_5()
	{
		var path = "unit/types/struct_domain_5.mzn";
		var solver = "gecode";
		await Test(path, solver);
	}

	[Fact(DisplayName="unit/types/struct_domain_6.mzn")]
	public async Task test_solve_unit_types_struct_domain_6()
	{
		var path = "unit/types/struct_domain_6.mzn";
		var solver = "gecode";
		await Test(path, solver);
	}

	[Fact(DisplayName="unit/types/struct_return_ti_4.mzn")]
	public async Task test_solve_unit_types_struct_return_ti_4()
	{
		var path = "unit/types/struct_return_ti_4.mzn";
		var solver = "gecode";
		await Test(path, solver);
	}

}

