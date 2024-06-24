/*
THIS FILE WAS GENERATED BY THE FOLLOWING COMMAND

dotnet run --project ./build/Make/Make.csproj --make-client-tests
*/
public class ClientSatisfyTests : IClassFixture<ClientFixture> {
	private readonly MiniZincClient MiniZinc;
	private readonly ITestOutputHelper _output;
	public ClientSatisfyTests(ClientFixture fixture, ITestOutputHelper output){
		MiniZinc = fixture.Client;
		_output = output;
	}
	async Task Test(string path, string solver, params string[]? args){
		_output.WriteLine(path);
		_output.WriteLine(new string('-',80));
		var model = Model.FromFile(path);
		_output.WriteLine(model.SourceText);
		_output.WriteLine(new string('-',80));
		foreach (var warn in model.Warnings) {
			_output.WriteLine(warn);
		}
		var options = SolveOptions.Create(solverId:solver);
		if (args is not null) {
			options = options.AddArgs(args);
		}
		var result = await MiniZinc.Solve(model, options);
		result.IsSuccess.Should().BeTrue();
		result.Status.Should().Be(SolveStatus.Satisfied);
	}
	[Theory(DisplayName="unit/compilation/par_arg_out_of_bounds.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_compilation_par_arg_out_of_bounds(string solver) {
		var path = "unit/compilation/par_arg_out_of_bounds.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/compilation/poly_overload.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_compilation_poly_overload(string solver) {
		var path = "unit/compilation/poly_overload.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/general/comprehension_var_ub.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_general_comprehension_var_ub(string solver) {
		var path = "unit/general/comprehension_var_ub.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/general/enum_order.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_general_enum_order(string solver) {
		var path = "unit/general/enum_order.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/general/pow_1.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_general_pow_1(string solver) {
		var path = "unit/general/pow_1.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/general/pow_4.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_general_pow_4(string solver) {
		var path = "unit/general/pow_4.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/general/pow_bounds.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_general_pow_bounds(string solver) {
		var path = "unit/general/pow_bounds.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/general/unicode_file_name_μ.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_general_unicode_file_name_μ(string solver) {
		var path = "unit/general/unicode_file_name_μ.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/globals/typecheck_globals.mzn")]
	[InlineData("chuffed")]
	[InlineData("gecode")]
	[InlineData("gecode_presolver")]
	[InlineData("highs")]
	public async Task test_solve_unit_globals_typecheck_globals(string solver) {
		var path = "unit/globals/typecheck_globals.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/json/coerce_enum_str.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_json_coerce_enum_str(string solver) {
		var path = "unit/json/coerce_enum_str.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/json/coerce_indices.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_json_coerce_indices(string solver) {
		var path = "unit/json/coerce_indices.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/json/coerce_set.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_json_coerce_set(string solver) {
		var path = "unit/json/coerce_set.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/json/json_array2d_set.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_json_json_array2d_set(string solver) {
		var path = "unit/json/json_array2d_set.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/json/record_json_input.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_json_record_json_input(string solver) {
		var path = "unit/json/record_json_input.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/json/tuple_json_input.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_json_tuple_json_input(string solver) {
		var path = "unit/json/tuple_json_input.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/optional/conj_absent_1.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_optional_conj_absent_1(string solver) {
		var path = "unit/optional/conj_absent_1.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/optional/conj_absent_2.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_optional_conj_absent_2(string solver) {
		var path = "unit/optional/conj_absent_2.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/optional/opt_array_access.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_optional_opt_array_access(string solver) {
		var path = "unit/optional/opt_array_access.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/binop_mult_gclock.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_binop_mult_gclock(string solver) {
		var path = "unit/regression/binop_mult_gclock.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/bug212.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async Task test_solve_unit_regression_bug212(string solver) {
		var path = "unit/regression/bug212.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/bug635.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_bug635(string solver) {
		var path = "unit/regression/bug635.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_639_part2.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_639_part2(string solver) {
		var path = "unit/regression/github_639_part2.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_670.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_670(string solver) {
		var path = "unit/regression/github_670.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_675b.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_675b(string solver) {
		var path = "unit/regression/github_675b.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_726.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_726(string solver) {
		var path = "unit/regression/github_726.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_752.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_752(string solver) {
		var path = "unit/regression/github_752.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_761.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_761(string solver) {
		var path = "unit/regression/github_761.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_778.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_778(string solver) {
		var path = "unit/regression/github_778.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/github_783.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_github_783(string solver) {
		var path = "unit/regression/github_783.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/let_domain_from_generator.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_let_domain_from_generator(string solver) {
		var path = "unit/regression/let_domain_from_generator.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/multi_goal_hierarchy_error.mzn")]
	[InlineData("coin-bc")]
	public async Task test_solve_unit_regression_multi_goal_hierarchy_error(string solver) {
		var path = "unit/regression/multi_goal_hierarchy_error.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/output_2d_array_enum.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_output_2d_array_enum(string solver) {
		var path = "unit/regression/output_2d_array_enum.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/par_opt_dom.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_par_opt_dom(string solver) {
		var path = "unit/regression/par_opt_dom.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/regression/test_bug_637.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_regression_test_bug_637(string solver) {
		var path = "unit/regression/test_bug_637.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/types/enum_decl.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_types_enum_decl(string solver) {
		var path = "unit/types/enum_decl.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/types/overload_inst_tuple_return.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_types_overload_inst_tuple_return(string solver) {
		var path = "unit/types/overload_inst_tuple_return.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/types/par_struct_tiid.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_types_par_struct_tiid(string solver) {
		var path = "unit/types/par_struct_tiid.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/types/record_nested.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_types_record_nested(string solver) {
		var path = "unit/types/record_nested.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/globals/cumulative/github_589.mzn")]
	[InlineData("gecode")]
	public async Task test_solve_unit_globals_cumulative_github_589(string solver) {
		var path = "unit/globals/cumulative/github_589.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="unit/globals/nvalue/globals_nvalue.mzn")]
	[InlineData("coin-bc")]
	[InlineData("highs")]
	public async Task test_solve_unit_globals_nvalue_globals_nvalue_2(string solver) {
		var path = "unit/globals/nvalue/globals_nvalue.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="examples/radiation.mzn")]
	[InlineData("coin-bc")]
	public async Task test_solve_examples_radiation_2(string solver) {
		var path = "examples/radiation.mzn";
		await Test(path, solver);
	}
	[Theory(DisplayName="examples/wolf_goat_cabbage.mzn")]
	[InlineData("coin-bc")]
	public async Task test_solve_examples_wolf_goat_cabbage(string solver) {
		var path = "examples/wolf_goat_cabbage.mzn";
		await Test(path, solver);
	}
	}