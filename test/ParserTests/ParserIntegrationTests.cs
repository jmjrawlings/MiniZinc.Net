public sealed class ParserIntegrationTests
{
    private void Test(string path)
    {
        var results = Parser.ParseFile(path, out var tree);
        results.ErrorTrace.Should().BeNull();
    }

    [Fact(DisplayName = "unit/test-globals-float.mzn")]
    public void test_unit_test_globals_float()
    {
        var path = "unit/test-globals-float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/aggregation.mzn")]
    public void test_unit_compilation_aggregation()
    {
        var path = "unit/compilation/aggregation.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/annotate_bool_literal.mzn")]
    public void test_unit_compilation_annotate_bool_literal()
    {
        var path = "unit/compilation/annotate_bool_literal.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/annotate_from_array.mzn")]
    public void test_unit_compilation_annotate_from_array()
    {
        var path = "unit/compilation/annotate_from_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/assert_dbg_ignore.mzn")]
    public void test_unit_compilation_assert_dbg_ignore()
    {
        var path = "unit/compilation/assert_dbg_ignore.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/call_root_ctx.mzn")]
    public void test_unit_compilation_call_root_ctx()
    {
        var path = "unit/compilation/call_root_ctx.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/chain_compr_mult_clause.mzn")]
    public void test_unit_compilation_chain_compr_mult_clause()
    {
        var path = "unit/compilation/chain_compr_mult_clause.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/clause_disable_hr.mzn")]
    public void test_unit_compilation_clause_disable_hr()
    {
        var path = "unit/compilation/clause_disable_hr.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/commutative_cse.mzn")]
    public void test_unit_compilation_commutative_cse()
    {
        var path = "unit/compilation/commutative_cse.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/count_rewrite.mzn")]
    public void test_unit_compilation_count_rewrite()
    {
        var path = "unit/compilation/count_rewrite.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/cv_domain.mzn")]
    public void test_unit_compilation_cv_domain()
    {
        var path = "unit/compilation/cv_domain.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/debug_mode_false.mzn")]
    public void test_unit_compilation_debug_mode_false()
    {
        var path = "unit/compilation/debug_mode_false.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/debug_mode_true.mzn")]
    public void test_unit_compilation_debug_mode_true()
    {
        var path = "unit/compilation/debug_mode_true.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/defines_var_cycle_breaking.mzn")]
    public void test_unit_compilation_defines_var_cycle_breaking()
    {
        var path = "unit/compilation/defines_var_cycle_breaking.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/float_inf_range_dom.mzn")]
    public void test_unit_compilation_float_inf_range_dom()
    {
        var path = "unit/compilation/float_inf_range_dom.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/has_ann.mzn")]
    public void test_unit_compilation_has_ann()
    {
        var path = "unit/compilation/has_ann.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/if_then_no_else.mzn")]
    public void test_unit_compilation_if_then_no_else()
    {
        var path = "unit/compilation/if_then_no_else.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/implied_exists_chain.mzn")]
    public void test_unit_compilation_implied_exists_chain()
    {
        var path = "unit/compilation/implied_exists_chain.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/implied_hr.mzn")]
    public void test_unit_compilation_implied_hr()
    {
        var path = "unit/compilation/implied_hr.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/int_inf_dom.mzn")]
    public void test_unit_compilation_int_inf_dom()
    {
        var path = "unit/compilation/int_inf_dom.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/multiple_neg.mzn")]
    public void test_unit_compilation_multiple_neg()
    {
        var path = "unit/compilation/multiple_neg.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/optimization.mzn")]
    public void test_unit_compilation_optimization()
    {
        var path = "unit/compilation/optimization.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/par_arg_out_of_bounds.mzn")]
    public void test_unit_compilation_par_arg_out_of_bounds()
    {
        var path = "unit/compilation/par_arg_out_of_bounds.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/poly_overload.mzn")]
    public void test_unit_compilation_poly_overload()
    {
        var path = "unit/compilation/poly_overload.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/quoted_id_flatzinc.mzn")]
    public void test_unit_compilation_quoted_id_flatzinc()
    {
        var path = "unit/compilation/quoted_id_flatzinc.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/compilation/set2iter.mzn")]
    public void test_unit_compilation_set2iter()
    {
        var path = "unit/compilation/set2iter.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div1.mzn")]
    public void test_unit_division_test_div1()
    {
        var path = "unit/division/test_div1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div10.mzn")]
    public void test_unit_division_test_div10()
    {
        var path = "unit/division/test_div10.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div11.mzn")]
    public void test_unit_division_test_div11()
    {
        var path = "unit/division/test_div11.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div12.mzn")]
    public void test_unit_division_test_div12()
    {
        var path = "unit/division/test_div12.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div2.mzn")]
    public void test_unit_division_test_div2()
    {
        var path = "unit/division/test_div2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div3.mzn")]
    public void test_unit_division_test_div3()
    {
        var path = "unit/division/test_div3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div4.mzn")]
    public void test_unit_division_test_div4()
    {
        var path = "unit/division/test_div4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div5.mzn")]
    public void test_unit_division_test_div5()
    {
        var path = "unit/division/test_div5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div6.mzn")]
    public void test_unit_division_test_div6()
    {
        var path = "unit/division/test_div6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div7.mzn")]
    public void test_unit_division_test_div7()
    {
        var path = "unit/division/test_div7.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div8.mzn")]
    public void test_unit_division_test_div8()
    {
        var path = "unit/division/test_div8.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div9.mzn")]
    public void test_unit_division_test_div9()
    {
        var path = "unit/division/test_div9.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_div_mod_bounds.mzn")]
    public void test_unit_division_test_div_mod_bounds()
    {
        var path = "unit/division/test_div_mod_bounds.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_fldiv_01.mzn")]
    public void test_unit_division_test_fldiv_01()
    {
        var path = "unit/division/test_fldiv_01.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/division/test_fldiv_02.mzn")]
    public void test_unit_division_test_fldiv_02()
    {
        var path = "unit/division/test_fldiv_02.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/fdlp/test_lp_solve_satisfy.mzn")]
    public void test_unit_fdlp_test_lp_solve_satisfy()
    {
        var path = "unit/fdlp/test_lp_solve_satisfy.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/annotated_expression_1.mzn")]
    public void test_unit_general_annotated_expression_1()
    {
        var path = "unit/general/annotated_expression_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/anon_var_flatten.mzn")]
    public void test_unit_general_anon_var_flatten()
    {
        var path = "unit/general/anon_var_flatten.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/array_intersect_context.mzn")]
    public void test_unit_general_array_intersect_context()
    {
        var path = "unit/general/array_intersect_context.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/array_param_non_array_return.mzn")]
    public void test_unit_general_array_param_non_array_return()
    {
        var path = "unit/general/array_param_non_array_return.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/array_string_gen.mzn")]
    public void test_unit_general_array_string_gen()
    {
        var path = "unit/general/array_string_gen.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/array_union_intersect_enum.mzn")]
    public void test_unit_general_array_union_intersect_enum()
    {
        var path = "unit/general/array_union_intersect_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/assert_good.mzn")]
    public void test_unit_general_assert_good()
    {
        var path = "unit/general/assert_good.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/auto_par_function.mzn")]
    public void test_unit_general_auto_par_function()
    {
        var path = "unit/general/auto_par_function.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/bin_pack_multiobj.mzn")]
    public void test_unit_general_bin_pack_multiobj()
    {
        var path = "unit/general/bin_pack_multiobj.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/builtins_arg_max.mzn")]
    public void test_unit_general_builtins_arg_max()
    {
        var path = "unit/general/builtins_arg_max.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/builtins_debug.mzn")]
    public void test_unit_general_builtins_debug()
    {
        var path = "unit/general/builtins_debug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/comprehension_var_ub.mzn")]
    public void test_unit_general_comprehension_var_ub()
    {
        var path = "unit/general/comprehension_var_ub.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/cse_ctx.mzn")]
    public void test_unit_general_cse_ctx()
    {
        var path = "unit/general/cse_ctx.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/enum_constructor_quoting.mzn")]
    public void test_unit_general_enum_constructor_quoting()
    {
        var path = "unit/general/enum_constructor_quoting.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/enum_order.mzn")]
    public void test_unit_general_enum_order()
    {
        var path = "unit/general/enum_order.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/eval_par_opt_set.mzn")]
    public void test_unit_general_eval_par_opt_set()
    {
        var path = "unit/general/eval_par_opt_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/fix_struct.mzn")]
    public void test_unit_general_fix_struct()
    {
        var path = "unit/general/fix_struct.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/iffall_bv.mzn")]
    public void test_unit_general_iffall_bv()
    {
        var path = "unit/general/iffall_bv.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/infinite_domain_bind.mzn")]
    public void test_unit_general_infinite_domain_bind()
    {
        var path = "unit/general/infinite_domain_bind.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/json_ignore.mzn")]
    public void test_unit_general_json_ignore()
    {
        var path = "unit/general/json_ignore.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/let_struct_domain.mzn")]
    public void test_unit_general_let_struct_domain()
    {
        var path = "unit/general/let_struct_domain.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_exists.mzn")]
    public void test_unit_general_md_exists()
    {
        var path = "unit/general/md_exists.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_forall.mzn")]
    public void test_unit_general_md_forall()
    {
        var path = "unit/general/md_forall.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_iffall.mzn")]
    public void test_unit_general_md_iffall()
    {
        var path = "unit/general/md_iffall.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_product_int.mzn")]
    public void test_unit_general_md_product_int()
    {
        var path = "unit/general/md_product_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_sum_float.mzn")]
    public void test_unit_general_md_sum_float()
    {
        var path = "unit/general/md_sum_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_sum_int.mzn")]
    public void test_unit_general_md_sum_int()
    {
        var path = "unit/general/md_sum_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/md_xorall.mzn")]
    public void test_unit_general_md_xorall()
    {
        var path = "unit/general/md_xorall.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/missing_ozn_decl.mzn")]
    public void test_unit_general_missing_ozn_decl()
    {
        var path = "unit/general/missing_ozn_decl.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mortgage.mzn")]
    public void test_unit_general_mortgage()
    {
        var path = "unit/general/mortgage.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit1.mzn")]
    public void test_unit_general_mzn_implicit1()
    {
        var path = "unit/general/mzn-implicit1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit2.mzn")]
    public void test_unit_general_mzn_implicit2()
    {
        var path = "unit/general/mzn-implicit2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit3.mzn")]
    public void test_unit_general_mzn_implicit3()
    {
        var path = "unit/general/mzn-implicit3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit4.mzn")]
    public void test_unit_general_mzn_implicit4()
    {
        var path = "unit/general/mzn-implicit4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit5.mzn")]
    public void test_unit_general_mzn_implicit5()
    {
        var path = "unit/general/mzn-implicit5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit6.mzn")]
    public void test_unit_general_mzn_implicit6()
    {
        var path = "unit/general/mzn-implicit6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit7.mzn")]
    public void test_unit_general_mzn_implicit7()
    {
        var path = "unit/general/mzn-implicit7.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn-implicit8.mzn")]
    public void test_unit_general_mzn_implicit8()
    {
        var path = "unit/general/mzn-implicit8.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn_div.mzn")]
    public void test_unit_general_mzn_div()
    {
        var path = "unit/general/mzn_div.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/mzn_mod.mzn")]
    public void test_unit_general_mzn_mod()
    {
        var path = "unit/general/mzn_mod.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/overload_bottom.mzn")]
    public void test_unit_general_overload_bottom()
    {
        var path = "unit/general/overload_bottom.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/pow_1.mzn")]
    public void test_unit_general_pow_1()
    {
        var path = "unit/general/pow_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/pow_2.mzn")]
    public void test_unit_general_pow_2()
    {
        var path = "unit/general/pow_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/pow_3.mzn")]
    public void test_unit_general_pow_3()
    {
        var path = "unit/general/pow_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/pow_4.mzn")]
    public void test_unit_general_pow_4()
    {
        var path = "unit/general/pow_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/pow_bounds.mzn")]
    public void test_unit_general_pow_bounds()
    {
        var path = "unit/general/pow_bounds.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/quoted_id_1.mzn")]
    public void test_unit_general_quoted_id_1()
    {
        var path = "unit/general/quoted_id_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/quoted_id_2.mzn")]
    public void test_unit_general_quoted_id_2()
    {
        var path = "unit/general/quoted_id_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/quoted_id_3.mzn")]
    public void test_unit_general_quoted_id_3()
    {
        var path = "unit/general/quoted_id_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/quoted_id_4.mzn")]
    public void test_unit_general_quoted_id_4()
    {
        var path = "unit/general/quoted_id_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/range_var_enum.mzn")]
    public void test_unit_general_range_var_enum()
    {
        var path = "unit/general/range_var_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test-search1.mzn")]
    public void test_unit_general_test_search1()
    {
        var path = "unit/general/test-search1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_array1.mzn")]
    public void test_unit_general_test_array1()
    {
        var path = "unit/general/test_array1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_array1and2d.mzn")]
    public void test_unit_general_test_array1and2d()
    {
        var path = "unit/general/test_array1and2d.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_array2.mzn")]
    public void test_unit_general_test_array2()
    {
        var path = "unit/general/test_array2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_array_as_generator.mzn")]
    public void test_unit_general_test_array_as_generator()
    {
        var path = "unit/general/test_array_as_generator.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_bad_lb_ub_dom-bad.mzn")]
    public void test_unit_general_test_bad_lb_ub_dom_bad()
    {
        var path = "unit/general/test_bad_lb_ub_dom-bad.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_bool_var_array_access.mzn")]
    public void test_unit_general_test_bool_var_array_access()
    {
        var path = "unit/general/test_bool_var_array_access.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_concat1.mzn")]
    public void test_unit_general_test_concat1()
    {
        var path = "unit/general/test_concat1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_concat2.mzn")]
    public void test_unit_general_test_concat2()
    {
        var path = "unit/general/test_concat2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_concat3.mzn")]
    public void test_unit_general_test_concat3()
    {
        var path = "unit/general/test_concat3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_empty_enum.mzn")]
    public void test_unit_general_test_empty_enum()
    {
        var path = "unit/general/test_empty_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_is_fixed.mzn")]
    public void test_unit_general_test_is_fixed()
    {
        var path = "unit/general/test_is_fixed.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_join1.mzn")]
    public void test_unit_general_test_join1()
    {
        var path = "unit/general/test_join1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_join2.mzn")]
    public void test_unit_general_test_join2()
    {
        var path = "unit/general/test_join2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_join3.mzn")]
    public void test_unit_general_test_join3()
    {
        var path = "unit/general/test_join3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_lb_ub_array_int.mzn")]
    public void test_unit_general_test_lb_ub_array_int()
    {
        var path = "unit/general/test_lb_ub_array_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_lb_ub_dom_int.mzn")]
    public void test_unit_general_test_lb_ub_dom_int()
    {
        var path = "unit/general/test_lb_ub_dom_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_lb_ub_float.mzn")]
    public void test_unit_general_test_lb_ub_float()
    {
        var path = "unit/general/test_lb_ub_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_let_complex.mzn")]
    public void test_unit_general_test_let_complex()
    {
        var path = "unit/general/test_let_complex.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_let_par_array.mzn")]
    public void test_unit_general_test_let_par_array()
    {
        var path = "unit/general/test_let_par_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_let_simple.mzn")]
    public void test_unit_general_test_let_simple()
    {
        var path = "unit/general/test_let_simple.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_let_var_array.mzn")]
    public void test_unit_general_test_let_var_array()
    {
        var path = "unit/general/test_let_var_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_let_with_annotation.mzn")]
    public void test_unit_general_test_let_with_annotation()
    {
        var path = "unit/general/test_let_with_annotation.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_min_var_array.mzn")]
    public void test_unit_general_test_min_var_array()
    {
        var path = "unit/general/test_min_var_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_mod_bounds.mzn")]
    public void test_unit_general_test_mod_bounds()
    {
        var path = "unit/general/test_mod_bounds.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_negated_and.mzn")]
    public void test_unit_general_test_negated_and()
    {
        var path = "unit/general/test_negated_and.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_negated_and_or.mzn")]
    public void test_unit_general_test_negated_and_or()
    {
        var path = "unit/general/test_negated_and_or.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_negated_let_good.mzn")]
    public void test_unit_general_test_negated_let_good()
    {
        var path = "unit/general/test_negated_let_good.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_negated_let_good_2.mzn")]
    public void test_unit_general_test_negated_let_good_2()
    {
        var path = "unit/general/test_negated_let_good_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_negated_or.mzn")]
    public void test_unit_general_test_negated_or()
    {
        var path = "unit/general/test_negated_or.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_par_set_element.mzn")]
    public void test_unit_general_test_par_set_element()
    {
        var path = "unit/general/test_par_set_element.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_par_set_id_array_index_sets.mzn")]
    public void test_unit_general_test_par_set_id_array_index_sets()
    {
        var path = "unit/general/test_par_set_id_array_index_sets.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_queens.mzn")]
    public void test_unit_general_test_queens()
    {
        var path = "unit/general/test_queens.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_reified_element_constraint.mzn")]
    public void test_unit_general_test_reified_element_constraint()
    {
        var path = "unit/general/test_reified_element_constraint.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_reified_let_good.mzn")]
    public void test_unit_general_test_reified_let_good()
    {
        var path = "unit/general/test_reified_let_good.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_rounding_a.mzn")]
    public void test_unit_general_test_rounding_a()
    {
        var path = "unit/general/test_rounding_a.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_rounding_b.mzn")]
    public void test_unit_general_test_rounding_b()
    {
        var path = "unit/general/test_rounding_b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_rounding_c.mzn")]
    public void test_unit_general_test_rounding_c()
    {
        var path = "unit/general/test_rounding_c.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_same.mzn")]
    public void test_unit_general_test_same()
    {
        var path = "unit/general/test_same.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_set_inequality_par.mzn")]
    public void test_unit_general_test_set_inequality_par()
    {
        var path = "unit/general/test_set_inequality_par.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_set_lt_1.mzn")]
    public void test_unit_general_test_set_lt_1()
    {
        var path = "unit/general/test_set_lt_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_set_lt_2.mzn")]
    public void test_unit_general_test_set_lt_2()
    {
        var path = "unit/general/test_set_lt_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_set_lt_3.mzn")]
    public void test_unit_general_test_set_lt_3()
    {
        var path = "unit/general/test_set_lt_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_string_array_var.mzn")]
    public void test_unit_general_test_string_array_var()
    {
        var path = "unit/general/test_string_array_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_string_cmp.mzn")]
    public void test_unit_general_test_string_cmp()
    {
        var path = "unit/general/test_string_cmp.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_string_var.mzn")]
    public void test_unit_general_test_string_var()
    {
        var path = "unit/general/test_string_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_string_with_quote.mzn")]
    public void test_unit_general_test_string_with_quote()
    {
        var path = "unit/general/test_string_with_quote.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_times_int_float_eq.mzn")]
    public void test_unit_general_test_times_int_float_eq()
    {
        var path = "unit/general/test_times_int_float_eq.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_times_int_float_eq__defaultopt.mzn")]
    public void test_unit_general_test_times_int_float_eq__defaultopt()
    {
        var path = "unit/general/test_times_int_float_eq__defaultopt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_to_enum.mzn")]
    public void test_unit_general_test_to_enum()
    {
        var path = "unit/general/test_to_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_undefined_enum.mzn")]
    public void test_unit_general_test_undefined_enum()
    {
        var path = "unit/general/test_undefined_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_var_array.mzn")]
    public void test_unit_general_test_var_array()
    {
        var path = "unit/general/test_var_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_var_array_access.mzn")]
    public void test_unit_general_test_var_array_access()
    {
        var path = "unit/general/test_var_array_access.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_var_prod.mzn")]
    public void test_unit_general_test_var_prod()
    {
        var path = "unit/general/test_var_prod.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_var_set_assignment.mzn")]
    public void test_unit_general_test_var_set_assignment()
    {
        var path = "unit/general/test_var_set_assignment.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/test_var_set_element.mzn")]
    public void test_unit_general_test_var_set_element()
    {
        var path = "unit/general/test_var_set_element.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/unicode_file_name_μ.mzn")]
    public void test_unit_general_unicode_file_name_μ()
    {
        var path = "unit/general/unicode_file_name_μ.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/wolfgoatetc.mzn")]
    public void test_unit_general_wolfgoatetc()
    {
        var path = "unit/general/wolfgoatetc.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/general/xorall_bv.mzn")]
    public void test_unit_general_xorall_bv()
    {
        var path = "unit/general/xorall_bv.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/typecheck_globals.mzn")]
    public void test_unit_globals_typecheck_globals()
    {
        var path = "unit/globals/typecheck_globals.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/anon_enum_json.mzn")]
    public void test_unit_json_anon_enum_json()
    {
        var path = "unit/json/anon_enum_json.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/coerce_enum_str.mzn")]
    public void test_unit_json_coerce_enum_str()
    {
        var path = "unit/json/coerce_enum_str.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/coerce_indices.mzn")]
    public void test_unit_json_coerce_indices()
    {
        var path = "unit/json/coerce_indices.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/coerce_set.mzn")]
    public void test_unit_json_coerce_set()
    {
        var path = "unit/json/coerce_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/enum_array_xd.mzn")]
    public void test_unit_json_enum_array_xd()
    {
        var path = "unit/json/enum_array_xd.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/enum_constructor_basic.mzn")]
    public void test_unit_json_enum_constructor_basic()
    {
        var path = "unit/json/enum_constructor_basic.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/enum_constructor_basic_2.mzn")]
    public void test_unit_json_enum_constructor_basic_2()
    {
        var path = "unit/json/enum_constructor_basic_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/enum_constructor_int.mzn")]
    public void test_unit_json_enum_constructor_int()
    {
        var path = "unit/json/enum_constructor_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/enum_constructor_nested.mzn")]
    public void test_unit_json_enum_constructor_nested()
    {
        var path = "unit/json/enum_constructor_nested.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/enum_escaping.mzn")]
    public void test_unit_json_enum_escaping()
    {
        var path = "unit/json/enum_escaping.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/float_json_exponent.mzn")]
    public void test_unit_json_float_json_exponent()
    {
        var path = "unit/json/float_json_exponent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_array2d_set.mzn")]
    public void test_unit_json_json_array2d_set()
    {
        var path = "unit/json/json_array2d_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_enum_def.mzn")]
    public void test_unit_json_json_enum_def()
    {
        var path = "unit/json/json_enum_def.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_input_1.mzn")]
    public void test_unit_json_json_input_1()
    {
        var path = "unit/json/json_input_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_input_multidim_enum_str.mzn")]
    public void test_unit_json_json_input_multidim_enum_str()
    {
        var path = "unit/json/json_input_multidim_enum_str.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_input_set.mzn")]
    public void test_unit_json_json_input_set()
    {
        var path = "unit/json/json_input_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_input_str.mzn")]
    public void test_unit_json_json_input_str()
    {
        var path = "unit/json/json_input_str.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/json_unicode_escapes.mzn")]
    public void test_unit_json_json_unicode_escapes()
    {
        var path = "unit/json/json_unicode_escapes.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/mult_dim_enum.mzn")]
    public void test_unit_json_mult_dim_enum()
    {
        var path = "unit/json/mult_dim_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/record_json_input.mzn")]
    public void test_unit_json_record_json_input()
    {
        var path = "unit/json/record_json_input.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/json/tuple_json_input.mzn")]
    public void test_unit_json_tuple_json_input()
    {
        var path = "unit/json/tuple_json_input.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/complete.mzn")]
    public void test_unit_on_restart_complete()
    {
        var path = "unit/on_restart/complete.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/last_val_bool.mzn")]
    public void test_unit_on_restart_last_val_bool()
    {
        var path = "unit/on_restart/last_val_bool.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/last_val_float.mzn")]
    public void test_unit_on_restart_last_val_float()
    {
        var path = "unit/on_restart/last_val_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/last_val_int.mzn")]
    public void test_unit_on_restart_last_val_int()
    {
        var path = "unit/on_restart/last_val_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/last_val_set.mzn")]
    public void test_unit_on_restart_last_val_set()
    {
        var path = "unit/on_restart/last_val_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/sol_bool.mzn")]
    public void test_unit_on_restart_sol_bool()
    {
        var path = "unit/on_restart/sol_bool.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/sol_float.mzn")]
    public void test_unit_on_restart_sol_float()
    {
        var path = "unit/on_restart/sol_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/sol_int.mzn")]
    public void test_unit_on_restart_sol_int()
    {
        var path = "unit/on_restart/sol_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/on_restart/sol_set.mzn")]
    public void test_unit_on_restart_sol_set()
    {
        var path = "unit/on_restart/sol_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/bind_absent.mzn")]
    public void test_unit_optional_bind_absent()
    {
        var path = "unit/optional/bind_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/comprehension_of_absent_1.mzn")]
    public void test_unit_optional_comprehension_of_absent_1()
    {
        var path = "unit/optional/comprehension_of_absent_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/comprehension_of_absent_2.mzn")]
    public void test_unit_optional_comprehension_of_absent_2()
    {
        var path = "unit/optional/comprehension_of_absent_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/conj_absent_1.mzn")]
    public void test_unit_optional_conj_absent_1()
    {
        var path = "unit/optional/conj_absent_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/conj_absent_2.mzn")]
    public void test_unit_optional_conj_absent_2()
    {
        var path = "unit/optional/conj_absent_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/fn_return_array_absent.mzn")]
    public void test_unit_optional_fn_return_array_absent()
    {
        var path = "unit/optional/fn_return_array_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/opt_array_access.mzn")]
    public void test_unit_optional_opt_array_access()
    {
        var path = "unit/optional/opt_array_access.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/opt_array_access_absent.mzn")]
    public void test_unit_optional_opt_array_access_absent()
    {
        var path = "unit/optional/opt_array_access_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/opt_array_access_absent_empty.mzn")]
    public void test_unit_optional_opt_array_access_absent_empty()
    {
        var path = "unit/optional/opt_array_access_absent_empty.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/opt_string_comprehension.mzn")]
    public void test_unit_optional_opt_string_comprehension()
    {
        var path = "unit/optional/opt_string_comprehension.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/opt_top_absent.mzn")]
    public void test_unit_optional_opt_top_absent()
    {
        var path = "unit/optional/opt_top_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-binop-flatten.mzn")]
    public void test_unit_optional_test_opt_binop_flatten()
    {
        var path = "unit/optional/test-opt-binop-flatten.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-bool-1.mzn")]
    public void test_unit_optional_test_opt_bool_1()
    {
        var path = "unit/optional/test-opt-bool-1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-bool-2.mzn")]
    public void test_unit_optional_test_opt_bool_2()
    {
        var path = "unit/optional/test-opt-bool-2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-bool-3.mzn")]
    public void test_unit_optional_test_opt_bool_3()
    {
        var path = "unit/optional/test-opt-bool-3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-bool-4.mzn")]
    public void test_unit_optional_test_opt_bool_4()
    {
        var path = "unit/optional/test-opt-bool-4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-bool-5.mzn")]
    public void test_unit_optional_test_opt_bool_5()
    {
        var path = "unit/optional/test-opt-bool-5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-bool-6.mzn")]
    public void test_unit_optional_test_opt_bool_6()
    {
        var path = "unit/optional/test-opt-bool-6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-compute-bounds.mzn")]
    public void test_unit_optional_test_opt_compute_bounds()
    {
        var path = "unit/optional/test-opt-compute-bounds.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-float-1.mzn")]
    public void test_unit_optional_test_opt_float_1()
    {
        var path = "unit/optional/test-opt-float-1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-float-2.mzn")]
    public void test_unit_optional_test_opt_float_2()
    {
        var path = "unit/optional/test-opt-float-2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-if-then-else.mzn")]
    public void test_unit_optional_test_opt_if_then_else()
    {
        var path = "unit/optional/test-opt-if-then-else.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-int-1.mzn")]
    public void test_unit_optional_test_opt_int_1()
    {
        var path = "unit/optional/test-opt-int-1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-int-2.mzn")]
    public void test_unit_optional_test_opt_int_2()
    {
        var path = "unit/optional/test-opt-int-2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test-opt-int-3.mzn")]
    public void test_unit_optional_test_opt_int_3()
    {
        var path = "unit/optional/test-opt-int-3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_bug_456.mzn")]
    public void test_unit_optional_test_bug_456()
    {
        var path = "unit/optional/test_bug_456.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_count_set.mzn")]
    public void test_unit_optional_test_count_set()
    {
        var path = "unit/optional/test_count_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_deopt_absent.mzn")]
    public void test_unit_optional_test_deopt_absent()
    {
        var path = "unit/optional/test_deopt_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_if_then_else_opt_bool.mzn")]
    public void test_unit_optional_test_if_then_else_opt_bool()
    {
        var path = "unit/optional/test_if_then_else_opt_bool.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_if_then_else_opt_float.mzn")]
    public void test_unit_optional_test_if_then_else_opt_float()
    {
        var path = "unit/optional/test_if_then_else_opt_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_if_then_else_opt_int.mzn")]
    public void test_unit_optional_test_if_then_else_opt_int()
    {
        var path = "unit/optional/test_if_then_else_opt_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_if_then_else_var_opt_bool.mzn")]
    public void test_unit_optional_test_if_then_else_var_opt_bool()
    {
        var path = "unit/optional/test_if_then_else_var_opt_bool.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_if_then_else_var_opt_float.mzn")]
    public void test_unit_optional_test_if_then_else_var_opt_float()
    {
        var path = "unit/optional/test_if_then_else_var_opt_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_if_then_else_var_opt_int.mzn")]
    public void test_unit_optional_test_if_then_else_var_opt_int()
    {
        var path = "unit/optional/test_if_then_else_var_opt_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_optional_not_absent.mzn")]
    public void test_unit_optional_test_optional_not_absent()
    {
        var path = "unit/optional/test_optional_not_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_opt_comprehension.mzn")]
    public void test_unit_optional_test_opt_comprehension()
    {
        var path = "unit/optional/test_opt_comprehension.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_opt_dom_empty.mzn")]
    public void test_unit_optional_test_opt_dom_empty()
    {
        var path = "unit/optional/test_opt_dom_empty.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_opt_dom_empty_no_absent_zero.mzn")]
    public void test_unit_optional_test_opt_dom_empty_no_absent_zero()
    {
        var path = "unit/optional/test_opt_dom_empty_no_absent_zero.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_opt_max.mzn")]
    public void test_unit_optional_test_opt_max()
    {
        var path = "unit/optional/test_opt_max.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/optional/test_opt_min.mzn")]
    public void test_unit_optional_test_opt_min()
    {
        var path = "unit/optional/test_opt_min.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/arg-reif-output.mzn")]
    public void test_unit_output_arg_reif_output()
    {
        var path = "unit/output/arg-reif-output.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/array-ann.mzn")]
    public void test_unit_output_array_ann()
    {
        var path = "unit/output/array-ann.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/array_of_array.mzn")]
    public void test_unit_output_array_of_array()
    {
        var path = "unit/output/array_of_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/bug288a.mzn")]
    public void test_unit_output_bug288a()
    {
        var path = "unit/output/bug288a.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/bug288b.mzn")]
    public void test_unit_output_bug288b()
    {
        var path = "unit/output/bug288b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/bug288c.mzn")]
    public void test_unit_output_bug288c()
    {
        var path = "unit/output/bug288c.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/ctx_ann.mzn")]
    public void test_unit_output_ctx_ann()
    {
        var path = "unit/output/ctx_ann.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/dzn_output_array.mzn")]
    public void test_unit_output_dzn_output_array()
    {
        var path = "unit/output/dzn_output_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/enum_constructor_functions.mzn")]
    public void test_unit_output_enum_constructor_functions()
    {
        var path = "unit/output/enum_constructor_functions.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/json_ann.mzn")]
    public void test_unit_output_json_ann()
    {
        var path = "unit/output/json_ann.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/local_output.mzn")]
    public void test_unit_output_local_output()
    {
        var path = "unit/output/local_output.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/mzn_bottom1.mzn")]
    public void test_unit_output_mzn_bottom1()
    {
        var path = "unit/output/mzn_bottom1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/mzn_bottom2.mzn")]
    public void test_unit_output_mzn_bottom2()
    {
        var path = "unit/output/mzn_bottom2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/mzn_bottom3.mzn")]
    public void test_unit_output_mzn_bottom3()
    {
        var path = "unit/output/mzn_bottom3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/mzn_bottom4.mzn")]
    public void test_unit_output_mzn_bottom4()
    {
        var path = "unit/output/mzn_bottom4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/mzn_bottom5.mzn")]
    public void test_unit_output_mzn_bottom5()
    {
        var path = "unit/output/mzn_bottom5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/mzn_bottom6.mzn")]
    public void test_unit_output_mzn_bottom6()
    {
        var path = "unit/output/mzn_bottom6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_annotations_1.mzn")]
    public void test_unit_output_output_annotations_1()
    {
        var path = "unit/output/output_annotations_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_annotations_2.mzn")]
    public void test_unit_output_output_annotations_2()
    {
        var path = "unit/output/output_annotations_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_annotations_3.mzn")]
    public void test_unit_output_output_annotations_3()
    {
        var path = "unit/output/output_annotations_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_annotations_4.mzn")]
    public void test_unit_output_output_annotations_4()
    {
        var path = "unit/output/output_annotations_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_1.mzn")]
    public void test_unit_output_output_sections_1()
    {
        var path = "unit/output/output_sections_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_2.mzn")]
    public void test_unit_output_output_sections_2()
    {
        var path = "unit/output/output_sections_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_3.mzn")]
    public void test_unit_output_output_sections_3()
    {
        var path = "unit/output/output_sections_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_4.mzn")]
    public void test_unit_output_output_sections_4()
    {
        var path = "unit/output/output_sections_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_5.mzn")]
    public void test_unit_output_output_sections_5()
    {
        var path = "unit/output/output_sections_5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_6.mzn")]
    public void test_unit_output_output_sections_6()
    {
        var path = "unit/output/output_sections_6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/output_sections_7.mzn")]
    public void test_unit_output_output_sections_7()
    {
        var path = "unit/output/output_sections_7.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/quoted_id_ozn.mzn")]
    public void test_unit_output_quoted_id_ozn()
    {
        var path = "unit/output/quoted_id_ozn.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/record_access_printing.mzn")]
    public void test_unit_output_record_access_printing()
    {
        var path = "unit/output/record_access_printing.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show2d_empty.mzn")]
    public void test_unit_output_show2d_empty()
    {
        var path = "unit/output/show2d_empty.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show3d_empty.mzn")]
    public void test_unit_output_show3d_empty()
    {
        var path = "unit/output/show3d_empty.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show_empty.mzn")]
    public void test_unit_output_show_empty()
    {
        var path = "unit/output/show_empty.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show_float.mzn")]
    public void test_unit_output_show_float()
    {
        var path = "unit/output/show_float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show_float_set.mzn")]
    public void test_unit_output_show_float_set()
    {
        var path = "unit/output/show_float_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show_int.mzn")]
    public void test_unit_output_show_int()
    {
        var path = "unit/output/show_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/show_int_set.mzn")]
    public void test_unit_output_show_int_set()
    {
        var path = "unit/output/show_int_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/test-in-output.mzn")]
    public void test_unit_output_test_in_output()
    {
        var path = "unit/output/test-in-output.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/var_enum.mzn")]
    public void test_unit_output_var_enum()
    {
        var path = "unit/output/var_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/very_empty.mzn")]
    public void test_unit_output_very_empty()
    {
        var path = "unit/output/very_empty.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/output/very_empty_set.mzn")]
    public void test_unit_output_very_empty_set()
    {
        var path = "unit/output/very_empty_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/param_file/param_file_array.mzn")]
    public void test_unit_param_file_param_file_array()
    {
        var path = "unit/param_file/param_file_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/param_file/param_file_nested_object.mzn")]
    public void test_unit_param_file_param_file_nested_object()
    {
        var path = "unit/param_file/param_file_nested_object.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/param_file/param_file_resolution.mzn")]
    public void test_unit_param_file_param_file_resolution()
    {
        var path = "unit/param_file/param_file_resolution.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/absent_id_crash.mzn")]
    public void test_unit_regression_absent_id_crash()
    {
        var path = "unit/regression/absent_id_crash.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/abs_bug.mzn")]
    public void test_unit_regression_abs_bug()
    {
        var path = "unit/regression/abs_bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/arg-reif-array-float.mzn")]
    public void test_unit_regression_arg_reif_array_float()
    {
        var path = "unit/regression/arg-reif-array-float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/arg-reif-array-int.mzn")]
    public void test_unit_regression_arg_reif_array_int()
    {
        var path = "unit/regression/arg-reif-array-int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/arg-reif-float.mzn")]
    public void test_unit_regression_arg_reif_float()
    {
        var path = "unit/regression/arg-reif-float.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/arg-reif-int-set.mzn")]
    public void test_unit_regression_arg_reif_int_set()
    {
        var path = "unit/regression/arg-reif-int-set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/arg-reif-int.mzn")]
    public void test_unit_regression_arg_reif_int()
    {
        var path = "unit/regression/arg-reif-int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/array_of_empty_sets.mzn")]
    public void test_unit_regression_array_of_empty_sets()
    {
        var path = "unit/regression/array_of_empty_sets.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/array_set_element_nosets.mzn")]
    public void test_unit_regression_array_set_element_nosets()
    {
        var path = "unit/regression/array_set_element_nosets.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/array_var_set_element_nosets.mzn")]
    public void test_unit_regression_array_var_set_element_nosets()
    {
        var path = "unit/regression/array_var_set_element_nosets.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/assign_reverse_map.mzn")]
    public void test_unit_regression_assign_reverse_map()
    {
        var path = "unit/regression/assign_reverse_map.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/big_array_lit.mzn")]
    public void test_unit_regression_big_array_lit()
    {
        var path = "unit/regression/big_array_lit.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bind-defines-var.mzn")]
    public void test_unit_regression_bind_defines_var()
    {
        var path = "unit/regression/bind-defines-var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/binop_mult_gclock.mzn")]
    public void test_unit_regression_binop_mult_gclock()
    {
        var path = "unit/regression/binop_mult_gclock.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bizarre.mzn")]
    public void test_unit_regression_bizarre()
    {
        var path = "unit/regression/bizarre.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bool2float_let.mzn")]
    public void test_unit_regression_bool2float_let()
    {
        var path = "unit/regression/bool2float_let.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bounds_for_linear_01_max_0.mzn")]
    public void test_unit_regression_bounds_for_linear_01_max_0()
    {
        var path = "unit/regression/bounds_for_linear_01_max_0.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bounds_for_linear_01_max_1.mzn")]
    public void test_unit_regression_bounds_for_linear_01_max_1()
    {
        var path = "unit/regression/bounds_for_linear_01_max_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bounds_for_linear_01_min_0.mzn")]
    public void test_unit_regression_bounds_for_linear_01_min_0()
    {
        var path = "unit/regression/bounds_for_linear_01_min_0.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bounds_for_linear_01_min_1.mzn")]
    public void test_unit_regression_bounds_for_linear_01_min_1()
    {
        var path = "unit/regression/bounds_for_linear_01_min_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug109.mzn")]
    public void test_unit_regression_bug109()
    {
        var path = "unit/regression/bug109.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug110.mzn")]
    public void test_unit_regression_bug110()
    {
        var path = "unit/regression/bug110.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug131.mzn")]
    public void test_unit_regression_bug131()
    {
        var path = "unit/regression/bug131.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug141.mzn")]
    public void test_unit_regression_bug141()
    {
        var path = "unit/regression/bug141.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug202.mzn")]
    public void test_unit_regression_bug202()
    {
        var path = "unit/regression/bug202.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug212.mzn")]
    public void test_unit_regression_bug212()
    {
        var path = "unit/regression/bug212.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug222.mzn")]
    public void test_unit_regression_bug222()
    {
        var path = "unit/regression/bug222.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug244.mzn")]
    public void test_unit_regression_bug244()
    {
        var path = "unit/regression/bug244.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug256.mzn")]
    public void test_unit_regression_bug256()
    {
        var path = "unit/regression/bug256.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug256b.mzn")]
    public void test_unit_regression_bug256b()
    {
        var path = "unit/regression/bug256b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug259.mzn")]
    public void test_unit_regression_bug259()
    {
        var path = "unit/regression/bug259.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug269.mzn")]
    public void test_unit_regression_bug269()
    {
        var path = "unit/regression/bug269.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug282.mzn")]
    public void test_unit_regression_bug282()
    {
        var path = "unit/regression/bug282.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug283.mzn")]
    public void test_unit_regression_bug283()
    {
        var path = "unit/regression/bug283.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug284.mzn")]
    public void test_unit_regression_bug284()
    {
        var path = "unit/regression/bug284.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug287.mzn")]
    public void test_unit_regression_bug287()
    {
        var path = "unit/regression/bug287.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug290_orig.mzn")]
    public void test_unit_regression_bug290_orig()
    {
        var path = "unit/regression/bug290_orig.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug290_simple.mzn")]
    public void test_unit_regression_bug290_simple()
    {
        var path = "unit/regression/bug290_simple.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug312.mzn")]
    public void test_unit_regression_bug312()
    {
        var path = "unit/regression/bug312.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug318_orig.mzn")]
    public void test_unit_regression_bug318_orig()
    {
        var path = "unit/regression/bug318_orig.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug335.mzn")]
    public void test_unit_regression_bug335()
    {
        var path = "unit/regression/bug335.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug337.mzn")]
    public void test_unit_regression_bug337()
    {
        var path = "unit/regression/bug337.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug337_mod.mzn")]
    public void test_unit_regression_bug337_mod()
    {
        var path = "unit/regression/bug337_mod.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug341.mzn")]
    public void test_unit_regression_bug341()
    {
        var path = "unit/regression/bug341.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug347.mzn")]
    public void test_unit_regression_bug347()
    {
        var path = "unit/regression/bug347.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug380.mzn")]
    public void test_unit_regression_bug380()
    {
        var path = "unit/regression/bug380.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug45.mzn")]
    public void test_unit_regression_bug45()
    {
        var path = "unit/regression/bug45.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug52.mzn")]
    public void test_unit_regression_bug52()
    {
        var path = "unit/regression/bug52.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug532.mzn")]
    public void test_unit_regression_bug532()
    {
        var path = "unit/regression/bug532.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug534.mzn")]
    public void test_unit_regression_bug534()
    {
        var path = "unit/regression/bug534.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug536.mzn")]
    public void test_unit_regression_bug536()
    {
        var path = "unit/regression/bug536.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug552.mzn")]
    public void test_unit_regression_bug552()
    {
        var path = "unit/regression/bug552.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug565.mzn")]
    public void test_unit_regression_bug565()
    {
        var path = "unit/regression/bug565.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug570.mzn")]
    public void test_unit_regression_bug570()
    {
        var path = "unit/regression/bug570.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug620.mzn")]
    public void test_unit_regression_bug620()
    {
        var path = "unit/regression/bug620.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug635.mzn")]
    public void test_unit_regression_bug635()
    {
        var path = "unit/regression/bug635.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug67.mzn")]
    public void test_unit_regression_bug67()
    {
        var path = "unit/regression/bug67.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug68b.mzn")]
    public void test_unit_regression_bug68b()
    {
        var path = "unit/regression/bug68b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug69_1.mzn")]
    public void test_unit_regression_bug69_1()
    {
        var path = "unit/regression/bug69_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug69_2.mzn")]
    public void test_unit_regression_bug69_2()
    {
        var path = "unit/regression/bug69_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug69_3.mzn")]
    public void test_unit_regression_bug69_3()
    {
        var path = "unit/regression/bug69_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug69_4.mzn")]
    public void test_unit_regression_bug69_4()
    {
        var path = "unit/regression/bug69_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug70.mzn")]
    public void test_unit_regression_bug70()
    {
        var path = "unit/regression/bug70.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug71_1.mzn")]
    public void test_unit_regression_bug71_1()
    {
        var path = "unit/regression/bug71_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug71_2.mzn")]
    public void test_unit_regression_bug71_2()
    {
        var path = "unit/regression/bug71_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug82.mzn")]
    public void test_unit_regression_bug82()
    {
        var path = "unit/regression/bug82.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug85.mzn")]
    public void test_unit_regression_bug85()
    {
        var path = "unit/regression/bug85.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug_08.mzn")]
    public void test_unit_regression_bug_08()
    {
        var path = "unit/regression/bug_08.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug_629.mzn")]
    public void test_unit_regression_bug_629()
    {
        var path = "unit/regression/bug_629.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug_empty_enum_extension.mzn")]
    public void test_unit_regression_bug_empty_enum_extension()
    {
        var path = "unit/regression/bug_empty_enum_extension.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug_opt_polymorphic.mzn")]
    public void test_unit_regression_bug_opt_polymorphic()
    {
        var path = "unit/regression/bug_opt_polymorphic.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/bug_r7995.mzn")]
    public void test_unit_regression_bug_r7995()
    {
        var path = "unit/regression/bug_r7995.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/cardinality_atmost_partition.mzn")]
    public void test_unit_regression_cardinality_atmost_partition()
    {
        var path = "unit/regression/cardinality_atmost_partition.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/card_flatten_lb.mzn")]
    public void test_unit_regression_card_flatten_lb()
    {
        var path = "unit/regression/card_flatten_lb.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/change_partition.mzn")]
    public void test_unit_regression_change_partition()
    {
        var path = "unit/regression/change_partition.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/checker_mzn_check_var.mzn")]
    public void test_unit_regression_checker_mzn_check_var()
    {
        var path = "unit/regression/checker_mzn_check_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/checker_opt.mzn")]
    public void test_unit_regression_checker_opt()
    {
        var path = "unit/regression/checker_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/checker_params.mzn")]
    public void test_unit_regression_checker_params()
    {
        var path = "unit/regression/checker_params.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/checker_same_var.mzn")]
    public void test_unit_regression_checker_same_var()
    {
        var path = "unit/regression/checker_same_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/checker_var_bug.mzn")]
    public void test_unit_regression_checker_var_bug()
    {
        var path = "unit/regression/checker_var_bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/check_dom_float_array.mzn")]
    public void test_unit_regression_check_dom_float_array()
    {
        var path = "unit/regression/check_dom_float_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/coerce_set_to_array_1.mzn")]
    public void test_unit_regression_coerce_set_to_array_1()
    {
        var path = "unit/regression/coerce_set_to_array_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/coercion_par.mzn")]
    public void test_unit_regression_coercion_par()
    {
        var path = "unit/regression/coercion_par.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/comprehension_where.mzn")]
    public void test_unit_regression_comprehension_where()
    {
        var path = "unit/regression/comprehension_where.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/comp_in_empty_set.mzn")]
    public void test_unit_regression_comp_in_empty_set()
    {
        var path = "unit/regression/comp_in_empty_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/constructor_of_set.mzn")]
    public void test_unit_regression_constructor_of_set()
    {
        var path = "unit/regression/constructor_of_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/cse_array_lit.mzn")]
    public void test_unit_regression_cse_array_lit()
    {
        var path = "unit/regression/cse_array_lit.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/decision_tree_binary.mzn")]
    public void test_unit_regression_decision_tree_binary()
    {
        var path = "unit/regression/decision_tree_binary.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/empty-array1d.mzn")]
    public void test_unit_regression_empty_array1d()
    {
        var path = "unit/regression/empty-array1d.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/enigma_1568.mzn")]
    public void test_unit_regression_enigma_1568()
    {
        var path = "unit/regression/enigma_1568.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/flatten_comp_in.mzn")]
    public void test_unit_regression_flatten_comp_in()
    {
        var path = "unit/regression/flatten_comp_in.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/flatten_comp_in2.mzn")]
    public void test_unit_regression_flatten_comp_in2()
    {
        var path = "unit/regression/flatten_comp_in2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/flat_cv_call.mzn")]
    public void test_unit_regression_flat_cv_call()
    {
        var path = "unit/regression/flat_cv_call.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/flat_cv_let.mzn")]
    public void test_unit_regression_flat_cv_let()
    {
        var path = "unit/regression/flat_cv_let.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/flat_set_lit.mzn")]
    public void test_unit_regression_flat_set_lit()
    {
        var path = "unit/regression/flat_set_lit.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/flipstrip_simple.mzn")]
    public void test_unit_regression_flipstrip_simple()
    {
        var path = "unit/regression/flipstrip_simple.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/float_card.mzn")]
    public void test_unit_regression_float_card()
    {
        var path = "unit/regression/float_card.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/float_ceil_floor.mzn")]
    public void test_unit_regression_float_ceil_floor()
    {
        var path = "unit/regression/float_ceil_floor.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/float_opt_crash.mzn")]
    public void test_unit_regression_float_opt_crash()
    {
        var path = "unit/regression/float_opt_crash.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/follow_id_absent_crash.mzn")]
    public void test_unit_regression_follow_id_absent_crash()
    {
        var path = "unit/regression/follow_id_absent_crash.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github537.mzn")]
    public void test_unit_regression_github537()
    {
        var path = "unit/regression/github537.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_638_reduced.mzn")]
    public void test_unit_regression_github_638_reduced()
    {
        var path = "unit/regression/github_638_reduced.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_639_part1.mzn")]
    public void test_unit_regression_github_639_part1()
    {
        var path = "unit/regression/github_639_part1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_639_part2.mzn")]
    public void test_unit_regression_github_639_part2()
    {
        var path = "unit/regression/github_639_part2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_644_a.mzn")]
    public void test_unit_regression_github_644_a()
    {
        var path = "unit/regression/github_644_a.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_644_b.mzn")]
    public void test_unit_regression_github_644_b()
    {
        var path = "unit/regression/github_644_b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_644_c.mzn")]
    public void test_unit_regression_github_644_c()
    {
        var path = "unit/regression/github_644_c.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_644_d.mzn")]
    public void test_unit_regression_github_644_d()
    {
        var path = "unit/regression/github_644_d.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_644_e.mzn")]
    public void test_unit_regression_github_644_e()
    {
        var path = "unit/regression/github_644_e.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_646.mzn")]
    public void test_unit_regression_github_646()
    {
        var path = "unit/regression/github_646.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_660a.mzn")]
    public void test_unit_regression_github_660a()
    {
        var path = "unit/regression/github_660a.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_660b.mzn")]
    public void test_unit_regression_github_660b()
    {
        var path = "unit/regression/github_660b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_661_part1.mzn")]
    public void test_unit_regression_github_661_part1()
    {
        var path = "unit/regression/github_661_part1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_661_part2.mzn")]
    public void test_unit_regression_github_661_part2()
    {
        var path = "unit/regression/github_661_part2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_664.mzn")]
    public void test_unit_regression_github_664()
    {
        var path = "unit/regression/github_664.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_666.mzn")]
    public void test_unit_regression_github_666()
    {
        var path = "unit/regression/github_666.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_667.mzn")]
    public void test_unit_regression_github_667()
    {
        var path = "unit/regression/github_667.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_668.mzn")]
    public void test_unit_regression_github_668()
    {
        var path = "unit/regression/github_668.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_669.mzn")]
    public void test_unit_regression_github_669()
    {
        var path = "unit/regression/github_669.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_670.mzn")]
    public void test_unit_regression_github_670()
    {
        var path = "unit/regression/github_670.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_671.mzn")]
    public void test_unit_regression_github_671()
    {
        var path = "unit/regression/github_671.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_673.mzn")]
    public void test_unit_regression_github_673()
    {
        var path = "unit/regression/github_673.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_674.mzn")]
    public void test_unit_regression_github_674()
    {
        var path = "unit/regression/github_674.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_675a.mzn")]
    public void test_unit_regression_github_675a()
    {
        var path = "unit/regression/github_675a.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_675b.mzn")]
    public void test_unit_regression_github_675b()
    {
        var path = "unit/regression/github_675b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_681.mzn")]
    public void test_unit_regression_github_681()
    {
        var path = "unit/regression/github_681.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_683.mzn")]
    public void test_unit_regression_github_683()
    {
        var path = "unit/regression/github_683.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_685.mzn")]
    public void test_unit_regression_github_685()
    {
        var path = "unit/regression/github_685.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_687.mzn")]
    public void test_unit_regression_github_687()
    {
        var path = "unit/regression/github_687.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_691.mzn")]
    public void test_unit_regression_github_691()
    {
        var path = "unit/regression/github_691.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_693_part1.mzn")]
    public void test_unit_regression_github_693_part1()
    {
        var path = "unit/regression/github_693_part1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_693_part2.mzn")]
    public void test_unit_regression_github_693_part2()
    {
        var path = "unit/regression/github_693_part2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_695.mzn")]
    public void test_unit_regression_github_695()
    {
        var path = "unit/regression/github_695.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_700.mzn")]
    public void test_unit_regression_github_700()
    {
        var path = "unit/regression/github_700.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_700_bad_sol.mzn")]
    public void test_unit_regression_github_700_bad_sol()
    {
        var path = "unit/regression/github_700_bad_sol.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_716.mzn")]
    public void test_unit_regression_github_716()
    {
        var path = "unit/regression/github_716.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_726.mzn")]
    public void test_unit_regression_github_726()
    {
        var path = "unit/regression/github_726.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_728.mzn")]
    public void test_unit_regression_github_728()
    {
        var path = "unit/regression/github_728.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_732.mzn")]
    public void test_unit_regression_github_732()
    {
        var path = "unit/regression/github_732.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_747.mzn")]
    public void test_unit_regression_github_747()
    {
        var path = "unit/regression/github_747.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_748.mzn")]
    public void test_unit_regression_github_748()
    {
        var path = "unit/regression/github_748.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_749.mzn")]
    public void test_unit_regression_github_749()
    {
        var path = "unit/regression/github_749.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_752.mzn")]
    public void test_unit_regression_github_752()
    {
        var path = "unit/regression/github_752.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_754.mzn")]
    public void test_unit_regression_github_754()
    {
        var path = "unit/regression/github_754.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_758.mzn")]
    public void test_unit_regression_github_758()
    {
        var path = "unit/regression/github_758.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_760.mzn")]
    public void test_unit_regression_github_760()
    {
        var path = "unit/regression/github_760.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_761.mzn")]
    public void test_unit_regression_github_761()
    {
        var path = "unit/regression/github_761.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_762.mzn")]
    public void test_unit_regression_github_762()
    {
        var path = "unit/regression/github_762.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_765.mzn")]
    public void test_unit_regression_github_765()
    {
        var path = "unit/regression/github_765.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_766.mzn")]
    public void test_unit_regression_github_766()
    {
        var path = "unit/regression/github_766.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_768a.mzn")]
    public void test_unit_regression_github_768a()
    {
        var path = "unit/regression/github_768a.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_768b.mzn")]
    public void test_unit_regression_github_768b()
    {
        var path = "unit/regression/github_768b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_771.mzn")]
    public void test_unit_regression_github_771()
    {
        var path = "unit/regression/github_771.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_773.mzn")]
    public void test_unit_regression_github_773()
    {
        var path = "unit/regression/github_773.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_776.mzn")]
    public void test_unit_regression_github_776()
    {
        var path = "unit/regression/github_776.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_778.mzn")]
    public void test_unit_regression_github_778()
    {
        var path = "unit/regression/github_778.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_783.mzn")]
    public void test_unit_regression_github_783()
    {
        var path = "unit/regression/github_783.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/github_785.mzn")]
    public void test_unit_regression_github_785()
    {
        var path = "unit/regression/github_785.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/hundred_doors_unoptimized.mzn")]
    public void test_unit_regression_hundred_doors_unoptimized()
    {
        var path = "unit/regression/hundred_doors_unoptimized.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/if_then_else_absent.mzn")]
    public void test_unit_regression_if_then_else_absent()
    {
        var path = "unit/regression/if_then_else_absent.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/int_times.mzn")]
    public void test_unit_regression_int_times()
    {
        var path = "unit/regression/int_times.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/in_array_eval_error.mzn")]
    public void test_unit_regression_in_array_eval_error()
    {
        var path = "unit/regression/in_array_eval_error.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/is_fixed.mzn")]
    public void test_unit_regression_is_fixed()
    {
        var path = "unit/regression/is_fixed.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/is_fixed_comp.mzn")]
    public void test_unit_regression_is_fixed_comp()
    {
        var path = "unit/regression/is_fixed_comp.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/lb_ub_dom_array_opt.mzn")]
    public void test_unit_regression_lb_ub_dom_array_opt()
    {
        var path = "unit/regression/lb_ub_dom_array_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/let_domain_from_generator.mzn")]
    public void test_unit_regression_let_domain_from_generator()
    {
        var path = "unit/regression/let_domain_from_generator.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/linear_bool_elem_bug.mzn")]
    public void test_unit_regression_linear_bool_elem_bug()
    {
        var path = "unit/regression/linear_bool_elem_bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/makepar_output.mzn")]
    public void test_unit_regression_makepar_output()
    {
        var path = "unit/regression/makepar_output.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/multi_goal_hierarchy_error.mzn")]
    public void test_unit_regression_multi_goal_hierarchy_error()
    {
        var path = "unit/regression/multi_goal_hierarchy_error.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/nested_clause.mzn")]
    public void test_unit_regression_nested_clause()
    {
        var path = "unit/regression/nested_clause.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/non_pos_pow.mzn")]
    public void test_unit_regression_non_pos_pow()
    {
        var path = "unit/regression/non_pos_pow.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/nosets_369.mzn")]
    public void test_unit_regression_nosets_369()
    {
        var path = "unit/regression/nosets_369.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/nosets_set_search.mzn")]
    public void test_unit_regression_nosets_set_search()
    {
        var path = "unit/regression/nosets_set_search.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/no_macro.mzn")]
    public void test_unit_regression_no_macro()
    {
        var path = "unit/regression/no_macro.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/opt_minmax.mzn")]
    public void test_unit_regression_opt_minmax()
    {
        var path = "unit/regression/opt_minmax.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/opt_noncontiguous_domain.mzn")]
    public void test_unit_regression_opt_noncontiguous_domain()
    {
        var path = "unit/regression/opt_noncontiguous_domain.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/opt_removed_items.mzn")]
    public void test_unit_regression_opt_removed_items()
    {
        var path = "unit/regression/opt_removed_items.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/output_2d_array_enum.mzn")]
    public void test_unit_regression_output_2d_array_enum()
    {
        var path = "unit/regression/output_2d_array_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/output_fn_toplevel_var.mzn")]
    public void test_unit_regression_output_fn_toplevel_var()
    {
        var path = "unit/regression/output_fn_toplevel_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/output_only_fn.mzn")]
    public void test_unit_regression_output_only_fn()
    {
        var path = "unit/regression/output_only_fn.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/overloading.mzn")]
    public void test_unit_regression_overloading()
    {
        var path = "unit/regression/overloading.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/parse_assignments.mzn")]
    public void test_unit_regression_parse_assignments()
    {
        var path = "unit/regression/parse_assignments.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/par_opt_dom.mzn")]
    public void test_unit_regression_par_opt_dom()
    {
        var path = "unit/regression/par_opt_dom.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/par_opt_equal.mzn")]
    public void test_unit_regression_par_opt_equal()
    {
        var path = "unit/regression/par_opt_equal.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/polymorphic_var_and_par.mzn")]
    public void test_unit_regression_polymorphic_var_and_par()
    {
        var path = "unit/regression/polymorphic_var_and_par.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/pow_undefined.mzn")]
    public void test_unit_regression_pow_undefined()
    {
        var path = "unit/regression/pow_undefined.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/pred_param_r7550.mzn")]
    public void test_unit_regression_pred_param_r7550()
    {
        var path = "unit/regression/pred_param_r7550.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/round.mzn")]
    public void test_unit_regression_round()
    {
        var path = "unit/regression/round.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/seq_search_bug.mzn")]
    public void test_unit_regression_seq_search_bug()
    {
        var path = "unit/regression/seq_search_bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/set_inequality_par.mzn")]
    public void test_unit_regression_set_inequality_par()
    {
        var path = "unit/regression/set_inequality_par.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/slice_enum_indexset.mzn")]
    public void test_unit_regression_slice_enum_indexset()
    {
        var path = "unit/regression/slice_enum_indexset.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/string-test-arg.mzn")]
    public void test_unit_regression_string_test_arg()
    {
        var path = "unit/regression/string-test-arg.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/subsets_100.mzn")]
    public void test_unit_regression_subsets_100()
    {
        var path = "unit/regression/subsets_100.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_annotation_on_exists.mzn")]
    public void test_unit_regression_test_annotation_on_exists()
    {
        var path = "unit/regression/test_annotation_on_exists.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bool2int.mzn")]
    public void test_unit_regression_test_bool2int()
    {
        var path = "unit/regression/test_bool2int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug218.mzn")]
    public void test_unit_regression_test_bug218()
    {
        var path = "unit/regression/test_bug218.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug359.mzn")]
    public void test_unit_regression_test_bug359()
    {
        var path = "unit/regression/test_bug359.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug45.mzn")]
    public void test_unit_regression_test_bug45()
    {
        var path = "unit/regression/test_bug45.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug53.mzn")]
    public void test_unit_regression_test_bug53()
    {
        var path = "unit/regression/test_bug53.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug54.mzn")]
    public void test_unit_regression_test_bug54()
    {
        var path = "unit/regression/test_bug54.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug55.mzn")]
    public void test_unit_regression_test_bug55()
    {
        var path = "unit/regression/test_bug55.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug57.mzn")]
    public void test_unit_regression_test_bug57()
    {
        var path = "unit/regression/test_bug57.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug65.mzn")]
    public void test_unit_regression_test_bug65()
    {
        var path = "unit/regression/test_bug65.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug66.mzn")]
    public void test_unit_regression_test_bug66()
    {
        var path = "unit/regression/test_bug66.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug70.mzn")]
    public void test_unit_regression_test_bug70()
    {
        var path = "unit/regression/test_bug70.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug71.mzn")]
    public void test_unit_regression_test_bug71()
    {
        var path = "unit/regression/test_bug71.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug72.mzn")]
    public void test_unit_regression_test_bug72()
    {
        var path = "unit/regression/test_bug72.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_129.mzn")]
    public void test_unit_regression_test_bug_129()
    {
        var path = "unit/regression/test_bug_129.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_476.mzn")]
    public void test_unit_regression_test_bug_476()
    {
        var path = "unit/regression/test_bug_476.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_483.mzn")]
    public void test_unit_regression_test_bug_483()
    {
        var path = "unit/regression/test_bug_483.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_493.mzn")]
    public void test_unit_regression_test_bug_493()
    {
        var path = "unit/regression/test_bug_493.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_494.mzn")]
    public void test_unit_regression_test_bug_494()
    {
        var path = "unit/regression/test_bug_494.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_520.mzn")]
    public void test_unit_regression_test_bug_520()
    {
        var path = "unit/regression/test_bug_520.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_521.mzn")]
    public void test_unit_regression_test_bug_521()
    {
        var path = "unit/regression/test_bug_521.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_527.mzn")]
    public void test_unit_regression_test_bug_527()
    {
        var path = "unit/regression/test_bug_527.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_529.mzn")]
    public void test_unit_regression_test_bug_529()
    {
        var path = "unit/regression/test_bug_529.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_588.mzn")]
    public void test_unit_regression_test_bug_588()
    {
        var path = "unit/regression/test_bug_588.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_637.mzn")]
    public void test_unit_regression_test_bug_637()
    {
        var path = "unit/regression/test_bug_637.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_array_sum_bounds.mzn")]
    public void test_unit_regression_test_bug_array_sum_bounds()
    {
        var path = "unit/regression/test_bug_array_sum_bounds.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_ite_array_eq.mzn")]
    public void test_unit_regression_test_bug_ite_array_eq()
    {
        var path = "unit/regression/test_bug_ite_array_eq.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_bug_pred_arg.mzn")]
    public void test_unit_regression_test_bug_pred_arg()
    {
        var path = "unit/regression/test_bug_pred_arg.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_equality_of_indirect_annotations.mzn")]
    public void test_unit_regression_test_equality_of_indirect_annotations()
    {
        var path = "unit/regression/test_equality_of_indirect_annotations.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_github_30.mzn")]
    public void test_unit_regression_test_github_30()
    {
        var path = "unit/regression/test_github_30.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_multioutput.mzn")]
    public void test_unit_regression_test_multioutput()
    {
        var path = "unit/regression/test_multioutput.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_not_in.mzn")]
    public void test_unit_regression_test_not_in()
    {
        var path = "unit/regression/test_not_in.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_output_array_of_set.mzn")]
    public void test_unit_regression_test_output_array_of_set()
    {
        var path = "unit/regression/test_output_array_of_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_output_string_var.mzn")]
    public void test_unit_regression_test_output_string_var()
    {
        var path = "unit/regression/test_output_string_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_parout.mzn")]
    public void test_unit_regression_test_parout()
    {
        var path = "unit/regression/test_parout.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_seq_precede_chain_set.mzn")]
    public void test_unit_regression_test_seq_precede_chain_set()
    {
        var path = "unit/regression/test_seq_precede_chain_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/test_slice_1d_array.mzn")]
    public void test_unit_regression_test_slice_1d_array()
    {
        var path = "unit/regression/test_slice_1d_array.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/ts_bug.mzn")]
    public void test_unit_regression_ts_bug()
    {
        var path = "unit/regression/ts_bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/type_specialise_array_return.mzn")]
    public void test_unit_regression_type_specialise_array_return()
    {
        var path = "unit/regression/type_specialise_array_return.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/var_bool_comp.mzn")]
    public void test_unit_regression_var_bool_comp()
    {
        var path = "unit/regression/var_bool_comp.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/var_opt_unconstrained.mzn")]
    public void test_unit_regression_var_opt_unconstrained()
    {
        var path = "unit/regression/var_opt_unconstrained.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/var_self_assign_bug.mzn")]
    public void test_unit_regression_var_self_assign_bug()
    {
        var path = "unit/regression/var_self_assign_bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/warm_start.mzn")]
    public void test_unit_regression_warm_start()
    {
        var path = "unit/regression/warm_start.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/where-forall-bug.mzn")]
    public void test_unit_regression_where_forall_bug()
    {
        var path = "unit/regression/where-forall-bug.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/regression/xor_mixed_context.mzn")]
    public void test_unit_regression_xor_mixed_context()
    {
        var path = "unit/regression/xor_mixed_context.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_choice_1.mzn")]
    public void test_unit_search_int_choice_1()
    {
        var path = "unit/search/int_choice_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_choice_2.mzn")]
    public void test_unit_search_int_choice_2()
    {
        var path = "unit/search/int_choice_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_choice_6.mzn")]
    public void test_unit_search_int_choice_6()
    {
        var path = "unit/search/int_choice_6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_var_select_1.mzn")]
    public void test_unit_search_int_var_select_1()
    {
        var path = "unit/search/int_var_select_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_var_select_2.mzn")]
    public void test_unit_search_int_var_select_2()
    {
        var path = "unit/search/int_var_select_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_var_select_3.mzn")]
    public void test_unit_search_int_var_select_3()
    {
        var path = "unit/search/int_var_select_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_var_select_4.mzn")]
    public void test_unit_search_int_var_select_4()
    {
        var path = "unit/search/int_var_select_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/int_var_select_6.mzn")]
    public void test_unit_search_int_var_select_6()
    {
        var path = "unit/search/int_var_select_6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/test-ff1.mzn")]
    public void test_unit_search_test_ff1()
    {
        var path = "unit/search/test-ff1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/test-ff2.mzn")]
    public void test_unit_search_test_ff2()
    {
        var path = "unit/search/test-ff2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/test-ff3.mzn")]
    public void test_unit_search_test_ff3()
    {
        var path = "unit/search/test-ff3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/test-large1.mzn")]
    public void test_unit_search_test_large1()
    {
        var path = "unit/search/test-large1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/test-med1.mzn")]
    public void test_unit_search_test_med1()
    {
        var path = "unit/search/test-med1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/search/test-small1.mzn")]
    public void test_unit_search_test_small1()
    {
        var path = "unit/search/test-small1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/alias.mzn")]
    public void test_unit_types_alias()
    {
        var path = "unit/types/alias.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/alias_call.mzn")]
    public void test_unit_types_alias_call()
    {
        var path = "unit/types/alias_call.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/alias_extern_dom.mzn")]
    public void test_unit_types_alias_extern_dom()
    {
        var path = "unit/types/alias_extern_dom.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/comprehension_type.mzn")]
    public void test_unit_types_comprehension_type()
    {
        var path = "unit/types/comprehension_type.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/cv_comprehension.mzn")]
    public void test_unit_types_cv_comprehension()
    {
        var path = "unit/types/cv_comprehension.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/enum_decl.mzn")]
    public void test_unit_types_enum_decl()
    {
        var path = "unit/types/enum_decl.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/enum_refl.mzn")]
    public void test_unit_types_enum_refl()
    {
        var path = "unit/types/enum_refl.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/github_647.mzn")]
    public void test_unit_types_github_647()
    {
        var path = "unit/types/github_647.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/non_contig_enum.mzn")]
    public void test_unit_types_non_contig_enum()
    {
        var path = "unit/types/non_contig_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/overload_inst_tuple_return.mzn")]
    public void test_unit_types_overload_inst_tuple_return()
    {
        var path = "unit/types/overload_inst_tuple_return.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/par_struct_tiid.mzn")]
    public void test_unit_types_par_struct_tiid()
    {
        var path = "unit/types/par_struct_tiid.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/polymorphic_overloading.mzn")]
    public void test_unit_types_polymorphic_overloading()
    {
        var path = "unit/types/polymorphic_overloading.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_access_success.mzn")]
    public void test_unit_types_record_access_success()
    {
        var path = "unit/types/record_access_success.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_binop_par.mzn")]
    public void test_unit_types_record_binop_par()
    {
        var path = "unit/types/record_binop_par.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_binop_var.mzn")]
    public void test_unit_types_record_binop_var()
    {
        var path = "unit/types/record_binop_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_comprehensions.mzn")]
    public void test_unit_types_record_comprehensions()
    {
        var path = "unit/types/record_comprehensions.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_nested.mzn")]
    public void test_unit_types_record_nested()
    {
        var path = "unit/types/record_nested.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_output.mzn")]
    public void test_unit_types_record_output()
    {
        var path = "unit/types/record_output.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_subtyping.mzn")]
    public void test_unit_types_record_subtyping()
    {
        var path = "unit/types/record_subtyping.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_var_element.mzn")]
    public void test_unit_types_record_var_element()
    {
        var path = "unit/types/record_var_element.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/record_var_ite.mzn")]
    public void test_unit_types_record_var_ite()
    {
        var path = "unit/types/record_var_ite.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/specialise_large_struct.mzn")]
    public void test_unit_types_specialise_large_struct()
    {
        var path = "unit/types/specialise_large_struct.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_array_coercion.mzn")]
    public void test_unit_types_struct_array_coercion()
    {
        var path = "unit/types/struct_array_coercion.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_bind_1.mzn")]
    public void test_unit_types_struct_bind_1()
    {
        var path = "unit/types/struct_bind_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_bind_2.mzn")]
    public void test_unit_types_struct_bind_2()
    {
        var path = "unit/types/struct_bind_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_domain_1.mzn")]
    public void test_unit_types_struct_domain_1()
    {
        var path = "unit/types/struct_domain_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_domain_2.mzn")]
    public void test_unit_types_struct_domain_2()
    {
        var path = "unit/types/struct_domain_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_domain_3.mzn")]
    public void test_unit_types_struct_domain_3()
    {
        var path = "unit/types/struct_domain_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_domain_4.mzn")]
    public void test_unit_types_struct_domain_4()
    {
        var path = "unit/types/struct_domain_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_domain_5.mzn")]
    public void test_unit_types_struct_domain_5()
    {
        var path = "unit/types/struct_domain_5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_domain_6.mzn")]
    public void test_unit_types_struct_domain_6()
    {
        var path = "unit/types/struct_domain_6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_par_function_version.mzn")]
    public void test_unit_types_struct_par_function_version()
    {
        var path = "unit/types/struct_par_function_version.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_return_ti_1.mzn")]
    public void test_unit_types_struct_return_ti_1()
    {
        var path = "unit/types/struct_return_ti_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_return_ti_2.mzn")]
    public void test_unit_types_struct_return_ti_2()
    {
        var path = "unit/types/struct_return_ti_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_return_ti_4.mzn")]
    public void test_unit_types_struct_return_ti_4()
    {
        var path = "unit/types/struct_return_ti_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_specialise.mzn")]
    public void test_unit_types_struct_specialise()
    {
        var path = "unit/types/struct_specialise.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/struct_specialise_return.mzn")]
    public void test_unit_types_struct_specialise_return()
    {
        var path = "unit/types/struct_specialise_return.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/test_any_enum_typeinstid.mzn")]
    public void test_unit_types_test_any_enum_typeinstid()
    {
        var path = "unit/types/test_any_enum_typeinstid.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_access_success.mzn")]
    public void test_unit_types_tuple_access_success()
    {
        var path = "unit/types/tuple_access_success.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_binop_par.mzn")]
    public void test_unit_types_tuple_binop_par()
    {
        var path = "unit/types/tuple_binop_par.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_binop_var.mzn")]
    public void test_unit_types_tuple_binop_var()
    {
        var path = "unit/types/tuple_binop_var.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_comprehensions.mzn")]
    public void test_unit_types_tuple_comprehensions()
    {
        var path = "unit/types/tuple_comprehensions.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_int_set_of_int_specialisation.mzn")]
    public void test_unit_types_tuple_int_set_of_int_specialisation()
    {
        var path = "unit/types/tuple_int_set_of_int_specialisation.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_lit.mzn")]
    public void test_unit_types_tuple_lit()
    {
        var path = "unit/types/tuple_lit.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_mkpar.mzn")]
    public void test_unit_types_tuple_mkpar()
    {
        var path = "unit/types/tuple_mkpar.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_output.mzn")]
    public void test_unit_types_tuple_output()
    {
        var path = "unit/types/tuple_output.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_subtyping.mzn")]
    public void test_unit_types_tuple_subtyping()
    {
        var path = "unit/types/tuple_subtyping.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_var_element.mzn")]
    public void test_unit_types_tuple_var_element()
    {
        var path = "unit/types/tuple_var_element.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/types/tuple_var_ite.mzn")]
    public void test_unit_types_tuple_var_ite()
    {
        var path = "unit/types/tuple_var_ite.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/alldifferent/globals_alldiff_set_nosets.mzn")]
    public void test_unit_globals_alldifferent_globals_alldiff_set_nosets()
    {
        var path = "unit/globals/alldifferent/globals_alldiff_set_nosets.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/alldifferent/globals_all_different_int.mzn")]
    public void test_unit_globals_alldifferent_globals_all_different_int()
    {
        var path = "unit/globals/alldifferent/globals_all_different_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/alldifferent/globals_all_different_int_opt.mzn")]
    public void test_unit_globals_alldifferent_globals_all_different_int_opt()
    {
        var path = "unit/globals/alldifferent/globals_all_different_int_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/alldifferent/globals_all_different_set.mzn")]
    public void test_unit_globals_alldifferent_globals_all_different_set()
    {
        var path = "unit/globals/alldifferent/globals_all_different_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/alldifferent_except_0/test_alldiff_except0.mzn")]
    public void test_unit_globals_alldifferent_except_0_test_alldiff_except0()
    {
        var path = "unit/globals/alldifferent_except_0/test_alldiff_except0.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/alldifferent_except_0/test_alldiff_except0b.mzn")]
    public void test_unit_globals_alldifferent_except_0_test_alldiff_except0b()
    {
        var path = "unit/globals/alldifferent_except_0/test_alldiff_except0b.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/all_disjoint/globals_all_disjoint.mzn")]
    public void test_unit_globals_all_disjoint_globals_all_disjoint()
    {
        var path = "unit/globals/all_disjoint/globals_all_disjoint.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/all_equal/globals_all_equal_int.mzn")]
    public void test_unit_globals_all_equal_globals_all_equal_int()
    {
        var path = "unit/globals/all_equal/globals_all_equal_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/all_equal/globals_all_equal_set.mzn")]
    public void test_unit_globals_all_equal_globals_all_equal_set()
    {
        var path = "unit/globals/all_equal/globals_all_equal_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/among/globals_among.mzn")]
    public void test_unit_globals_among_globals_among()
    {
        var path = "unit/globals/among/globals_among.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_max/globals_arg_max.mzn")]
    public void test_unit_globals_arg_max_globals_arg_max()
    {
        var path = "unit/globals/arg_max/globals_arg_max.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_max/globals_arg_max_opt.mzn")]
    public void test_unit_globals_arg_max_globals_arg_max_opt()
    {
        var path = "unit/globals/arg_max/globals_arg_max_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_max/globals_arg_max_opt_weak.mzn")]
    public void test_unit_globals_arg_max_globals_arg_max_opt_weak()
    {
        var path = "unit/globals/arg_max/globals_arg_max_opt_weak.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_min/globals_arg_max_opt.mzn")]
    public void test_unit_globals_arg_min_globals_arg_max_opt()
    {
        var path = "unit/globals/arg_min/globals_arg_max_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_min/globals_arg_min.mzn")]
    public void test_unit_globals_arg_min_globals_arg_min()
    {
        var path = "unit/globals/arg_min/globals_arg_min.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_min/globals_arg_min_opt_weak.mzn")]
    public void test_unit_globals_arg_min_globals_arg_min_opt_weak()
    {
        var path = "unit/globals/arg_min/globals_arg_min_opt_weak.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/arg_val/arg_val_enum.mzn")]
    public void test_unit_globals_arg_val_arg_val_enum()
    {
        var path = "unit/globals/arg_val/arg_val_enum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/atleast/globals_at_least.mzn")]
    public void test_unit_globals_atleast_globals_at_least()
    {
        var path = "unit/globals/atleast/globals_at_least.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/atmost1/globals_at_most1.mzn")]
    public void test_unit_globals_atmost1_globals_at_most1()
    {
        var path = "unit/globals/atmost1/globals_at_most1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/bin_packing/globals_bin_packing.mzn")]
    public void test_unit_globals_bin_packing_globals_bin_packing()
    {
        var path = "unit/globals/bin_packing/globals_bin_packing.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/bin_packing_capa/globals_bin_packing_capa.mzn")]
    public void test_unit_globals_bin_packing_capa_globals_bin_packing_capa()
    {
        var path = "unit/globals/bin_packing_capa/globals_bin_packing_capa.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/circuit/test_circuit.mzn")]
    public void test_unit_globals_circuit_test_circuit()
    {
        var path = "unit/globals/circuit/test_circuit.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/count/globals_count.mzn")]
    public void test_unit_globals_count_globals_count()
    {
        var path = "unit/globals/count/globals_count.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/cumulative/github_589.mzn")]
    public void test_unit_globals_cumulative_github_589()
    {
        var path = "unit/globals/cumulative/github_589.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/cumulative/globals_cumulative.mzn")]
    public void test_unit_globals_cumulative_globals_cumulative()
    {
        var path = "unit/globals/cumulative/globals_cumulative.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/cumulative/unsat_resource_cumulative.mzn")]
    public void test_unit_globals_cumulative_unsat_resource_cumulative()
    {
        var path = "unit/globals/cumulative/unsat_resource_cumulative.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/cumulatives/globals_cumulatives.mzn")]
    public void test_unit_globals_cumulatives_globals_cumulatives()
    {
        var path = "unit/globals/cumulatives/globals_cumulatives.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/decreasing/globals_decreasing.mzn")]
    public void test_unit_globals_decreasing_globals_decreasing()
    {
        var path = "unit/globals/decreasing/globals_decreasing.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/disjoint/globals_disjoint.mzn")]
    public void test_unit_globals_disjoint_globals_disjoint()
    {
        var path = "unit/globals/disjoint/globals_disjoint.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/distribute/globals_distribute.mzn")]
    public void test_unit_globals_distribute_globals_distribute()
    {
        var path = "unit/globals/distribute/globals_distribute.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/global_cardinality/globals_global_cardinality.mzn")]
    public void test_unit_globals_global_cardinality_globals_global_cardinality()
    {
        var path = "unit/globals/global_cardinality/globals_global_cardinality.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/global_cardinality/globals_global_cardinality_low_up.mzn")]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_low_up()
    {
        var path = "unit/globals/global_cardinality/globals_global_cardinality_low_up.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality/globals_global_cardinality_low_up_opt.mzn"
    )]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_low_up_opt()
    {
        var path = "unit/globals/global_cardinality/globals_global_cardinality_low_up_opt.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality/globals_global_cardinality_low_up_set.mzn"
    )]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_low_up_set()
    {
        var path = "unit/globals/global_cardinality/globals_global_cardinality_low_up_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/global_cardinality/globals_global_cardinality_opt.mzn")]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_opt()
    {
        var path = "unit/globals/global_cardinality/globals_global_cardinality_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/global_cardinality/globals_global_cardinality_set.mzn")]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_set()
    {
        var path = "unit/globals/global_cardinality/globals_global_cardinality_set.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality_closed/globals_global_cardinality_closed.mzn"
    )]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed()
    {
        var path = "unit/globals/global_cardinality_closed/globals_global_cardinality_closed.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality_closed/globals_global_cardinality_closed_opt.mzn"
    )]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed_opt()
    {
        var path =
            "unit/globals/global_cardinality_closed/globals_global_cardinality_closed_opt.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality_closed/globals_global_cardinality_closed_set.mzn"
    )]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed_set()
    {
        var path =
            "unit/globals/global_cardinality_closed/globals_global_cardinality_closed_set.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed.mzn"
    )]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed()
    {
        var path =
            "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_opt.mzn"
    )]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed_opt()
    {
        var path =
            "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_opt.mzn";
        Test(path);
    }

    [Fact(
        DisplayName = "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_set.mzn"
    )]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed_set()
    {
        var path =
            "unit/globals/global_cardinality_closed/globals_global_cardinality_low_up_closed_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/increasing/globals_increasing.mzn")]
    public void test_unit_globals_increasing_globals_increasing()
    {
        var path = "unit/globals/increasing/globals_increasing.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/increasing/globals_strictly_increasing_opt.mzn")]
    public void test_unit_globals_increasing_globals_strictly_increasing_opt()
    {
        var path = "unit/globals/increasing/globals_strictly_increasing_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/int_set_channel/globals_int_set_channel.mzn")]
    public void test_unit_globals_int_set_channel_globals_int_set_channel()
    {
        var path = "unit/globals/int_set_channel/globals_int_set_channel.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/int_set_channel/test_int_set_channel2.mzn")]
    public void test_unit_globals_int_set_channel_test_int_set_channel2()
    {
        var path = "unit/globals/int_set_channel/test_int_set_channel2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/inverse/globals_inverse.mzn")]
    public void test_unit_globals_inverse_globals_inverse()
    {
        var path = "unit/globals/inverse/globals_inverse.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/inverse/inverse_opt.mzn")]
    public void test_unit_globals_inverse_inverse_opt()
    {
        var path = "unit/globals/inverse/inverse_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/inverse_in_range/globals_inverse_in_range.mzn")]
    public void test_unit_globals_inverse_in_range_globals_inverse_in_range()
    {
        var path = "unit/globals/inverse_in_range/globals_inverse_in_range.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/inverse_set/globals_inverse_set.mzn")]
    public void test_unit_globals_inverse_set_globals_inverse_set()
    {
        var path = "unit/globals/inverse_set/globals_inverse_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex2/globals_lex2.mzn")]
    public void test_unit_globals_lex2_globals_lex2()
    {
        var path = "unit/globals/lex2/globals_lex2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_chain/globals_lex_chain.mzn")]
    public void test_unit_globals_lex_chain_globals_lex_chain()
    {
        var path = "unit/globals/lex_chain/globals_lex_chain.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_chain/globals_lex_chain__orbitope.mzn")]
    public void test_unit_globals_lex_chain_globals_lex_chain__orbitope()
    {
        var path = "unit/globals/lex_chain/globals_lex_chain__orbitope.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_greater/globals_lex_greater.mzn")]
    public void test_unit_globals_lex_greater_globals_lex_greater()
    {
        var path = "unit/globals/lex_greater/globals_lex_greater.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_greatereq/globals_lex_greatereq.mzn")]
    public void test_unit_globals_lex_greatereq_globals_lex_greatereq()
    {
        var path = "unit/globals/lex_greatereq/globals_lex_greatereq.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_less/globals_lex_less.mzn")]
    public void test_unit_globals_lex_less_globals_lex_less()
    {
        var path = "unit/globals/lex_less/globals_lex_less.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_less/test_bool_lex_less.mzn")]
    public void test_unit_globals_lex_less_test_bool_lex_less()
    {
        var path = "unit/globals/lex_less/test_bool_lex_less.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_lesseq/globals_lex_lesseq.mzn")]
    public void test_unit_globals_lex_lesseq_globals_lex_lesseq()
    {
        var path = "unit/globals/lex_lesseq/globals_lex_lesseq.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/lex_lesseq/test_bool_lex_lesseq.mzn")]
    public void test_unit_globals_lex_lesseq_test_bool_lex_lesseq()
    {
        var path = "unit/globals/lex_lesseq/test_bool_lex_lesseq.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/link_set_to_booleans/globals_link_set_to_booleans.mzn")]
    public void test_unit_globals_link_set_to_booleans_globals_link_set_to_booleans()
    {
        var path = "unit/globals/link_set_to_booleans/globals_link_set_to_booleans.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/maximum/globals_maximum_int.mzn")]
    public void test_unit_globals_maximum_globals_maximum_int()
    {
        var path = "unit/globals/maximum/globals_maximum_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/minimum/globals_minimum_int.mzn")]
    public void test_unit_globals_minimum_globals_minimum_int()
    {
        var path = "unit/globals/minimum/globals_minimum_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/nvalue/globals_nvalue.mzn")]
    public void test_unit_globals_nvalue_globals_nvalue()
    {
        var path = "unit/globals/nvalue/globals_nvalue.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/nvalue/nvalue_total.mzn")]
    public void test_unit_globals_nvalue_nvalue_total()
    {
        var path = "unit/globals/nvalue/nvalue_total.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/partition_set/globals_partition_set.mzn")]
    public void test_unit_globals_partition_set_globals_partition_set()
    {
        var path = "unit/globals/partition_set/globals_partition_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/range/globals_range.mzn")]
    public void test_unit_globals_range_globals_range()
    {
        var path = "unit/globals/range/globals_range.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular.mzn")]
    public void test_unit_globals_regular_globals_regular()
    {
        var path = "unit/globals/regular/globals_regular.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular_regex_1.mzn")]
    public void test_unit_globals_regular_globals_regular_regex_1()
    {
        var path = "unit/globals/regular/globals_regular_regex_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular_regex_2.mzn")]
    public void test_unit_globals_regular_globals_regular_regex_2()
    {
        var path = "unit/globals/regular/globals_regular_regex_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular_regex_3.mzn")]
    public void test_unit_globals_regular_globals_regular_regex_3()
    {
        var path = "unit/globals/regular/globals_regular_regex_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular_regex_4.mzn")]
    public void test_unit_globals_regular_globals_regular_regex_4()
    {
        var path = "unit/globals/regular/globals_regular_regex_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular_regex_5.mzn")]
    public void test_unit_globals_regular_globals_regular_regex_5()
    {
        var path = "unit/globals/regular/globals_regular_regex_5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/regular/globals_regular_regex_6.mzn")]
    public void test_unit_globals_regular_globals_regular_regex_6()
    {
        var path = "unit/globals/regular/globals_regular_regex_6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/roots/test_roots.mzn")]
    public void test_unit_globals_roots_test_roots()
    {
        var path = "unit/globals/roots/test_roots.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/roots/test_roots2.mzn")]
    public void test_unit_globals_roots_test_roots2()
    {
        var path = "unit/globals/roots/test_roots2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/roots/test_roots3.mzn")]
    public void test_unit_globals_roots_test_roots3()
    {
        var path = "unit/globals/roots/test_roots3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/sliding_sum/globals_sliding_sum.mzn")]
    public void test_unit_globals_sliding_sum_globals_sliding_sum()
    {
        var path = "unit/globals/sliding_sum/globals_sliding_sum.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/sort/globals_sort.mzn")]
    public void test_unit_globals_sort_globals_sort()
    {
        var path = "unit/globals/sort/globals_sort.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/strict_lex2/globals_strict_lex2.mzn")]
    public void test_unit_globals_strict_lex2_globals_strict_lex2()
    {
        var path = "unit/globals/strict_lex2/globals_strict_lex2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/subcircuit/test_subcircuit.mzn")]
    public void test_unit_globals_subcircuit_test_subcircuit()
    {
        var path = "unit/globals/subcircuit/test_subcircuit.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/sum_pred/globals_sum_pred.mzn")]
    public void test_unit_globals_sum_pred_globals_sum_pred()
    {
        var path = "unit/globals/sum_pred/globals_sum_pred.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/table/globals_table.mzn")]
    public void test_unit_globals_table_globals_table()
    {
        var path = "unit/globals/table/globals_table.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/table/globals_table_opt.mzn")]
    public void test_unit_globals_table_globals_table_opt()
    {
        var path = "unit/globals/table/globals_table_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/value_precede/globals_value_precede_int.mzn")]
    public void test_unit_globals_value_precede_globals_value_precede_int()
    {
        var path = "unit/globals/value_precede/globals_value_precede_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/value_precede/globals_value_precede_int_opt.mzn")]
    public void test_unit_globals_value_precede_globals_value_precede_int_opt()
    {
        var path = "unit/globals/value_precede/globals_value_precede_int_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/value_precede/globals_value_precede_set.mzn")]
    public void test_unit_globals_value_precede_globals_value_precede_set()
    {
        var path = "unit/globals/value_precede/globals_value_precede_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/value_precede_chain/globals_value_precede_chain_int.mzn")]
    public void test_unit_globals_value_precede_chain_globals_value_precede_chain_int()
    {
        var path = "unit/globals/value_precede_chain/globals_value_precede_chain_int.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/value_precede_chain/globals_value_precede_chain_int_opt.mzn")]
    public void test_unit_globals_value_precede_chain_globals_value_precede_chain_int_opt()
    {
        var path = "unit/globals/value_precede_chain/globals_value_precede_chain_int_opt.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/value_precede_chain/globals_value_precede_chain_set.mzn")]
    public void test_unit_globals_value_precede_chain_globals_value_precede_chain_set()
    {
        var path = "unit/globals/value_precede_chain/globals_value_precede_chain_set.mzn";
        Test(path);
    }

    [Fact(DisplayName = "unit/globals/var_sqr_sym/globals_var_sqr_sym.mzn")]
    public void test_unit_globals_var_sqr_sym_globals_var_sqr_sym()
    {
        var path = "unit/globals/var_sqr_sym/globals_var_sqr_sym.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/2DPacking.mzn")]
    public void test_examples_2DPacking()
    {
        var path = "examples/2DPacking.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/alpha.mzn")]
    public void test_examples_alpha()
    {
        var path = "examples/alpha.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships10.mzn")]
    public void test_examples_battleships10()
    {
        var path = "examples/battleships10.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_1.mzn")]
    public void test_examples_battleships_1()
    {
        var path = "examples/battleships_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_2.mzn")]
    public void test_examples_battleships_2()
    {
        var path = "examples/battleships_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_3.mzn")]
    public void test_examples_battleships_3()
    {
        var path = "examples/battleships_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_4.mzn")]
    public void test_examples_battleships_4()
    {
        var path = "examples/battleships_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_5.mzn")]
    public void test_examples_battleships_5()
    {
        var path = "examples/battleships_5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_7.mzn")]
    public void test_examples_battleships_7()
    {
        var path = "examples/battleships_7.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/battleships_9.mzn")]
    public void test_examples_battleships_9()
    {
        var path = "examples/battleships_9.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/blocksworld_instance_1.mzn")]
    public void test_examples_blocksworld_instance_1()
    {
        var path = "examples/blocksworld_instance_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/blocksworld_instance_2.mzn")]
    public void test_examples_blocksworld_instance_2()
    {
        var path = "examples/blocksworld_instance_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/cutstock.mzn")]
    public void test_examples_cutstock()
    {
        var path = "examples/cutstock.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/eq20.mzn")]
    public void test_examples_eq20()
    {
        var path = "examples/eq20.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/factory_planning_instance.mzn")]
    public void test_examples_factory_planning_instance()
    {
        var path = "examples/factory_planning_instance.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/golomb.mzn")]
    public void test_examples_golomb()
    {
        var path = "examples/golomb.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/halfreif.mzn")]
    public void test_examples_halfreif()
    {
        var path = "examples/halfreif.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/jobshop2x2.mzn")]
    public void test_examples_jobshop2x2()
    {
        var path = "examples/jobshop2x2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/knights.mzn")]
    public void test_examples_knights()
    {
        var path = "examples/knights.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/langford.mzn")]
    public void test_examples_langford()
    {
        var path = "examples/langford.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/langford2.mzn")]
    public void test_examples_langford2()
    {
        var path = "examples/langford2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/latin_squares_fd.mzn")]
    public void test_examples_latin_squares_fd()
    {
        var path = "examples/latin_squares_fd.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/magicsq_3.mzn")]
    public void test_examples_magicsq_3()
    {
        var path = "examples/magicsq_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/magicsq_4.mzn")]
    public void test_examples_magicsq_4()
    {
        var path = "examples/magicsq_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/magicsq_5.mzn")]
    public void test_examples_magicsq_5()
    {
        var path = "examples/magicsq_5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/multidimknapsack_simple.mzn")]
    public void test_examples_multidimknapsack_simple()
    {
        var path = "examples/multidimknapsack_simple.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/oss.mzn")]
    public void test_examples_oss()
    {
        var path = "examples/oss.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/packing.mzn")]
    public void test_examples_packing()
    {
        var path = "examples/packing.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/perfsq.mzn")]
    public void test_examples_perfsq()
    {
        var path = "examples/perfsq.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/perfsq2.mzn")]
    public void test_examples_perfsq2()
    {
        var path = "examples/perfsq2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/photo.mzn")]
    public void test_examples_photo()
    {
        var path = "examples/photo.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/product_fd.mzn")]
    public void test_examples_product_fd()
    {
        var path = "examples/product_fd.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/product_lp.mzn")]
    public void test_examples_product_lp()
    {
        var path = "examples/product_lp.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/quasigroup_qg5.mzn")]
    public void test_examples_quasigroup_qg5()
    {
        var path = "examples/quasigroup_qg5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/queen_cp2.mzn")]
    public void test_examples_queen_cp2()
    {
        var path = "examples/queen_cp2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/queen_ip.mzn")]
    public void test_examples_queen_ip()
    {
        var path = "examples/queen_ip.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/radiation.mzn")]
    public void test_examples_radiation()
    {
        var path = "examples/radiation.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/simple_sat.mzn")]
    public void test_examples_simple_sat()
    {
        var path = "examples/simple_sat.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/singHoist2.mzn")]
    public void test_examples_singHoist2()
    {
        var path = "examples/singHoist2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/steiner-triples.mzn")]
    public void test_examples_steiner_triples()
    {
        var path = "examples/steiner-triples.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/sudoku.mzn")]
    public void test_examples_sudoku()
    {
        var path = "examples/sudoku.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/template_design.mzn")]
    public void test_examples_template_design()
    {
        var path = "examples/template_design.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/tenpenki_1.mzn")]
    public void test_examples_tenpenki_1()
    {
        var path = "examples/tenpenki_1.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/tenpenki_2.mzn")]
    public void test_examples_tenpenki_2()
    {
        var path = "examples/tenpenki_2.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/tenpenki_3.mzn")]
    public void test_examples_tenpenki_3()
    {
        var path = "examples/tenpenki_3.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/tenpenki_4.mzn")]
    public void test_examples_tenpenki_4()
    {
        var path = "examples/tenpenki_4.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/tenpenki_5.mzn")]
    public void test_examples_tenpenki_5()
    {
        var path = "examples/tenpenki_5.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/tenpenki_6.mzn")]
    public void test_examples_tenpenki_6()
    {
        var path = "examples/tenpenki_6.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/timetabling.mzn")]
    public void test_examples_timetabling()
    {
        var path = "examples/timetabling.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/trucking.mzn")]
    public void test_examples_trucking()
    {
        var path = "examples/trucking.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/warehouses.mzn")]
    public void test_examples_warehouses()
    {
        var path = "examples/warehouses.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/wolf_goat_cabbage.mzn")]
    public void test_examples_wolf_goat_cabbage()
    {
        var path = "examples/wolf_goat_cabbage.mzn";
        Test(path);
    }

    [Fact(DisplayName = "examples/zebra.mzn")]
    public void test_examples_zebra()
    {
        var path = "examples/zebra.mzn";
        Test(path);
    }
}
