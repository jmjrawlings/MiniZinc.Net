/*
<auto-generated>
This file has been auto generated by the following command:
dotnet run --project ./build/Make/Make.csproj --make-client-tests
</auto-generated>
*/
#nullable enable

public class SatisfyTests : IClassFixture<ClientFixture>
{
	private readonly MiniZincClient MiniZinc;
	private readonly ITestOutputHelper _output;

	public SatisfyTests(ClientFixture fixture, ITestOutputHelper output)
	{
		MiniZinc = fixture.MiniZinc;
		_output = output;
	}

	async Task TestSatisfy(string path, string solver, List<string> solutions, List<string> args)
	{
		_output.WriteLine(path);
		_output.WriteLine(new string('-',80));

		var model = Model.FromFile(path);
		model.Satisfy();
		_output.WriteLine(model.SourceText);
		_output.WriteLine(new string('-',80));

		var options = SolveOptions.Create(solverId:solver).AddArgs(args);;

		var result = await MiniZinc.Solve(model, options);
		result.IsSuccess.Should().BeTrue();
		var anySolution = false;
		foreach (var dzn in solutions)
		{
			var parsed = Parser.ParseDataString(dzn, out var data);;
			parsed.Ok.Should().BeTrue();
			if (result.Data.Equals(data))
			{
				anySolution = true;
				break;
			}

			_output.WriteLine("The result was not expected:");
			_output.WriteLine("");
			_output.WriteLine(result.Data.Write());
			_output.WriteLine("");
			_output.WriteLine(data.Write());
			_output.WriteLine("");
			_output.WriteLine(new string('-',80));
		}

		anySolution.Should().BeTrue();
		result.Status.Should().Be(SolveStatus.Satisfied);
	}

