public class SolverIntegrationTests {

	private readonly MiniZincClient _client;

	public SolverIntegrationTests(ClientFixture fixture, ITestOutputHelper output) {
		_client = fixture.Client;
	}

	[Theory(DisplayName="unit/test-globals-float.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_test_globals_float(string solver) {
		var path = "unit/test-globals-float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/aggregation.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_aggregation(string solver) {
		var path = "unit/compilation/aggregation.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/annotate_bool_literal.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_annotate_bool_literal(string solver) {
		var path = "unit/compilation/annotate_bool_literal.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/annotate_from_array.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_annotate_from_array(string solver) {
		var path = "unit/compilation/annotate_from_array.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/assert_dbg_flag.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_assert_dbg_flag(string solver) {
		var path = "unit/compilation/assert_dbg_flag.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/assert_dbg_ignore.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_assert_dbg_ignore(string solver) {
		var path = "unit/compilation/assert_dbg_ignore.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/call_root_ctx.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_call_root_ctx(string solver) {
		var path = "unit/compilation/call_root_ctx.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/chain_compr_mult_clause.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_chain_compr_mult_clause(string solver) {
		var path = "unit/compilation/chain_compr_mult_clause.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/clause_disable_hr.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_clause_disable_hr(string solver) {
		var path = "unit/compilation/clause_disable_hr.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/commutative_cse.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_commutative_cse(string solver) {
		var path = "unit/compilation/commutative_cse.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/count_rewrite.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_count_rewrite(string solver) {
		var path = "unit/compilation/count_rewrite.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/debug_mode_false.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_debug_mode_false(string solver) {
		var path = "unit/compilation/debug_mode_false.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/debug_mode_true.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_debug_mode_true(string solver) {
		var path = "unit/compilation/debug_mode_true.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/defines_var_cycle_breaking.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_defines_var_cycle_breaking(string solver) {
		var path = "unit/compilation/defines_var_cycle_breaking.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/float_inf_range_dom.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_float_inf_range_dom(string solver) {
		var path = "unit/compilation/float_inf_range_dom.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/has_ann.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_has_ann(string solver) {
		var path = "unit/compilation/has_ann.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/if_then_no_else.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_if_then_no_else(string solver) {
		var path = "unit/compilation/if_then_no_else.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/implied_exists_chain.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_implied_exists_chain(string solver) {
		var path = "unit/compilation/implied_exists_chain.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/implied_hr.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_implied_hr(string solver) {
		var path = "unit/compilation/implied_hr.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/int_inf_dom.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_int_inf_dom(string solver) {
		var path = "unit/compilation/int_inf_dom.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/multiple_neg.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_multiple_neg(string solver) {
		var path = "unit/compilation/multiple_neg.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/optimization.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_optimization(string solver) {
		var path = "unit/compilation/optimization.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/optimization.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_optimization_case_2(string solver) {
		var path = "unit/compilation/optimization.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/optimization.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_optimization_case_3(string solver) {
		var path = "unit/compilation/optimization.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/optimization.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_optimization_case_4(string solver) {
		var path = "unit/compilation/optimization.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/par_arg_out_of_bounds.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_par_arg_out_of_bounds(string solver) {
		var path = "unit/compilation/par_arg_out_of_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/poly_overload.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_poly_overload(string solver) {
		var path = "unit/compilation/poly_overload.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/quoted_id_flatzinc.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_quoted_id_flatzinc(string solver) {
		var path = "unit/compilation/quoted_id_flatzinc.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/set2iter.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_compilation_set2iter(string solver) {
		var path = "unit/compilation/set2iter.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/compilation/time_limit.mzn")]
	[InlineData("gecode")]
	public async void test_unit_compilation_time_limit(string solver) {
		var path = "unit/compilation/time_limit.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_div10.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_division_test_div10(string solver) {
		var path = "unit/division/test_div10.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_div11.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_division_test_div11(string solver) {
		var path = "unit/division/test_div11.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_div12.mzn")]
	[InlineData("gecode")]
	public async void test_unit_division_test_div12(string solver) {
		var path = "unit/division/test_div12.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_div8.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_division_test_div8(string solver) {
		var path = "unit/division/test_div8.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_div_mod_bounds.mzn")]
	[InlineData("gecode")]
	[InlineData("gurobi")]
	[InlineData("scip")]
	public async void test_unit_division_test_div_mod_bounds(string solver) {
		var path = "unit/division/test_div_mod_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_fldiv_01.mzn")]
	[InlineData("gurobi")]
	[InlineData("scip")]
	public async void test_unit_division_test_fldiv_01(string solver) {
		var path = "unit/division/test_fldiv_01.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/division/test_fldiv_02.mzn")]
	[InlineData("gurobi")]
	[InlineData("scip")]
	public async void test_unit_division_test_fldiv_02(string solver) {
		var path = "unit/division/test_fldiv_02.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/annotated_expression_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_annotated_expression_1(string solver) {
		var path = "unit/general/annotated_expression_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_access_out_of_bounds_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_access_out_of_bounds_1(string solver) {
		var path = "unit/general/array_access_out_of_bounds_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_access_out_of_bounds_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_access_out_of_bounds_2(string solver) {
		var path = "unit/general/array_access_out_of_bounds_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_access_record_out_of_bounds.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_access_record_out_of_bounds(string solver) {
		var path = "unit/general/array_access_record_out_of_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_access_tuple_out_of_bounds.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_access_tuple_out_of_bounds(string solver) {
		var path = "unit/general/array_access_tuple_out_of_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_intersect_context.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_intersect_context(string solver) {
		var path = "unit/general/array_intersect_context.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_param_non_array_return.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_param_non_array_return(string solver) {
		var path = "unit/general/array_param_non_array_return.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/array_union_intersect_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_array_union_intersect_enum(string solver) {
		var path = "unit/general/array_union_intersect_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/bin_pack_multiobj.mzn")]
	[InlineData("gurobi")]
	public async void test_unit_general_bin_pack_multiobj(string solver) {
		var path = "unit/general/bin_pack_multiobj.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/builtins_arg_max.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_builtins_arg_max(string solver) {
		var path = "unit/general/builtins_arg_max.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/builtins_debug.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_builtins_debug(string solver) {
		var path = "unit/general/builtins_debug.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/comprehension_var_ub.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_comprehension_var_ub(string solver) {
		var path = "unit/general/comprehension_var_ub.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/cse_ctx.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_cse_ctx(string solver) {
		var path = "unit/general/cse_ctx.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/enum_constructor_quoting.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_enum_constructor_quoting(string solver) {
		var path = "unit/general/enum_constructor_quoting.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/enum_order.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_enum_order(string solver) {
		var path = "unit/general/enum_order.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/enum_out_of_range_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_enum_out_of_range_1(string solver) {
		var path = "unit/general/enum_out_of_range_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/enum_out_of_range_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_enum_out_of_range_2(string solver) {
		var path = "unit/general/enum_out_of_range_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/eval_par_opt_set.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_eval_par_opt_set(string solver) {
		var path = "unit/general/eval_par_opt_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/fix_struct.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_fix_struct(string solver) {
		var path = "unit/general/fix_struct.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/function_param_out_of_range.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_function_param_out_of_range(string solver) {
		var path = "unit/general/function_param_out_of_range.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/function_return_out_of_range.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_function_return_out_of_range(string solver) {
		var path = "unit/general/function_return_out_of_range.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/function_return_out_of_range_opt.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_function_return_out_of_range_opt(string solver) {
		var path = "unit/general/function_return_out_of_range_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/infinite_domain_bind.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_infinite_domain_bind(string solver) {
		var path = "unit/general/infinite_domain_bind.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/json_ignore.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_json_ignore(string solver) {
		var path = "unit/general/json_ignore.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/let_struct_domain.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_let_struct_domain(string solver) {
		var path = "unit/general/let_struct_domain.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_exists.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_exists(string solver) {
		var path = "unit/general/md_exists.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_exists.mzn")]
	[InlineData("cbc")]
	public async void test_unit_general_md_exists_case_2(string solver) {
		var path = "unit/general/md_exists.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_forall.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_md_forall(string solver) {
		var path = "unit/general/md_forall.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_forall.mzn")]
	[InlineData("cbc")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_forall_case_2(string solver) {
		var path = "unit/general/md_forall.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_iffall.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_iffall(string solver) {
		var path = "unit/general/md_iffall.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_iffall.mzn")]
	[InlineData("cbc")]
	public async void test_unit_general_md_iffall_case_2(string solver) {
		var path = "unit/general/md_iffall.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_product_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_product_int(string solver) {
		var path = "unit/general/md_product_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_sum_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_md_sum_float(string solver) {
		var path = "unit/general/md_sum_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_sum_float.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_sum_float_case_2(string solver) {
		var path = "unit/general/md_sum_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_sum_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_sum_int(string solver) {
		var path = "unit/general/md_sum_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/md_xorall.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_md_xorall(string solver) {
		var path = "unit/general/md_xorall.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/missing_ozn_decl.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_missing_ozn_decl(string solver) {
		var path = "unit/general/missing_ozn_decl.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/mortgage.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_general_mortgage(string solver) {
		var path = "unit/general/mortgage.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/overload_bottom.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_overload_bottom(string solver) {
		var path = "unit/general/overload_bottom.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/param_out_of_range_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_param_out_of_range_float(string solver) {
		var path = "unit/general/param_out_of_range_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/param_out_of_range_int.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_param_out_of_range_int(string solver) {
		var path = "unit/general/param_out_of_range_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_1(string solver) {
		var path = "unit/general/pow_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2_case_2(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2_case_3(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2_case_4(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2_case_5(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2_case_6(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_2_case_7(string solver) {
		var path = "unit/general/pow_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_3(string solver) {
		var path = "unit/general/pow_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_3_case_2(string solver) {
		var path = "unit/general/pow_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_3_case_3(string solver) {
		var path = "unit/general/pow_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_3_case_4(string solver) {
		var path = "unit/general/pow_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_3_case_5(string solver) {
		var path = "unit/general/pow_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_3_case_6(string solver) {
		var path = "unit/general/pow_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_4.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_4(string solver) {
		var path = "unit/general/pow_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/pow_bounds.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_pow_bounds(string solver) {
		var path = "unit/general/pow_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/quoted_id_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_quoted_id_1(string solver) {
		var path = "unit/general/quoted_id_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/quoted_id_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_quoted_id_2(string solver) {
		var path = "unit/general/quoted_id_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/quoted_id_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_quoted_id_3(string solver) {
		var path = "unit/general/quoted_id_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/quoted_id_4.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_quoted_id_4(string solver) {
		var path = "unit/general/quoted_id_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/range_var_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_range_var_enum(string solver) {
		var path = "unit/general/range_var_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/stack_overflow.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_stack_overflow(string solver) {
		var path = "unit/general/stack_overflow.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test-search1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	public async void test_unit_general_test_search1(string solver) {
		var path = "unit/general/test-search1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_mod_bounds.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_test_mod_bounds(string solver) {
		var path = "unit/general/test_mod_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_negated_and.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_negated_and(string solver) {
		var path = "unit/general/test_negated_and.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_negated_and_or.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_negated_and_or(string solver) {
		var path = "unit/general/test_negated_and_or.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_negated_let_good_2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_negated_let_good_2(string solver) {
		var path = "unit/general/test_negated_let_good_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_negated_or.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_negated_or(string solver) {
		var path = "unit/general/test_negated_or.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_queens.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_queens(string solver) {
		var path = "unit/general/test_queens.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_rounding_a.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_test_rounding_a(string solver) {
		var path = "unit/general/test_rounding_a.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_rounding_b.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_test_rounding_b(string solver) {
		var path = "unit/general/test_rounding_b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_rounding_c.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_test_rounding_c(string solver) {
		var path = "unit/general/test_rounding_c.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_same.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_general_test_same(string solver) {
		var path = "unit/general/test_same.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_set_lt_1.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_set_lt_1(string solver) {
		var path = "unit/general/test_set_lt_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_set_lt_2.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_set_lt_2(string solver) {
		var path = "unit/general/test_set_lt_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_set_lt_2.mzn")]
	[InlineData("cbc")]
	public async void test_unit_general_test_set_lt_2_case_2(string solver) {
		var path = "unit/general/test_set_lt_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_set_lt_3.mzn")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_general_test_set_lt_3(string solver) {
		var path = "unit/general/test_set_lt_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_times_int_float_eq.mzn")]
	[InlineData("gurobi")]
	[InlineData("scip")]
	public async void test_unit_general_test_times_int_float_eq(string solver) {
		var path = "unit/general/test_times_int_float_eq.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_times_int_float_eq__defaultopt.mzn")]
	[InlineData("gurobi")]
	[InlineData("scip")]
	public async void test_unit_general_test_times_int_float_eq__defaultopt(string solver) {
		var path = "unit/general/test_times_int_float_eq__defaultopt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_to_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_test_to_enum(string solver) {
		var path = "unit/general/test_to_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_undefined_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_test_undefined_enum(string solver) {
		var path = "unit/general/test_undefined_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_var_prod.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_general_test_var_prod(string solver) {
		var path = "unit/general/test_var_prod.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/test_var_set_element.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	public async void test_unit_general_test_var_set_element(string solver) {
		var path = "unit/general/test_var_set_element.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/general/unicode_file_name_μ.mzn")]
	[InlineData("gecode")]
	public async void test_unit_general_unicode_file_name_μ(string solver) {
		var path = "unit/general/unicode_file_name_μ.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/typecheck_globals.mzn")]
	[InlineData("chuffed")]
	[InlineData("gecode")]
	[InlineData("gecode_presolver")]
	[InlineData("highs")]
	public async void test_unit_globals_typecheck_globals(string solver) {
		var path = "unit/globals/typecheck_globals.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/anon_enum_json.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_anon_enum_json(string solver) {
		var path = "unit/json/anon_enum_json.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/anon_enum_json.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_anon_enum_json_case_2(string solver) {
		var path = "unit/json/anon_enum_json.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/coerce_enum_str.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_coerce_enum_str(string solver) {
		var path = "unit/json/coerce_enum_str.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/coerce_enum_str_err.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_coerce_enum_str_err(string solver) {
		var path = "unit/json/coerce_enum_str_err.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/coerce_indices.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_coerce_indices(string solver) {
		var path = "unit/json/coerce_indices.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/coerce_set.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_coerce_set(string solver) {
		var path = "unit/json/coerce_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/enum_constructor_basic.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_enum_constructor_basic(string solver) {
		var path = "unit/json/enum_constructor_basic.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/enum_constructor_basic_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_enum_constructor_basic_2(string solver) {
		var path = "unit/json/enum_constructor_basic_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/enum_constructor_int.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_enum_constructor_int(string solver) {
		var path = "unit/json/enum_constructor_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/enum_constructor_nested.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_enum_constructor_nested(string solver) {
		var path = "unit/json/enum_constructor_nested.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/enum_escaping.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_enum_escaping(string solver) {
		var path = "unit/json/enum_escaping.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/float_json_exponent.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_float_json_exponent(string solver) {
		var path = "unit/json/float_json_exponent.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/json_array2d_set.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_json_array2d_set(string solver) {
		var path = "unit/json/json_array2d_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/json_enum_def.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_json_enum_def(string solver) {
		var path = "unit/json/json_enum_def.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/json_input_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_json_input_1(string solver) {
		var path = "unit/json/json_input_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/json_unicode_escapes.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_json_unicode_escapes(string solver) {
		var path = "unit/json/json_unicode_escapes.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/mult_dim_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_mult_dim_enum(string solver) {
		var path = "unit/json/mult_dim_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/record_json_input.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_record_json_input(string solver) {
		var path = "unit/json/record_json_input.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/json/tuple_json_input.mzn")]
	[InlineData("gecode")]
	public async void test_unit_json_tuple_json_input(string solver) {
		var path = "unit/json/tuple_json_input.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/complete.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_complete(string solver) {
		var path = "unit/on_restart/complete.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/last_val_bool.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_last_val_bool(string solver) {
		var path = "unit/on_restart/last_val_bool.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/last_val_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_on_restart_last_val_float(string solver) {
		var path = "unit/on_restart/last_val_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/last_val_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_last_val_int(string solver) {
		var path = "unit/on_restart/last_val_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/last_val_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_last_val_set(string solver) {
		var path = "unit/on_restart/last_val_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/sol_bool.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_sol_bool(string solver) {
		var path = "unit/on_restart/sol_bool.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/sol_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_on_restart_sol_float(string solver) {
		var path = "unit/on_restart/sol_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/sol_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_sol_int(string solver) {
		var path = "unit/on_restart/sol_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/on_restart/sol_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_on_restart_sol_set(string solver) {
		var path = "unit/on_restart/sol_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/conj_absent_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_conj_absent_1(string solver) {
		var path = "unit/optional/conj_absent_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/conj_absent_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_conj_absent_2(string solver) {
		var path = "unit/optional/conj_absent_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/fn_return_array_absent.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_fn_return_array_absent(string solver) {
		var path = "unit/optional/fn_return_array_absent.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/opt_array_access.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_opt_array_access(string solver) {
		var path = "unit/optional/opt_array_access.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-bool-2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_opt_bool_2(string solver) {
		var path = "unit/optional/test-opt-bool-2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-bool-3.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_opt_bool_3(string solver) {
		var path = "unit/optional/test-opt-bool-3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-bool-4.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_opt_bool_4(string solver) {
		var path = "unit/optional/test-opt-bool-4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-bool-5.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_opt_bool_5(string solver) {
		var path = "unit/optional/test-opt-bool-5.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-bool-6.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_opt_bool_6(string solver) {
		var path = "unit/optional/test-opt-bool-6.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-float-1.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_optional_test_opt_float_1(string solver) {
		var path = "unit/optional/test-opt-float-1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-if-then-else.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_opt_if_then_else(string solver) {
		var path = "unit/optional/test-opt-if-then-else.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-int-2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_opt_int_2(string solver) {
		var path = "unit/optional/test-opt-int-2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test-opt-int-3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_opt_int_3(string solver) {
		var path = "unit/optional/test-opt-int-3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_bug_456.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_bug_456(string solver) {
		var path = "unit/optional/test_bug_456.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_count_set.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_count_set(string solver) {
		var path = "unit/optional/test_count_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_deopt_absent.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_deopt_absent(string solver) {
		var path = "unit/optional/test_deopt_absent.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_if_then_else_opt_bool.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_if_then_else_opt_bool(string solver) {
		var path = "unit/optional/test_if_then_else_opt_bool.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_if_then_else_opt_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_if_then_else_opt_float(string solver) {
		var path = "unit/optional/test_if_then_else_opt_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_if_then_else_opt_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_if_then_else_opt_int(string solver) {
		var path = "unit/optional/test_if_then_else_opt_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_if_then_else_var_opt_bool.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_if_then_else_var_opt_bool(string solver) {
		var path = "unit/optional/test_if_then_else_var_opt_bool.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_if_then_else_var_opt_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_if_then_else_var_opt_float(string solver) {
		var path = "unit/optional/test_if_then_else_var_opt_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_if_then_else_var_opt_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_optional_test_if_then_else_var_opt_int(string solver) {
		var path = "unit/optional/test_if_then_else_var_opt_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_optional_not_absent.mzn")]
	[InlineData("chuffed")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_optional_not_absent(string solver) {
		var path = "unit/optional/test_optional_not_absent.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_opt_comprehension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_opt_comprehension(string solver) {
		var path = "unit/optional/test_opt_comprehension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_opt_dom_empty.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_opt_dom_empty(string solver) {
		var path = "unit/optional/test_opt_dom_empty.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/optional/test_opt_dom_empty_no_absent_zero.mzn")]
	[InlineData("gecode")]
	public async void test_unit_optional_test_opt_dom_empty_no_absent_zero(string solver) {
		var path = "unit/optional/test_opt_dom_empty_no_absent_zero.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/arg-reif-output.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_arg_reif_output(string solver) {
		var path = "unit/output/arg-reif-output.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/array_of_array.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_array_of_array(string solver) {
		var path = "unit/output/array_of_array.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/ctx_ann.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_ctx_ann(string solver) {
		var path = "unit/output/ctx_ann.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/dzn_output_array.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_dzn_output_array(string solver) {
		var path = "unit/output/dzn_output_array.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/enum_constructor_functions.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_enum_constructor_functions(string solver) {
		var path = "unit/output/enum_constructor_functions.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/json_ann.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_json_ann(string solver) {
		var path = "unit/output/json_ann.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/local_output.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_local_output(string solver) {
		var path = "unit/output/local_output.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_annotations_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_annotations_1(string solver) {
		var path = "unit/output/output_annotations_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_annotations_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_annotations_2(string solver) {
		var path = "unit/output/output_annotations_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_annotations_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_annotations_3(string solver) {
		var path = "unit/output/output_annotations_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_annotations_4.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_annotations_4(string solver) {
		var path = "unit/output/output_annotations_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_1(string solver) {
		var path = "unit/output/output_sections_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_1_case_2(string solver) {
		var path = "unit/output/output_sections_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_1_case_3(string solver) {
		var path = "unit/output/output_sections_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_2(string solver) {
		var path = "unit/output/output_sections_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_2_case_2(string solver) {
		var path = "unit/output/output_sections_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_2_case_3(string solver) {
		var path = "unit/output/output_sections_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_3(string solver) {
		var path = "unit/output/output_sections_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_4.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_4(string solver) {
		var path = "unit/output/output_sections_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_5.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_5(string solver) {
		var path = "unit/output/output_sections_5.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_6.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_6(string solver) {
		var path = "unit/output/output_sections_6.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/output_sections_7.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_output_sections_7(string solver) {
		var path = "unit/output/output_sections_7.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/quoted_id_ozn.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_quoted_id_ozn(string solver) {
		var path = "unit/output/quoted_id_ozn.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/record_access_printing.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_record_access_printing(string solver) {
		var path = "unit/output/record_access_printing.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/test-in-output.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_output_test_in_output(string solver) {
		var path = "unit/output/test-in-output.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/output/var_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_output_var_enum(string solver) {
		var path = "unit/output/var_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/param_file/param_file_array.mzn")]
	[InlineData("gecode")]
	public async void test_unit_param_file_param_file_array(string solver) {
		var path = "unit/param_file/param_file_array.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/param_file/param_file_blacklist.mzn")]
	[InlineData("gecode")]
	public async void test_unit_param_file_param_file_blacklist(string solver) {
		var path = "unit/param_file/param_file_blacklist.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/param_file/param_file_blacklist.mzn")]
	[InlineData("gecode")]
	public async void test_unit_param_file_param_file_blacklist_case_2(string solver) {
		var path = "unit/param_file/param_file_blacklist.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/param_file/param_file_nested_object.mzn")]
	[InlineData("gecode")]
	public async void test_unit_param_file_param_file_nested_object(string solver) {
		var path = "unit/param_file/param_file_nested_object.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/param_file/param_file_recursive.mzn")]
	[InlineData("gecode")]
	public async void test_unit_param_file_param_file_recursive(string solver) {
		var path = "unit/param_file/param_file_recursive.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/param_file/param_file_resolution.mzn")]
	[InlineData("gecode")]
	public async void test_unit_param_file_param_file_resolution(string solver) {
		var path = "unit/param_file/param_file_resolution.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/arg-reif-array-float.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_arg_reif_array_float(string solver) {
		var path = "unit/regression/arg-reif-array-float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/arg-reif-array-int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_arg_reif_array_int(string solver) {
		var path = "unit/regression/arg-reif-array-int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/arg-reif-float.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_arg_reif_float(string solver) {
		var path = "unit/regression/arg-reif-float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/arg-reif-int-set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_arg_reif_int_set(string solver) {
		var path = "unit/regression/arg-reif-int-set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/arg-reif-int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_arg_reif_int(string solver) {
		var path = "unit/regression/arg-reif-int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/array_of_empty_sets.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_array_of_empty_sets(string solver) {
		var path = "unit/regression/array_of_empty_sets.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/array_set_element_nosets.mzn")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_array_set_element_nosets(string solver) {
		var path = "unit/regression/array_set_element_nosets.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/array_var_set_element_nosets.mzn")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_array_var_set_element_nosets(string solver) {
		var path = "unit/regression/array_var_set_element_nosets.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/assign_reverse_map.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_assign_reverse_map(string solver) {
		var path = "unit/regression/assign_reverse_map.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bind-defines-var.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bind_defines_var(string solver) {
		var path = "unit/regression/bind-defines-var.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/binop_mult_gclock.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_binop_mult_gclock(string solver) {
		var path = "unit/regression/binop_mult_gclock.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bounds_for_linear_01_max_0.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_bounds_for_linear_01_max_0(string solver) {
		var path = "unit/regression/bounds_for_linear_01_max_0.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bounds_for_linear_01_max_1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_bounds_for_linear_01_max_1(string solver) {
		var path = "unit/regression/bounds_for_linear_01_max_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bounds_for_linear_01_min_0.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_bounds_for_linear_01_min_0(string solver) {
		var path = "unit/regression/bounds_for_linear_01_min_0.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bounds_for_linear_01_min_1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_bounds_for_linear_01_min_1(string solver) {
		var path = "unit/regression/bounds_for_linear_01_min_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug110.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug110(string solver) {
		var path = "unit/regression/bug110.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug131.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug131(string solver) {
		var path = "unit/regression/bug131.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug212.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug212(string solver) {
		var path = "unit/regression/bug212.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug222.mzn")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_regression_bug222(string solver) {
		var path = "unit/regression/bug222.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug244.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug244(string solver) {
		var path = "unit/regression/bug244.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug269.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug269(string solver) {
		var path = "unit/regression/bug269.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug284.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug284(string solver) {
		var path = "unit/regression/bug284.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug318_orig.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug318_orig(string solver) {
		var path = "unit/regression/bug318_orig.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug335.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug335(string solver) {
		var path = "unit/regression/bug335.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug380.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug380(string solver) {
		var path = "unit/regression/bug380.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug532.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug532(string solver) {
		var path = "unit/regression/bug532.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug534.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug534(string solver) {
		var path = "unit/regression/bug534.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug536.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug536(string solver) {
		var path = "unit/regression/bug536.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug552.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug552(string solver) {
		var path = "unit/regression/bug552.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug565.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug565(string solver) {
		var path = "unit/regression/bug565.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug570.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug570(string solver) {
		var path = "unit/regression/bug570.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug635.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug635(string solver) {
		var path = "unit/regression/bug635.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug67.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug67(string solver) {
		var path = "unit/regression/bug67.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug82.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_bug82(string solver) {
		var path = "unit/regression/bug82.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug_629.mzn")]
	[InlineData("cbc")]
	public async void test_unit_regression_bug_629(string solver) {
		var path = "unit/regression/bug_629.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug_629.mzn")]
	[InlineData("cbc")]
	public async void test_unit_regression_bug_629_case_2(string solver) {
		var path = "unit/regression/bug_629.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug_empty_enum_extension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug_empty_enum_extension(string solver) {
		var path = "unit/regression/bug_empty_enum_extension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/bug_opt_polymorphic.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_bug_opt_polymorphic(string solver) {
		var path = "unit/regression/bug_opt_polymorphic.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/checker_mzn_check_var.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_checker_mzn_check_var(string solver) {
		var path = "unit/regression/checker_mzn_check_var.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/checker_opt.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_checker_opt(string solver) {
		var path = "unit/regression/checker_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/checker_params.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_checker_params(string solver) {
		var path = "unit/regression/checker_params.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/checker_same_var.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_checker_same_var(string solver) {
		var path = "unit/regression/checker_same_var.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/checker_var_bug.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_checker_var_bug(string solver) {
		var path = "unit/regression/checker_var_bug.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/coercion_par.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_coercion_par(string solver) {
		var path = "unit/regression/coercion_par.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/constructor_of_set.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_constructor_of_set(string solver) {
		var path = "unit/regression/constructor_of_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/cse_array_lit.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_cse_array_lit(string solver) {
		var path = "unit/regression/cse_array_lit.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/cyclic_include.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_cyclic_include(string solver) {
		var path = "unit/regression/cyclic_include.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/decision_tree_binary.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_decision_tree_binary(string solver) {
		var path = "unit/regression/decision_tree_binary.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/enigma_1568.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_enigma_1568(string solver) {
		var path = "unit/regression/enigma_1568.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/flatten_comp_in.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_flatten_comp_in(string solver) {
		var path = "unit/regression/flatten_comp_in.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/flat_set_lit.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_flat_set_lit(string solver) {
		var path = "unit/regression/flat_set_lit.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/float_ceil_floor.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_float_ceil_floor(string solver) {
		var path = "unit/regression/float_ceil_floor.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/float_opt_crash.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_float_opt_crash(string solver) {
		var path = "unit/regression/float_opt_crash.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github537.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github537(string solver) {
		var path = "unit/regression/github537.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_638_reduced.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_638_reduced(string solver) {
		var path = "unit/regression/github_638_reduced.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_639_part1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_639_part1(string solver) {
		var path = "unit/regression/github_639_part1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_639_part2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_639_part2(string solver) {
		var path = "unit/regression/github_639_part2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_644_a.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_644_a(string solver) {
		var path = "unit/regression/github_644_a.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_644_b.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_644_b(string solver) {
		var path = "unit/regression/github_644_b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_644_c.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_644_c(string solver) {
		var path = "unit/regression/github_644_c.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_644_d.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_644_d(string solver) {
		var path = "unit/regression/github_644_d.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_644_e.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_644_e(string solver) {
		var path = "unit/regression/github_644_e.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_646.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_646(string solver) {
		var path = "unit/regression/github_646.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_648_par_array_decl.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_648_par_array_decl(string solver) {
		var path = "unit/regression/github_648_par_array_decl.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_648_par_decl.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_648_par_decl(string solver) {
		var path = "unit/regression/github_648_par_decl.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_656.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_656(string solver) {
		var path = "unit/regression/github_656.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_661_part1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_661_part1(string solver) {
		var path = "unit/regression/github_661_part1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_661_part2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_661_part2(string solver) {
		var path = "unit/regression/github_661_part2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_664.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_664(string solver) {
		var path = "unit/regression/github_664.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_666.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_666(string solver) {
		var path = "unit/regression/github_666.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_667.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("highs")]
	public async void test_unit_regression_github_667(string solver) {
		var path = "unit/regression/github_667.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_668.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_668(string solver) {
		var path = "unit/regression/github_668.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_669.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_669(string solver) {
		var path = "unit/regression/github_669.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_670.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_670(string solver) {
		var path = "unit/regression/github_670.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_671.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_671(string solver) {
		var path = "unit/regression/github_671.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_673.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_673(string solver) {
		var path = "unit/regression/github_673.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_674.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("highs")]
	public async void test_unit_regression_github_674(string solver) {
		var path = "unit/regression/github_674.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_675a.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_675a(string solver) {
		var path = "unit/regression/github_675a.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_675b.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_675b(string solver) {
		var path = "unit/regression/github_675b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_680.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_680(string solver) {
		var path = "unit/regression/github_680.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_681.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_681(string solver) {
		var path = "unit/regression/github_681.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_683.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_683(string solver) {
		var path = "unit/regression/github_683.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_685.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_685(string solver) {
		var path = "unit/regression/github_685.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_687.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_687(string solver) {
		var path = "unit/regression/github_687.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_691.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_691(string solver) {
		var path = "unit/regression/github_691.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_693_part1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_693_part1(string solver) {
		var path = "unit/regression/github_693_part1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_693_part2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_693_part2(string solver) {
		var path = "unit/regression/github_693_part2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_695.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_695(string solver) {
		var path = "unit/regression/github_695.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_700.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_700(string solver) {
		var path = "unit/regression/github_700.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_700_bad_sol.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_github_700_bad_sol(string solver) {
		var path = "unit/regression/github_700_bad_sol.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_716.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_716(string solver) {
		var path = "unit/regression/github_716.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_725.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_725(string solver) {
		var path = "unit/regression/github_725.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_726.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_726(string solver) {
		var path = "unit/regression/github_726.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_728.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_728(string solver) {
		var path = "unit/regression/github_728.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_730.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_730(string solver) {
		var path = "unit/regression/github_730.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_732.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_732(string solver) {
		var path = "unit/regression/github_732.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_747.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_747(string solver) {
		var path = "unit/regression/github_747.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_748.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_748(string solver) {
		var path = "unit/regression/github_748.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_748.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_748_case_2(string solver) {
		var path = "unit/regression/github_748.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_749.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_749(string solver) {
		var path = "unit/regression/github_749.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_752.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_752(string solver) {
		var path = "unit/regression/github_752.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_754.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_754(string solver) {
		var path = "unit/regression/github_754.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_758.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_758(string solver) {
		var path = "unit/regression/github_758.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_758.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_758_case_2(string solver) {
		var path = "unit/regression/github_758.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_760.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_760(string solver) {
		var path = "unit/regression/github_760.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_761.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_761(string solver) {
		var path = "unit/regression/github_761.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_762.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_regression_github_762(string solver) {
		var path = "unit/regression/github_762.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_765.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_765(string solver) {
		var path = "unit/regression/github_765.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_766.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_766(string solver) {
		var path = "unit/regression/github_766.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_768a.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_768a(string solver) {
		var path = "unit/regression/github_768a.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_768b.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_768b(string solver) {
		var path = "unit/regression/github_768b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_771.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_771(string solver) {
		var path = "unit/regression/github_771.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_773.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_773(string solver) {
		var path = "unit/regression/github_773.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_776.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_776(string solver) {
		var path = "unit/regression/github_776.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_778.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_778(string solver) {
		var path = "unit/regression/github_778.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_779.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_779(string solver) {
		var path = "unit/regression/github_779.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_783.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_783(string solver) {
		var path = "unit/regression/github_783.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/github_785.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_github_785(string solver) {
		var path = "unit/regression/github_785.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/if_then_else_absent.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_if_then_else_absent(string solver) {
		var path = "unit/regression/if_then_else_absent.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/in_array_eval_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_in_array_eval_error(string solver) {
		var path = "unit/regression/in_array_eval_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/let_domain_from_generator.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_let_domain_from_generator(string solver) {
		var path = "unit/regression/let_domain_from_generator.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/makepar_output.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_makepar_output(string solver) {
		var path = "unit/regression/makepar_output.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/multi_goal_hierarchy_error.mzn")]
	[InlineData("cbc")]
	public async void test_unit_regression_multi_goal_hierarchy_error(string solver) {
		var path = "unit/regression/multi_goal_hierarchy_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/non-set-array-ti-location.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_non_set_array_ti_location(string solver) {
		var path = "unit/regression/non-set-array-ti-location.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/non_pos_pow.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_non_pos_pow(string solver) {
		var path = "unit/regression/non_pos_pow.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/nosets_set_search.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_nosets_set_search(string solver) {
		var path = "unit/regression/nosets_set_search.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/opt_minmax.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_opt_minmax(string solver) {
		var path = "unit/regression/opt_minmax.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/opt_noncontiguous_domain.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_opt_noncontiguous_domain(string solver) {
		var path = "unit/regression/opt_noncontiguous_domain.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/output_2d_array_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_output_2d_array_enum(string solver) {
		var path = "unit/regression/output_2d_array_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/output_fn_toplevel_var.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_output_fn_toplevel_var(string solver) {
		var path = "unit/regression/output_fn_toplevel_var.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/output_only_fn.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_output_only_fn(string solver) {
		var path = "unit/regression/output_only_fn.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/output_only_no_rhs.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_output_only_no_rhs(string solver) {
		var path = "unit/regression/output_only_no_rhs.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/parser_location.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_parser_location(string solver) {
		var path = "unit/regression/parser_location.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/parse_assignments.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_parse_assignments(string solver) {
		var path = "unit/regression/parse_assignments.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/par_opt_dom.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_par_opt_dom(string solver) {
		var path = "unit/regression/par_opt_dom.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/polymorphic_var_and_par.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_polymorphic_var_and_par(string solver) {
		var path = "unit/regression/polymorphic_var_and_par.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/pow_undefined.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_pow_undefined(string solver) {
		var path = "unit/regression/pow_undefined.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/pred_param_r7550.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_pred_param_r7550(string solver) {
		var path = "unit/regression/pred_param_r7550.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/slice_enum_indexset.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_slice_enum_indexset(string solver) {
		var path = "unit/regression/slice_enum_indexset.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/subsets_100.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_subsets_100(string solver) {
		var path = "unit/regression/subsets_100.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_annotation_on_exists.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_annotation_on_exists(string solver) {
		var path = "unit/regression/test_annotation_on_exists.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug359.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_test_bug359(string solver) {
		var path = "unit/regression/test_bug359.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug54.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug54(string solver) {
		var path = "unit/regression/test_bug54.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug54.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_regression_test_bug54_case_2(string solver) {
		var path = "unit/regression/test_bug54.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug72.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug72(string solver) {
		var path = "unit/regression/test_bug72.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug72.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_regression_test_bug72_case_2(string solver) {
		var path = "unit/regression/test_bug72.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_476.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_476(string solver) {
		var path = "unit/regression/test_bug_476.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_483.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_483(string solver) {
		var path = "unit/regression/test_bug_483.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_493.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_493(string solver) {
		var path = "unit/regression/test_bug_493.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_494.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_494(string solver) {
		var path = "unit/regression/test_bug_494.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_520.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_520(string solver) {
		var path = "unit/regression/test_bug_520.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_521.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_521(string solver) {
		var path = "unit/regression/test_bug_521.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_527.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_527(string solver) {
		var path = "unit/regression/test_bug_527.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_529.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_529(string solver) {
		var path = "unit/regression/test_bug_529.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_588.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_588(string solver) {
		var path = "unit/regression/test_bug_588.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_637.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_637(string solver) {
		var path = "unit/regression/test_bug_637.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_array_sum_bounds.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_array_sum_bounds(string solver) {
		var path = "unit/regression/test_bug_array_sum_bounds.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_ite_array_eq.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_ite_array_eq(string solver) {
		var path = "unit/regression/test_bug_ite_array_eq.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_bug_pred_arg.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_test_bug_pred_arg(string solver) {
		var path = "unit/regression/test_bug_pred_arg.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_parout.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_test_parout(string solver) {
		var path = "unit/regression/test_parout.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/test_parout.mzn")]
	[InlineData("cbc")]
	public async void test_unit_regression_test_parout_case_2(string solver) {
		var path = "unit/regression/test_parout.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/ti_error_location.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_ti_error_location(string solver) {
		var path = "unit/regression/ti_error_location.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/ts_bug.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_ts_bug(string solver) {
		var path = "unit/regression/ts_bug.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/var_bool_comp.mzn")]
	[InlineData("gecode")]
	public async void test_unit_regression_var_bool_comp(string solver) {
		var path = "unit/regression/var_bool_comp.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/var_opt_unconstrained.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_regression_var_opt_unconstrained(string solver) {
		var path = "unit/regression/var_opt_unconstrained.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/regression/var_self_assign_bug.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	public async void test_unit_regression_var_self_assign_bug(string solver) {
		var path = "unit/regression/var_self_assign_bug.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_choice_1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_choice_1(string solver) {
		var path = "unit/search/int_choice_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_choice_2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_choice_2(string solver) {
		var path = "unit/search/int_choice_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_choice_6.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_choice_6(string solver) {
		var path = "unit/search/int_choice_6.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_var_select_1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_var_select_1(string solver) {
		var path = "unit/search/int_var_select_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_var_select_2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_var_select_2(string solver) {
		var path = "unit/search/int_var_select_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_var_select_3.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_var_select_3(string solver) {
		var path = "unit/search/int_var_select_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_var_select_4.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_var_select_4(string solver) {
		var path = "unit/search/int_var_select_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/int_var_select_6.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_int_var_select_6(string solver) {
		var path = "unit/search/int_var_select_6.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-ff1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_test_ff1(string solver) {
		var path = "unit/search/test-ff1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-ff2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_test_ff2(string solver) {
		var path = "unit/search/test-ff2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-ff3.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_test_ff3(string solver) {
		var path = "unit/search/test-ff3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-large1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_search_test_large1(string solver) {
		var path = "unit/search/test-large1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-large1.mzn")]
	[InlineData("chuffed")]
	public async void test_unit_search_test_large1_case_2(string solver) {
		var path = "unit/search/test-large1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-med1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_test_med1(string solver) {
		var path = "unit/search/test-med1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/search/test-small1.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_search_test_small1(string solver) {
		var path = "unit/search/test-small1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/alias.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_alias(string solver) {
		var path = "unit/types/alias.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/alias_call.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_alias_call(string solver) {
		var path = "unit/types/alias_call.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/alias_extern_dom.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_alias_extern_dom(string solver) {
		var path = "unit/types/alias_extern_dom.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/alias_set_of_array.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_alias_set_of_array(string solver) {
		var path = "unit/types/alias_set_of_array.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/array_var_opt_set_comprehension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_array_var_opt_set_comprehension(string solver) {
		var path = "unit/types/array_var_opt_set_comprehension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/comprehension_of_absent_any_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_comprehension_of_absent_any_1(string solver) {
		var path = "unit/types/comprehension_of_absent_any_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/comprehension_type.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_comprehension_type(string solver) {
		var path = "unit/types/comprehension_type.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/cv_comprehension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_cv_comprehension(string solver) {
		var path = "unit/types/cv_comprehension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/enum_decl.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_enum_decl(string solver) {
		var path = "unit/types/enum_decl.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/enum_refl.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_enum_refl(string solver) {
		var path = "unit/types/enum_refl.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/github_647.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_github_647(string solver) {
		var path = "unit/types/github_647.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/nested_type_inst_id.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_nested_type_inst_id(string solver) {
		var path = "unit/types/nested_type_inst_id.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/nonbool_constraint.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_nonbool_constraint(string solver) {
		var path = "unit/types/nonbool_constraint.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/nonbool_constraint_let.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_nonbool_constraint_let(string solver) {
		var path = "unit/types/nonbool_constraint_let.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/non_contig_enum.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_non_contig_enum(string solver) {
		var path = "unit/types/non_contig_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/overload_inst_tuple_return.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_overload_inst_tuple_return(string solver) {
		var path = "unit/types/overload_inst_tuple_return.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/par_struct_tiid.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_par_struct_tiid(string solver) {
		var path = "unit/types/par_struct_tiid.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/polymorphic_overloading.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_polymorphic_overloading(string solver) {
		var path = "unit/types/polymorphic_overloading.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_access_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_access_error(string solver) {
		var path = "unit/types/record_access_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_access_success.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_access_success(string solver) {
		var path = "unit/types/record_access_success.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_array_access_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_array_access_error(string solver) {
		var path = "unit/types/record_array_access_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_binop_par.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_binop_par(string solver) {
		var path = "unit/types/record_binop_par.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_binop_var.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_binop_var(string solver) {
		var path = "unit/types/record_binop_var.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_comprehensions.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_comprehensions(string solver) {
		var path = "unit/types/record_comprehensions.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_decl_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_decl_error(string solver) {
		var path = "unit/types/record_decl_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_ite_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_ite_error(string solver) {
		var path = "unit/types/record_ite_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_lit_dup.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_lit_dup(string solver) {
		var path = "unit/types/record_lit_dup.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_nested.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_nested(string solver) {
		var path = "unit/types/record_nested.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_output.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_output(string solver) {
		var path = "unit/types/record_output.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_subtyping.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_subtyping(string solver) {
		var path = "unit/types/record_subtyping.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_var_element.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_var_element(string solver) {
		var path = "unit/types/record_var_element.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/record_var_ite.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_record_var_ite(string solver) {
		var path = "unit/types/record_var_ite.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/specialise_large_struct.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_specialise_large_struct(string solver) {
		var path = "unit/types/specialise_large_struct.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_array_coercion.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_array_coercion(string solver) {
		var path = "unit/types/struct_array_coercion.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_bind_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_bind_1(string solver) {
		var path = "unit/types/struct_bind_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_bind_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_bind_2(string solver) {
		var path = "unit/types/struct_bind_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_domain_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_domain_1(string solver) {
		var path = "unit/types/struct_domain_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_domain_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_domain_2(string solver) {
		var path = "unit/types/struct_domain_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_domain_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_domain_3(string solver) {
		var path = "unit/types/struct_domain_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_domain_4.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_domain_4(string solver) {
		var path = "unit/types/struct_domain_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_domain_5.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_domain_5(string solver) {
		var path = "unit/types/struct_domain_5.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_domain_6.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_domain_6(string solver) {
		var path = "unit/types/struct_domain_6.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_index_sets_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_index_sets_1(string solver) {
		var path = "unit/types/struct_index_sets_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_index_sets_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_index_sets_2(string solver) {
		var path = "unit/types/struct_index_sets_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_par_function_version.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_par_function_version(string solver) {
		var path = "unit/types/struct_par_function_version.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_return_ti_1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_return_ti_1(string solver) {
		var path = "unit/types/struct_return_ti_1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_return_ti_2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_return_ti_2(string solver) {
		var path = "unit/types/struct_return_ti_2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_return_ti_3.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_return_ti_3(string solver) {
		var path = "unit/types/struct_return_ti_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_return_ti_4.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_return_ti_4(string solver) {
		var path = "unit/types/struct_return_ti_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_specialise.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_specialise(string solver) {
		var path = "unit/types/struct_specialise.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/struct_specialise_return.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_struct_specialise_return(string solver) {
		var path = "unit/types/struct_specialise_return.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/test_any_enum_typeinstid.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_test_any_enum_typeinstid(string solver) {
		var path = "unit/types/test_any_enum_typeinstid.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_access_error1.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_access_error1(string solver) {
		var path = "unit/types/tuple_access_error1.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_access_error2.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_access_error2(string solver) {
		var path = "unit/types/tuple_access_error2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_access_success.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_access_success(string solver) {
		var path = "unit/types/tuple_access_success.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_array_access_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_array_access_error(string solver) {
		var path = "unit/types/tuple_array_access_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_binop_par.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_binop_par(string solver) {
		var path = "unit/types/tuple_binop_par.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_binop_var.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_binop_var(string solver) {
		var path = "unit/types/tuple_binop_var.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_comprehensions.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_comprehensions(string solver) {
		var path = "unit/types/tuple_comprehensions.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_int_set_of_int_specialisation.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_int_set_of_int_specialisation(string solver) {
		var path = "unit/types/tuple_int_set_of_int_specialisation.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_ite_error.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_ite_error(string solver) {
		var path = "unit/types/tuple_ite_error.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_lit.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_lit(string solver) {
		var path = "unit/types/tuple_lit.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_mkpar.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_mkpar(string solver) {
		var path = "unit/types/tuple_mkpar.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_output.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_output(string solver) {
		var path = "unit/types/tuple_output.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_subtyping.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_subtyping(string solver) {
		var path = "unit/types/tuple_subtyping.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_var_element.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_var_element(string solver) {
		var path = "unit/types/tuple_var_element.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/tuple_var_ite.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_tuple_var_ite(string solver) {
		var path = "unit/types/tuple_var_ite.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_ann_a.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_ann_a(string solver) {
		var path = "unit/types/var_ann_a.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_ann_b.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_ann_b(string solver) {
		var path = "unit/types/var_ann_b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_ann_comprehension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_ann_comprehension(string solver) {
		var path = "unit/types/var_ann_comprehension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_opt_set_if_then_else.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_opt_set_if_then_else(string solver) {
		var path = "unit/types/var_opt_set_if_then_else.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_set_bool.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_set_bool(string solver) {
		var path = "unit/types/var_set_bool.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_set_float.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_set_float(string solver) {
		var path = "unit/types/var_set_float.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_set_float_comprehension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_set_float_comprehension(string solver) {
		var path = "unit/types/var_set_float_comprehension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_string_a.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_string_a(string solver) {
		var path = "unit/types/var_string_a.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_string_b.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_string_b(string solver) {
		var path = "unit/types/var_string_b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/types/var_string_comprehension.mzn")]
	[InlineData("gecode")]
	public async void test_unit_types_var_string_comprehension(string solver) {
		var path = "unit/types/var_string_comprehension.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/alldifferent_except_0/test_alldiff_except0b.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_alldifferent_except_0_test_alldiff_except0b(string solver) {
		var path = "unit/globals/alldifferent_except_0/test_alldiff_except0b.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_max/globals_arg_max.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_max_globals_arg_max(string solver) {
		var path = "unit/globals/arg_max/globals_arg_max.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_max/globals_arg_max_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_max_globals_arg_max_opt(string solver) {
		var path = "unit/globals/arg_max/globals_arg_max_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_max/globals_arg_max_opt_weak.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_max_globals_arg_max_opt_weak(string solver) {
		var path = "unit/globals/arg_max/globals_arg_max_opt_weak.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_min/globals_arg_max_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_min_globals_arg_max_opt(string solver) {
		var path = "unit/globals/arg_min/globals_arg_max_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_min/globals_arg_min.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_min_globals_arg_min(string solver) {
		var path = "unit/globals/arg_min/globals_arg_min.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_min/globals_arg_min_opt_weak.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_min_globals_arg_min_opt_weak(string solver) {
		var path = "unit/globals/arg_min/globals_arg_min_opt_weak.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/arg_val/arg_val_enum.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_arg_val_arg_val_enum(string solver) {
		var path = "unit/globals/arg_val/arg_val_enum.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/bin_packing/globals_bin_packing.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_bin_packing_globals_bin_packing(string solver) {
		var path = "unit/globals/bin_packing/globals_bin_packing.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/bin_packing_capa/globals_bin_packing_capa.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_bin_packing_capa_globals_bin_packing_capa(string solver) {
		var path = "unit/globals/bin_packing_capa/globals_bin_packing_capa.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/circuit/test_circuit.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_circuit_test_circuit(string solver) {
		var path = "unit/globals/circuit/test_circuit.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/cumulative/github_589.mzn")]
	[InlineData("gecode")]
	public async void test_unit_globals_cumulative_github_589(string solver) {
		var path = "unit/globals/cumulative/github_589.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/cumulatives/globals_cumulatives.mzn")]
	[InlineData("gecode")]
	public async void test_unit_globals_cumulatives_globals_cumulatives(string solver) {
		var path = "unit/globals/cumulatives/globals_cumulatives.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/cumulatives/globals_cumulatives.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_cumulatives_globals_cumulatives_case_2(string solver) {
		var path = "unit/globals/cumulatives/globals_cumulatives.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality/globals_global_cardinality_low_up_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_globals_global_cardinality_low_up_set(string solver) {
		var path = "unit/globals/global_cardinality/globals_global_cardinality_low_up_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality/globals_global_cardinality_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_globals_global_cardinality_opt(string solver) {
		var path = "unit/globals/global_cardinality/globals_global_cardinality_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality/globals_global_cardinality_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_globals_global_cardinality_set(string solver) {
		var path = "unit/globals/global_cardinality/globals_global_cardinality_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality_closed/globals_global_cardinality_closed_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed_opt(string solver) {
		var path = "unit/globals/global_cardinality_closed/globals_global_cardinality_closed_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality_closed/globals_global_cardinality_closed_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed_set(string solver) {
		var path = "unit/globals/global_cardinality_closed/globals_global_cardinality_closed_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed_opt(string solver) {
		var path = "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed_set(string solver) {
		var path = "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/int_set_channel/test_int_set_channel2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	[InlineData("cbc")]
	public async void test_unit_globals_int_set_channel_test_int_set_channel2(string solver) {
		var path = "unit/globals/int_set_channel/test_int_set_channel2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/inverse/inverse_opt.mzn")]
	[InlineData("gecode")]
	public async void test_unit_globals_inverse_inverse_opt(string solver) {
		var path = "unit/globals/inverse/inverse_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex2/globals_lex2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_lex2_globals_lex2(string solver) {
		var path = "unit/globals/lex2/globals_lex2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex2/globals_lex2.mzn")]
	[InlineData("cbc")]
	public async void test_unit_globals_lex2_globals_lex2_case_2(string solver) {
		var path = "unit/globals/lex2/globals_lex2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex_chain/globals_lex_chain__orbitope.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("scip")]
	public async void test_unit_globals_lex_chain_globals_lex_chain__orbitope(string solver) {
		var path = "unit/globals/lex_chain/globals_lex_chain__orbitope.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex_greater/globals_lex_greater.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_lex_greater_globals_lex_greater(string solver) {
		var path = "unit/globals/lex_greater/globals_lex_greater.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex_greatereq/globals_lex_greatereq.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_lex_greatereq_globals_lex_greatereq(string solver) {
		var path = "unit/globals/lex_greatereq/globals_lex_greatereq.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex_less/test_bool_lex_less.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_lex_less_test_bool_lex_less(string solver) {
		var path = "unit/globals/lex_less/test_bool_lex_less.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/lex_lesseq/test_bool_lex_lesseq.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_lex_lesseq_test_bool_lex_lesseq(string solver) {
		var path = "unit/globals/lex_lesseq/test_bool_lex_lesseq.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/nvalue/globals_nvalue.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_nvalue_globals_nvalue(string solver) {
		var path = "unit/globals/nvalue/globals_nvalue.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/nvalue/globals_nvalue.mzn")]
	[InlineData("cbc")]
	[InlineData("highs")]
	public async void test_unit_globals_nvalue_globals_nvalue_case_2(string solver) {
		var path = "unit/globals/nvalue/globals_nvalue.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/nvalue/nvalue_total.mzn")]
	[InlineData("gecode")]
	public async void test_unit_globals_nvalue_nvalue_total(string solver) {
		var path = "unit/globals/nvalue/nvalue_total.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/range/globals_range.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_range_globals_range(string solver) {
		var path = "unit/globals/range/globals_range.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/regular/globals_regular.mzn")]
	[InlineData("gecode")]
	public async void test_unit_globals_regular_globals_regular(string solver) {
		var path = "unit/globals/regular/globals_regular.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/regular/globals_regular_regex_3.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_regular_globals_regular_regex_3(string solver) {
		var path = "unit/globals/regular/globals_regular_regex_3.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/regular/globals_regular_regex_5.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_regular_globals_regular_regex_5(string solver) {
		var path = "unit/globals/regular/globals_regular_regex_5.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/roots/test_roots2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_roots_test_roots2(string solver) {
		var path = "unit/globals/roots/test_roots2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/strict_lex2/globals_strict_lex2.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_strict_lex2_globals_strict_lex2(string solver) {
		var path = "unit/globals/strict_lex2/globals_strict_lex2.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/subcircuit/test_subcircuit.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_subcircuit_test_subcircuit(string solver) {
		var path = "unit/globals/subcircuit/test_subcircuit.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/table/globals_table_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_table_globals_table_opt(string solver) {
		var path = "unit/globals/table/globals_table_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/value_precede/globals_value_precede_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_value_precede_globals_value_precede_int(string solver) {
		var path = "unit/globals/value_precede/globals_value_precede_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/value_precede/globals_value_precede_int_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_value_precede_globals_value_precede_int_opt(string solver) {
		var path = "unit/globals/value_precede/globals_value_precede_int_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/value_precede/globals_value_precede_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_value_precede_globals_value_precede_set(string solver) {
		var path = "unit/globals/value_precede/globals_value_precede_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/value_precede_chain/globals_value_precede_chain_int.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_value_precede_chain_globals_value_precede_chain_int(string solver) {
		var path = "unit/globals/value_precede_chain/globals_value_precede_chain_int.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/value_precede_chain/globals_value_precede_chain_int_opt.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_value_precede_chain_globals_value_precede_chain_int_opt(string solver) {
		var path = "unit/globals/value_precede_chain/globals_value_precede_chain_int_opt.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/value_precede_chain/globals_value_precede_chain_set.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_value_precede_chain_globals_value_precede_chain_set(string solver) {
		var path = "unit/globals/value_precede_chain/globals_value_precede_chain_set.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="unit/globals/var_sqr_sym/globals_var_sqr_sym.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_unit_globals_var_sqr_sym_globals_var_sqr_sym(string solver) {
		var path = "unit/globals/var_sqr_sym/globals_var_sqr_sym.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/battleships_4.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("chuffed")]
	public async void test_examples_battleships_4(string solver) {
		var path = "examples/battleships_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/battleships_7.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("chuffed")]
	public async void test_examples_battleships_7(string solver) {
		var path = "examples/battleships_7.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/factory_planning_instance.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("chuffed")]
	public async void test_examples_factory_planning_instance(string solver) {
		var path = "examples/factory_planning_instance.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/knights.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_knights(string solver) {
		var path = "examples/knights.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/magicsq_4.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_magicsq_4(string solver) {
		var path = "examples/magicsq_4.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/magicsq_5.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_magicsq_5(string solver) {
		var path = "examples/magicsq_5.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/oss.mzn")]
	[InlineData("gecode")]
	[InlineData("cbc")]
	[InlineData("chuffed")]
	public async void test_examples_oss(string solver) {
		var path = "examples/oss.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/packing.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_packing(string solver) {
		var path = "examples/packing.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/radiation.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_radiation(string solver) {
		var path = "examples/radiation.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/radiation.mzn")]
	[InlineData("cbc")]
	public async void test_examples_radiation_case_2(string solver) {
		var path = "examples/radiation.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/template_design.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_template_design(string solver) {
		var path = "examples/template_design.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/wolf_goat_cabbage.mzn")]
	[InlineData("cbc")]
	public async void test_examples_wolf_goat_cabbage(string solver) {
		var path = "examples/wolf_goat_cabbage.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	[Theory(DisplayName="examples/wolf_goat_cabbage.mzn")]
	[InlineData("gecode")]
	[InlineData("chuffed")]
	public async void test_examples_wolf_goat_cabbage_case_2(string solver) {
		var path = "examples/wolf_goat_cabbage.mzn";
		var solve = _client.SolveModelFile(path, solver);
		var solution = await solve.Solution();
	}

	}