	[Fact(DisplayName="unit/compilation/par_arg_out_of_bounds.mzn")]
	public async Task test_solve_unit_compilation_par_arg_out_of_bounds()
	{
		var path = "unit/compilation/par_arg_out_of_bounds.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/compilation/poly_overload.mzn")]
	public async Task test_solve_unit_compilation_poly_overload()
	{
		var path = "unit/compilation/poly_overload.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/comprehension_var_ub.mzn")]
	public async Task test_solve_unit_general_comprehension_var_ub()
	{
		var path = "unit/general/comprehension_var_ub.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/enum_order.mzn")]
	public async Task test_solve_unit_general_enum_order()
	{
		var path = "unit/general/enum_order.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/pow_1.mzn")]
	public async Task test_solve_unit_general_pow_1()
	{
		var path = "unit/general/pow_1.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/pow_4.mzn")]
	public async Task test_solve_unit_general_pow_4()
	{
		var path = "unit/general/pow_4.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/pow_bounds.mzn")]
	public async Task test_solve_unit_general_pow_bounds()
	{
		var path = "unit/general/pow_bounds.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/string_length.mzn")]
	public async Task test_solve_unit_general_string_length()
	{
		var path = "unit/general/string_length.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/string_split.mzn")]
	public async Task test_solve_unit_general_string_split()
	{
		var path = "unit/general/string_split.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/general/unicode_file_name_μ.mzn")]
	public async Task test_solve_unit_general_unicode_file_name_μ()
	{
		var path = "unit/general/unicode_file_name_μ.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Theory(DisplayName="unit/globals/typecheck_globals.mzn")]
	[InlineData("chuffed")]
	[InlineData("gecode")]
	[InlineData("gecode_presolver")]
	[InlineData("highs")]
	public async Task test_solve_unit_globals_typecheck_globals(string solver)
	{
		var path = "unit/globals/typecheck_globals.mzn";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/json/coerce_enum_str.mzn")]
	public async Task test_solve_unit_json_coerce_enum_str()
	{
		var path = "unit/json/coerce_enum_str.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--data \"unit/json/coerce_enum_str.json\"",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/json/coerce_indices.mzn")]
	public async Task test_solve_unit_json_coerce_indices()
	{
		var path = "unit/json/coerce_indices.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--data \"unit/json/coerce_indices.json\"",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/json/coerce_set.mzn")]
	public async Task test_solve_unit_json_coerce_set()
	{
		var path = "unit/json/coerce_set.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--data \"unit/json/coerce_set.json\"",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/json/json_array2d_set.mzn")]
	public async Task test_solve_unit_json_json_array2d_set()
	{
		var path = "unit/json/json_array2d_set.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--data \"unit/json/json_array2d_set.json\"",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/json/record_json_input.mzn")]
	public async Task test_solve_unit_json_record_json_input()
	{
		var path = "unit/json/record_json_input.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--data \"unit/json/record_json_input.json\"",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/json/tuple_json_input.mzn")]
	public async Task test_solve_unit_json_tuple_json_input()
	{
		var path = "unit/json/tuple_json_input.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--data \"unit/json/tuple_json_input.json\"",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/optional/conj_absent_1.mzn")]
	public async Task test_solve_unit_optional_conj_absent_1()
	{
		var path = "unit/optional/conj_absent_1.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/optional/conj_absent_2.mzn")]
	public async Task test_solve_unit_optional_conj_absent_2()
	{
		var path = "unit/optional/conj_absent_2.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/optional/opt_array_access.mzn")]
	public async Task test_solve_unit_optional_opt_array_access()
	{
		var path = "unit/optional/opt_array_access.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/optional/opt_math_abs.mzn")]
	public async Task test_solve_unit_optional_opt_math_abs()
	{
		var path = "unit/optional/opt_math_abs.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/optional/opt_math_neg.mzn")]
	public async Task test_solve_unit_optional_opt_math_neg()
	{
		var path = "unit/optional/opt_math_neg.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/binop_mult_gclock.mzn")]
	public async Task test_solve_unit_regression_binop_mult_gclock()
	{
		var path = "unit/regression/binop_mult_gclock.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Theory(DisplayName="unit/regression/bug212.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async Task test_solve_unit_regression_bug212(string solver)
	{
		var path = "unit/regression/bug212.mzn";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/bug635.mzn")]
	public async Task test_solve_unit_regression_bug635()
	{
		var path = "unit/regression/bug635.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-O2",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/dim_1_struct_merge.mzn")]
	public async Task test_solve_unit_regression_dim_1_struct_merge()
	{
		var path = "unit/regression/dim_1_struct_merge.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_639_part2.mzn")]
	public async Task test_solve_unit_regression_github_639_part2()
	{
		var path = "unit/regression/github_639_part2.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_670.mzn")]
	public async Task test_solve_unit_regression_github_670()
	{
		var path = "unit/regression/github_670.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_675b.mzn")]
	public async Task test_solve_unit_regression_github_675b()
	{
		var path = "unit/regression/github_675b.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"--keep-paths",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_726.mzn")]
	public async Task test_solve_unit_regression_github_726()
	{
		var path = "unit/regression/github_726.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_752.mzn")]
	public async Task test_solve_unit_regression_github_752()
	{
		var path = "unit/regression/github_752.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_761.mzn")]
	public async Task test_solve_unit_regression_github_761()
	{
		var path = "unit/regression/github_761.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_778.mzn")]
	public async Task test_solve_unit_regression_github_778()
	{
		var path = "unit/regression/github_778.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_783.mzn")]
	public async Task test_solve_unit_regression_github_783()
	{
		var path = "unit/regression/github_783.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-O2",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_805.mzn")]
	public async Task test_solve_unit_regression_github_805()
	{
		var path = "unit/regression/github_805.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/github_806.mzn")]
	public async Task test_solve_unit_regression_github_806()
	{
		var path = "unit/regression/github_806.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/let_domain_from_generator.mzn")]
	public async Task test_solve_unit_regression_let_domain_from_generator()
	{
		var path = "unit/regression/let_domain_from_generator.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/multi_goal_hierarchy_error.mzn")]
	public async Task test_solve_unit_regression_multi_goal_hierarchy_error()
	{
		var path = "unit/regression/multi_goal_hierarchy_error.mzn";
		var solver = "coin-bc";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/output_2d_array_enum.mzn")]
	public async Task test_solve_unit_regression_output_2d_array_enum()
	{
		var path = "unit/regression/output_2d_array_enum.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/par_opt_dom.mzn")]
	public async Task test_solve_unit_regression_par_opt_dom()
	{
		var path = "unit/regression/par_opt_dom.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/regression/test_bug_637.mzn")]
	public async Task test_solve_unit_regression_test_bug_637()
	{
		var path = "unit/regression/test_bug_637.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/types/enum_decl.mzn")]
	public async Task test_solve_unit_types_enum_decl()
	{
		var path = "unit/types/enum_decl.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/types/overload_inst_tuple_return.mzn")]
	public async Task test_solve_unit_types_overload_inst_tuple_return()
	{
		var path = "unit/types/overload_inst_tuple_return.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/types/par_struct_tiid.mzn")]
	public async Task test_solve_unit_types_par_struct_tiid()
	{
		var path = "unit/types/par_struct_tiid.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/types/record_nested.mzn")]
	public async Task test_solve_unit_types_record_nested()
	{
		var path = "unit/types/record_nested.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/types/struct_opt_supertype.mzn")]
	public async Task test_solve_unit_types_struct_opt_supertype()
	{
		var path = "unit/types/struct_opt_supertype.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/types/type_specialise_param_arrays.mzn")]
	public async Task test_solve_unit_types_type_specialise_param_arrays()
	{
		var path = "unit/types/type_specialise_param_arrays.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="unit/globals/cumulative/github_589.mzn")]
	public async Task test_solve_unit_globals_cumulative_github_589()
	{
		var path = "unit/globals/cumulative/github_589.mzn";
		var solver = "gecode";
		var solutions = new List<string>();
		var args = new List<string>{
			"-G std",
		};
		await TestSatisfy(path, solver, solutions, args);
	}

	[Theory(DisplayName="unit/globals/nvalue/globals_nvalue.mzn")]
	[InlineData("coin-bc")]
	[InlineData("highs")]
	public async Task test_solve_unit_globals_nvalue_globals_nvalue_2(string solver)
	{
		var path = "unit/globals/nvalue/globals_nvalue.mzn";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="examples/radiation.mzn")]
	public async Task test_solve_examples_radiation_2()
	{
		var path = "examples/radiation.mzn";
		var solver = "coin-bc";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

	[Fact(DisplayName="examples/wolf_goat_cabbage.mzn")]
	public async Task test_solve_examples_wolf_goat_cabbage()
	{
		var path = "examples/wolf_goat_cabbage.mzn";
		var solver = "coin-bc";
		var solutions = new List<string>();
		var args = new List<string>();
		await TestSatisfy(path, solver, solutions, args);
	}

}

