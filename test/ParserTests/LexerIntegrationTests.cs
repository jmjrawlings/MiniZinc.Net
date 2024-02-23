public sealed class LexerTests
{
    [Fact]
    public void test_unit_test_globals_float()
    {
        var path = @"unit\test-globals-float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_aggregation()
    {
        var path = @"unit\compilation\aggregation.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_annotate_bool_literal()
    {
        var path = @"unit\compilation\annotate_bool_literal.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_annotate_from_array()
    {
        var path = @"unit\compilation\annotate_from_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_assert_dbg_flag()
    {
        var path = @"unit\compilation\assert_dbg_flag.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_assert_dbg_ignore()
    {
        var path = @"unit\compilation\assert_dbg_ignore.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_call_root_ctx()
    {
        var path = @"unit\compilation\call_root_ctx.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_chain_compr_mult_clause()
    {
        var path = @"unit\compilation\chain_compr_mult_clause.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_clause_disable_hr()
    {
        var path = @"unit\compilation\clause_disable_hr.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_commutative_cse()
    {
        var path = @"unit\compilation\commutative_cse.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_count_rewrite()
    {
        var path = @"unit\compilation\count_rewrite.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_cv_domain()
    {
        var path = @"unit\compilation\cv_domain.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_debug_mode_false()
    {
        var path = @"unit\compilation\debug_mode_false.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_debug_mode_true()
    {
        var path = @"unit\compilation\debug_mode_true.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_defines_var_cycle_breaking()
    {
        var path = @"unit\compilation\defines_var_cycle_breaking.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_float_inf_range_dom()
    {
        var path = @"unit\compilation\float_inf_range_dom.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_has_ann()
    {
        var path = @"unit\compilation\has_ann.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_if_then_no_else()
    {
        var path = @"unit\compilation\if_then_no_else.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_implied_exists_chain()
    {
        var path = @"unit\compilation\implied_exists_chain.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_implied_hr()
    {
        var path = @"unit\compilation\implied_hr.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_int_inf_dom()
    {
        var path = @"unit\compilation\int_inf_dom.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_multiple_neg()
    {
        var path = @"unit\compilation\multiple_neg.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_optimization()
    {
        var path = @"unit\compilation\optimization.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_par_arg_out_of_bounds()
    {
        var path = @"unit\compilation\par_arg_out_of_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_poly_overload()
    {
        var path = @"unit\compilation\poly_overload.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_quoted_id_flatzinc()
    {
        var path = @"unit\compilation\quoted_id_flatzinc.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_set2iter()
    {
        var path = @"unit\compilation\set2iter.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_compilation_time_limit()
    {
        var path = @"unit\compilation\time_limit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div1()
    {
        var path = @"unit\division\test_div1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div10()
    {
        var path = @"unit\division\test_div10.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div11()
    {
        var path = @"unit\division\test_div11.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div12()
    {
        var path = @"unit\division\test_div12.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div2()
    {
        var path = @"unit\division\test_div2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div3()
    {
        var path = @"unit\division\test_div3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div4()
    {
        var path = @"unit\division\test_div4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div5()
    {
        var path = @"unit\division\test_div5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div6()
    {
        var path = @"unit\division\test_div6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div7()
    {
        var path = @"unit\division\test_div7.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div8()
    {
        var path = @"unit\division\test_div8.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div9()
    {
        var path = @"unit\division\test_div9.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_div_mod_bounds()
    {
        var path = @"unit\division\test_div_mod_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_fldiv_01()
    {
        var path = @"unit\division\test_fldiv_01.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_division_test_fldiv_02()
    {
        var path = @"unit\division\test_fldiv_02.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_fdlp_test_lp_solve_satisfy()
    {
        var path = @"unit\fdlp\test_lp_solve_satisfy.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_anon_var_flatten()
    {
        var path = @"unit\general\anon_var_flatten.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_access_out_of_bounds_1()
    {
        var path = @"unit\general\array_access_out_of_bounds_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_access_out_of_bounds_2()
    {
        var path = @"unit\general\array_access_out_of_bounds_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_access_record_out_of_bounds()
    {
        var path = @"unit\general\array_access_record_out_of_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_access_tuple_out_of_bounds()
    {
        var path = @"unit\general\array_access_tuple_out_of_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_intersect_context()
    {
        var path = @"unit\general\array_intersect_context.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_param_non_array_return()
    {
        var path = @"unit\general\array_param_non_array_return.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_string_gen()
    {
        var path = @"unit\general\array_string_gen.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_array_union_intersect_enum()
    {
        var path = @"unit\general\array_union_intersect_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_assert_bad_1()
    {
        var path = @"unit\general\assert_bad_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_assert_bad_2()
    {
        var path = @"unit\general\assert_bad_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_assert_good()
    {
        var path = @"unit\general\assert_good.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_auto_par_function()
    {
        var path = @"unit\general\auto_par_function.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_bin_pack_multiobj()
    {
        var path = @"unit\general\bin_pack_multiobj.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_builtins_arg_max()
    {
        var path = @"unit\general\builtins_arg_max.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_builtins_debug()
    {
        var path = @"unit\general\builtins_debug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_comprehension_var_ub()
    {
        var path = @"unit\general\comprehension_var_ub.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_cse_ctx()
    {
        var path = @"unit\general\cse_ctx.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_enum_constructor_quoting()
    {
        var path = @"unit\general\enum_constructor_quoting.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_enum_order()
    {
        var path = @"unit\general\enum_order.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_enum_out_of_range_1()
    {
        var path = @"unit\general\enum_out_of_range_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_enum_out_of_range_2()
    {
        var path = @"unit\general\enum_out_of_range_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_fix_struct()
    {
        var path = @"unit\general\fix_struct.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_function_param_out_of_range()
    {
        var path = @"unit\general\function_param_out_of_range.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_function_return_out_of_range()
    {
        var path = @"unit\general\function_return_out_of_range.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_function_return_out_of_range_opt()
    {
        var path = @"unit\general\function_return_out_of_range_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_iffall_bv()
    {
        var path = @"unit\general\iffall_bv.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_infinite_domain_bind()
    {
        var path = @"unit\general\infinite_domain_bind.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_json_ignore()
    {
        var path = @"unit\general\json_ignore.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_let_struct_domain()
    {
        var path = @"unit\general\let_struct_domain.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_exists()
    {
        var path = @"unit\general\md_exists.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_forall()
    {
        var path = @"unit\general\md_forall.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_iffall()
    {
        var path = @"unit\general\md_iffall.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_product_int()
    {
        var path = @"unit\general\md_product_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_sum_float()
    {
        var path = @"unit\general\md_sum_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_sum_int()
    {
        var path = @"unit\general\md_sum_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_md_xorall()
    {
        var path = @"unit\general\md_xorall.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_missing_ozn_decl()
    {
        var path = @"unit\general\missing_ozn_decl.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mortgage()
    {
        var path = @"unit\general\mortgage.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit1()
    {
        var path = @"unit\general\mzn-implicit1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit2()
    {
        var path = @"unit\general\mzn-implicit2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit3()
    {
        var path = @"unit\general\mzn-implicit3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit4()
    {
        var path = @"unit\general\mzn-implicit4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit5()
    {
        var path = @"unit\general\mzn-implicit5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit6()
    {
        var path = @"unit\general\mzn-implicit6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit7()
    {
        var path = @"unit\general\mzn-implicit7.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_implicit8()
    {
        var path = @"unit\general\mzn-implicit8.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_div()
    {
        var path = @"unit\general\mzn_div.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_mzn_mod()
    {
        var path = @"unit\general\mzn_mod.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_overload_bottom()
    {
        var path = @"unit\general\overload_bottom.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_param_out_of_range_float()
    {
        var path = @"unit\general\param_out_of_range_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_param_out_of_range_int()
    {
        var path = @"unit\general\param_out_of_range_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_pow_1()
    {
        var path = @"unit\general\pow_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_pow_2()
    {
        var path = @"unit\general\pow_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_pow_3()
    {
        var path = @"unit\general\pow_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_pow_4()
    {
        var path = @"unit\general\pow_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_pow_bounds()
    {
        var path = @"unit\general\pow_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_quoted_id_1()
    {
        var path = @"unit\general\quoted_id_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_quoted_id_2()
    {
        var path = @"unit\general\quoted_id_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_quoted_id_3()
    {
        var path = @"unit\general\quoted_id_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_quoted_id_4()
    {
        var path = @"unit\general\quoted_id_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_range_var_enum()
    {
        var path = @"unit\general\range_var_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_stack_overflow()
    {
        var path = @"unit\general\stack_overflow.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_search1()
    {
        var path = @"unit\general\test-search1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array1()
    {
        var path = @"unit\general\test_array1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array1and2d()
    {
        var path = @"unit\general\test_array1and2d.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array1d_bad_1()
    {
        var path = @"unit\general\test_array1d_bad_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array1d_bad_2()
    {
        var path = @"unit\general\test_array1d_bad_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array1d_bad_3()
    {
        var path = @"unit\general\test_array1d_bad_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array1d_bad_4()
    {
        var path = @"unit\general\test_array1d_bad_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array2()
    {
        var path = @"unit\general\test_array2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_array_as_generator()
    {
        var path = @"unit\general\test_array_as_generator.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_bad_array_size_bad()
    {
        var path = @"unit\general\test_bad_array_size-bad.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_bad_lb_ub_dom_bad()
    {
        var path = @"unit\general\test_bad_lb_ub_dom-bad.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_bool_var_array_access()
    {
        var path = @"unit\general\test_bool_var_array_access.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_concat1()
    {
        var path = @"unit\general\test_concat1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_concat2()
    {
        var path = @"unit\general\test_concat2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_concat3()
    {
        var path = @"unit\general\test_concat3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_empty_enum()
    {
        var path = @"unit\general\test_empty_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_is_fixed()
    {
        var path = @"unit\general\test_is_fixed.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_join1()
    {
        var path = @"unit\general\test_join1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_join2()
    {
        var path = @"unit\general\test_join2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_join3()
    {
        var path = @"unit\general\test_join3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_lb_ub_array_int()
    {
        var path = @"unit\general\test_lb_ub_array_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_lb_ub_dom_int()
    {
        var path = @"unit\general\test_lb_ub_dom_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_lb_ub_float()
    {
        var path = @"unit\general\test_lb_ub_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_let_complex()
    {
        var path = @"unit\general\test_let_complex.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_let_par_array()
    {
        var path = @"unit\general\test_let_par_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_let_simple()
    {
        var path = @"unit\general\test_let_simple.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_let_var_array()
    {
        var path = @"unit\general\test_let_var_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_let_with_annotation()
    {
        var path = @"unit\general\test_let_with_annotation.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_min_var_array()
    {
        var path = @"unit\general\test_min_var_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_mod_bounds()
    {
        var path = @"unit\general\test_mod_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_negated_and()
    {
        var path = @"unit\general\test_negated_and.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_negated_and_or()
    {
        var path = @"unit\general\test_negated_and_or.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_negated_let_bad()
    {
        var path = @"unit\general\test_negated_let_bad.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_negated_let_good()
    {
        var path = @"unit\general\test_negated_let_good.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_negated_let_good_2()
    {
        var path = @"unit\general\test_negated_let_good_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_negated_or()
    {
        var path = @"unit\general\test_negated_or.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_par_set_element()
    {
        var path = @"unit\general\test_par_set_element.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_par_set_id_array_index_sets()
    {
        var path = @"unit\general\test_par_set_id_array_index_sets.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_queens()
    {
        var path = @"unit\general\test_queens.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_reified_element_constraint()
    {
        var path = @"unit\general\test_reified_element_constraint.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_reified_let_bad()
    {
        var path = @"unit\general\test_reified_let_bad.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_reified_let_good()
    {
        var path = @"unit\general\test_reified_let_good.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_rounding_a()
    {
        var path = @"unit\general\test_rounding_a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_rounding_b()
    {
        var path = @"unit\general\test_rounding_b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_rounding_c()
    {
        var path = @"unit\general\test_rounding_c.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_same()
    {
        var path = @"unit\general\test_same.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_set_inequality_par()
    {
        var path = @"unit\general\test_set_inequality_par.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_set_lt_1()
    {
        var path = @"unit\general\test_set_lt_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_set_lt_2()
    {
        var path = @"unit\general\test_set_lt_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_set_lt_3()
    {
        var path = @"unit\general\test_set_lt_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_string_array_var()
    {
        var path = @"unit\general\test_string_array_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_string_cmp()
    {
        var path = @"unit\general\test_string_cmp.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_string_var()
    {
        var path = @"unit\general\test_string_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_string_with_quote()
    {
        var path = @"unit\general\test_string_with_quote.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_times_int_float_eq()
    {
        var path = @"unit\general\test_times_int_float_eq.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_times_int_float_eq__defaultopt()
    {
        var path = @"unit\general\test_times_int_float_eq__defaultopt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_to_enum()
    {
        var path = @"unit\general\test_to_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_undefined_enum()
    {
        var path = @"unit\general\test_undefined_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_var_array()
    {
        var path = @"unit\general\test_var_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_var_array_access()
    {
        var path = @"unit\general\test_var_array_access.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_var_prod()
    {
        var path = @"unit\general\test_var_prod.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_var_set_assignment()
    {
        var path = @"unit\general\test_var_set_assignment.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_test_var_set_element()
    {
        var path = @"unit\general\test_var_set_element.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_unicode_file_name_μ()
    {
        var path = @"unit\general\unicode_file_name_μ.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_wolfgoatetc()
    {
        var path = @"unit\general\wolfgoatetc.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_general_xorall_bv()
    {
        var path = @"unit\general\xorall_bv.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_typecheck_globals()
    {
        var path = @"unit\globals\typecheck_globals.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_anon_enum_json()
    {
        var path = @"unit\json\anon_enum_json.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_coerce_enum_str()
    {
        var path = @"unit\json\coerce_enum_str.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_coerce_enum_str_err()
    {
        var path = @"unit\json\coerce_enum_str_err.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_coerce_indices()
    {
        var path = @"unit\json\coerce_indices.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_coerce_set()
    {
        var path = @"unit\json\coerce_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_enum_array_xd()
    {
        var path = @"unit\json\enum_array_xd.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_enum_constructor_basic()
    {
        var path = @"unit\json\enum_constructor_basic.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_enum_constructor_basic_2()
    {
        var path = @"unit\json\enum_constructor_basic_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_enum_constructor_int()
    {
        var path = @"unit\json\enum_constructor_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_enum_constructor_nested()
    {
        var path = @"unit\json\enum_constructor_nested.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_enum_escaping()
    {
        var path = @"unit\json\enum_escaping.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_float_json_exponent()
    {
        var path = @"unit\json\float_json_exponent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_array2d_set()
    {
        var path = @"unit\json\json_array2d_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_enum_def()
    {
        var path = @"unit\json\json_enum_def.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_input_1()
    {
        var path = @"unit\json\json_input_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_input_multidim_enum_str()
    {
        var path = @"unit\json\json_input_multidim_enum_str.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_input_set()
    {
        var path = @"unit\json\json_input_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_input_str()
    {
        var path = @"unit\json\json_input_str.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_json_unicode_escapes()
    {
        var path = @"unit\json\json_unicode_escapes.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_mult_dim_enum()
    {
        var path = @"unit\json\mult_dim_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_record_json_input()
    {
        var path = @"unit\json\record_json_input.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_json_tuple_json_input()
    {
        var path = @"unit\json\tuple_json_input.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_complete()
    {
        var path = @"unit\on_restart\complete.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_last_val_bool()
    {
        var path = @"unit\on_restart\last_val_bool.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_last_val_float()
    {
        var path = @"unit\on_restart\last_val_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_last_val_int()
    {
        var path = @"unit\on_restart\last_val_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_last_val_set()
    {
        var path = @"unit\on_restart\last_val_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_sol_bool()
    {
        var path = @"unit\on_restart\sol_bool.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_sol_float()
    {
        var path = @"unit\on_restart\sol_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_sol_int()
    {
        var path = @"unit\on_restart\sol_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_on_restart_sol_set()
    {
        var path = @"unit\on_restart\sol_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_bind_absent()
    {
        var path = @"unit\optional\bind_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_conj_absent_1()
    {
        var path = @"unit\optional\conj_absent_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_conj_absent_2()
    {
        var path = @"unit\optional\conj_absent_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_fn_return_array_absent()
    {
        var path = @"unit\optional\fn_return_array_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_opt_array_access()
    {
        var path = @"unit\optional\opt_array_access.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_opt_array_access_absent()
    {
        var path = @"unit\optional\opt_array_access_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_opt_array_access_absent_empty()
    {
        var path = @"unit\optional\opt_array_access_absent_empty.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_opt_string_comprehension()
    {
        var path = @"unit\optional\opt_string_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_opt_top_absent()
    {
        var path = @"unit\optional\opt_top_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_binop_flatten()
    {
        var path = @"unit\optional\test-opt-binop-flatten.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_bool_1()
    {
        var path = @"unit\optional\test-opt-bool-1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_bool_2()
    {
        var path = @"unit\optional\test-opt-bool-2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_bool_3()
    {
        var path = @"unit\optional\test-opt-bool-3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_bool_4()
    {
        var path = @"unit\optional\test-opt-bool-4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_bool_5()
    {
        var path = @"unit\optional\test-opt-bool-5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_bool_6()
    {
        var path = @"unit\optional\test-opt-bool-6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_compute_bounds()
    {
        var path = @"unit\optional\test-opt-compute-bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_float_1()
    {
        var path = @"unit\optional\test-opt-float-1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_float_2()
    {
        var path = @"unit\optional\test-opt-float-2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_if_then_else()
    {
        var path = @"unit\optional\test-opt-if-then-else.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_int_1()
    {
        var path = @"unit\optional\test-opt-int-1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_bug_456()
    {
        var path = @"unit\optional\test_bug_456.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_count_set()
    {
        var path = @"unit\optional\test_count_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_deopt_absent()
    {
        var path = @"unit\optional\test_deopt_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_if_then_else_opt_bool()
    {
        var path = @"unit\optional\test_if_then_else_opt_bool.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_if_then_else_opt_float()
    {
        var path = @"unit\optional\test_if_then_else_opt_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_if_then_else_opt_int()
    {
        var path = @"unit\optional\test_if_then_else_opt_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_if_then_else_var_opt_bool()
    {
        var path = @"unit\optional\test_if_then_else_var_opt_bool.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_if_then_else_var_opt_float()
    {
        var path = @"unit\optional\test_if_then_else_var_opt_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_if_then_else_var_opt_int()
    {
        var path = @"unit\optional\test_if_then_else_var_opt_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_optional_not_absent()
    {
        var path = @"unit\optional\test_optional_not_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_comprehension()
    {
        var path = @"unit\optional\test_opt_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_dom_empty()
    {
        var path = @"unit\optional\test_opt_dom_empty.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_dom_empty_no_absent_zero()
    {
        var path = @"unit\optional\test_opt_dom_empty_no_absent_zero.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_max()
    {
        var path = @"unit\optional\test_opt_max.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_optional_test_opt_min()
    {
        var path = @"unit\optional\test_opt_min.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_arg_reif_output()
    {
        var path = @"unit\output\arg-reif-output.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_array_ann()
    {
        var path = @"unit\output\array-ann.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_array_of_array()
    {
        var path = @"unit\output\array_of_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_bug288a()
    {
        var path = @"unit\output\bug288a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_bug288b()
    {
        var path = @"unit\output\bug288b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_bug288c()
    {
        var path = @"unit\output\bug288c.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_ctx_ann()
    {
        var path = @"unit\output\ctx_ann.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_dzn_output_array()
    {
        var path = @"unit\output\dzn_output_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_enum_constructor_functions()
    {
        var path = @"unit\output\enum_constructor_functions.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_json_ann()
    {
        var path = @"unit\output\json_ann.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_local_output()
    {
        var path = @"unit\output\local_output.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_mzn_bottom1()
    {
        var path = @"unit\output\mzn_bottom1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_mzn_bottom2()
    {
        var path = @"unit\output\mzn_bottom2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_mzn_bottom3()
    {
        var path = @"unit\output\mzn_bottom3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_mzn_bottom4()
    {
        var path = @"unit\output\mzn_bottom4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_mzn_bottom5()
    {
        var path = @"unit\output\mzn_bottom5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_mzn_bottom6()
    {
        var path = @"unit\output\mzn_bottom6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_annotations_1()
    {
        var path = @"unit\output\output_annotations_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_annotations_2()
    {
        var path = @"unit\output\output_annotations_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_annotations_3()
    {
        var path = @"unit\output\output_annotations_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_annotations_4()
    {
        var path = @"unit\output\output_annotations_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_1()
    {
        var path = @"unit\output\output_sections_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_2()
    {
        var path = @"unit\output\output_sections_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_3()
    {
        var path = @"unit\output\output_sections_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_4()
    {
        var path = @"unit\output\output_sections_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_5()
    {
        var path = @"unit\output\output_sections_5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_6()
    {
        var path = @"unit\output\output_sections_6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_output_sections_7()
    {
        var path = @"unit\output\output_sections_7.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_quoted_id_ozn()
    {
        var path = @"unit\output\quoted_id_ozn.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_record_access_printing()
    {
        var path = @"unit\output\record_access_printing.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show2d_empty()
    {
        var path = @"unit\output\show2d_empty.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show3d_empty()
    {
        var path = @"unit\output\show3d_empty.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show_empty()
    {
        var path = @"unit\output\show_empty.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show_float()
    {
        var path = @"unit\output\show_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show_float_set()
    {
        var path = @"unit\output\show_float_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show_int()
    {
        var path = @"unit\output\show_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_show_int_set()
    {
        var path = @"unit\output\show_int_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_test_in_output()
    {
        var path = @"unit\output\test-in-output.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_var_enum()
    {
        var path = @"unit\output\var_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_very_empty()
    {
        var path = @"unit\output\very_empty.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_output_very_empty_set()
    {
        var path = @"unit\output\very_empty_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_param_file_param_file_array()
    {
        var path = @"unit\param_file\param_file_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_param_file_param_file_blacklist()
    {
        var path = @"unit\param_file\param_file_blacklist.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_param_file_param_file_nested_object()
    {
        var path = @"unit\param_file\param_file_nested_object.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_param_file_param_file_recursive()
    {
        var path = @"unit\param_file\param_file_recursive.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_param_file_param_file_resolution()
    {
        var path = @"unit\param_file\param_file_resolution.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_absent_id_crash()
    {
        var path = @"unit\regression\absent_id_crash.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_abs_bug()
    {
        var path = @"unit\regression\abs_bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_arg_reif_array_float()
    {
        var path = @"unit\regression\arg-reif-array-float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_arg_reif_array_int()
    {
        var path = @"unit\regression\arg-reif-array-int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_arg_reif_float()
    {
        var path = @"unit\regression\arg-reif-float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_arg_reif_int_set()
    {
        var path = @"unit\regression\arg-reif-int-set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_arg_reif_int()
    {
        var path = @"unit\regression\arg-reif-int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_array_of_empty_sets()
    {
        var path = @"unit\regression\array_of_empty_sets.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_array_set_element_nosets()
    {
        var path = @"unit\regression\array_set_element_nosets.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_array_var_set_element_nosets()
    {
        var path = @"unit\regression\array_var_set_element_nosets.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_assign_reverse_map()
    {
        var path = @"unit\regression\assign_reverse_map.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_big_array_lit()
    {
        var path = @"unit\regression\big_array_lit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bind_defines_var()
    {
        var path = @"unit\regression\bind-defines-var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_binop_mult_gclock()
    {
        var path = @"unit\regression\binop_mult_gclock.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bizarre()
    {
        var path = @"unit\regression\bizarre.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bool2float_let()
    {
        var path = @"unit\regression\bool2float_let.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bounds_for_linear_01_max_0()
    {
        var path = @"unit\regression\bounds_for_linear_01_max_0.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bounds_for_linear_01_max_1()
    {
        var path = @"unit\regression\bounds_for_linear_01_max_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bounds_for_linear_01_min_0()
    {
        var path = @"unit\regression\bounds_for_linear_01_min_0.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bounds_for_linear_01_min_1()
    {
        var path = @"unit\regression\bounds_for_linear_01_min_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug109()
    {
        var path = @"unit\regression\bug109.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug110()
    {
        var path = @"unit\regression\bug110.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug131()
    {
        var path = @"unit\regression\bug131.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug141()
    {
        var path = @"unit\regression\bug141.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug202()
    {
        var path = @"unit\regression\bug202.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug212()
    {
        var path = @"unit\regression\bug212.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug222()
    {
        var path = @"unit\regression\bug222.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug244()
    {
        var path = @"unit\regression\bug244.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug256()
    {
        var path = @"unit\regression\bug256.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug256b()
    {
        var path = @"unit\regression\bug256b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug259()
    {
        var path = @"unit\regression\bug259.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug269()
    {
        var path = @"unit\regression\bug269.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug282()
    {
        var path = @"unit\regression\bug282.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug283()
    {
        var path = @"unit\regression\bug283.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug284()
    {
        var path = @"unit\regression\bug284.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug287()
    {
        var path = @"unit\regression\bug287.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug290_orig()
    {
        var path = @"unit\regression\bug290_orig.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug290_simple()
    {
        var path = @"unit\regression\bug290_simple.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug312()
    {
        var path = @"unit\regression\bug312.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug318_orig()
    {
        var path = @"unit\regression\bug318_orig.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug335()
    {
        var path = @"unit\regression\bug335.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug337()
    {
        var path = @"unit\regression\bug337.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug337_mod()
    {
        var path = @"unit\regression\bug337_mod.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug341()
    {
        var path = @"unit\regression\bug341.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug347()
    {
        var path = @"unit\regression\bug347.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug380()
    {
        var path = @"unit\regression\bug380.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug45()
    {
        var path = @"unit\regression\bug45.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug52()
    {
        var path = @"unit\regression\bug52.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug532()
    {
        var path = @"unit\regression\bug532.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug534()
    {
        var path = @"unit\regression\bug534.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug536()
    {
        var path = @"unit\regression\bug536.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug552()
    {
        var path = @"unit\regression\bug552.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug565()
    {
        var path = @"unit\regression\bug565.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug570()
    {
        var path = @"unit\regression\bug570.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug620()
    {
        var path = @"unit\regression\bug620.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug635()
    {
        var path = @"unit\regression\bug635.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug67()
    {
        var path = @"unit\regression\bug67.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug68b()
    {
        var path = @"unit\regression\bug68b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug69_1()
    {
        var path = @"unit\regression\bug69_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug69_2()
    {
        var path = @"unit\regression\bug69_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug69_3()
    {
        var path = @"unit\regression\bug69_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug69_4()
    {
        var path = @"unit\regression\bug69_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug70()
    {
        var path = @"unit\regression\bug70.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug71_1()
    {
        var path = @"unit\regression\bug71_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug71_2()
    {
        var path = @"unit\regression\bug71_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug82()
    {
        var path = @"unit\regression\bug82.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug85()
    {
        var path = @"unit\regression\bug85.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug_08()
    {
        var path = @"unit\regression\bug_08.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug_629()
    {
        var path = @"unit\regression\bug_629.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug_empty_enum_extension()
    {
        var path = @"unit\regression\bug_empty_enum_extension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug_opt_polymorphic()
    {
        var path = @"unit\regression\bug_opt_polymorphic.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_bug_r7995()
    {
        var path = @"unit\regression\bug_r7995.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_cardinality_atmost_partition()
    {
        var path = @"unit\regression\cardinality_atmost_partition.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_card_flatten_lb()
    {
        var path = @"unit\regression\card_flatten_lb.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_change_partition()
    {
        var path = @"unit\regression\change_partition.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_checker_mzn_check_var()
    {
        var path = @"unit\regression\checker_mzn_check_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_checker_opt()
    {
        var path = @"unit\regression\checker_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_checker_params()
    {
        var path = @"unit\regression\checker_params.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_checker_same_var()
    {
        var path = @"unit\regression\checker_same_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_checker_var_bug()
    {
        var path = @"unit\regression\checker_var_bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_check_dom_float_array()
    {
        var path = @"unit\regression\check_dom_float_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_coerce_set_to_array_1()
    {
        var path = @"unit\regression\coerce_set_to_array_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_coerce_set_to_array_2()
    {
        var path = @"unit\regression\coerce_set_to_array_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_coercion_par()
    {
        var path = @"unit\regression\coercion_par.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_comprehension_where()
    {
        var path = @"unit\regression\comprehension_where.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_comp_in_empty_set()
    {
        var path = @"unit\regression\comp_in_empty_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_constructor_of_set()
    {
        var path = @"unit\regression\constructor_of_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_cse_array_lit()
    {
        var path = @"unit\regression\cse_array_lit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_cyclic_include()
    {
        var path = @"unit\regression\cyclic_include.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_decision_tree_binary()
    {
        var path = @"unit\regression\decision_tree_binary.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_empty_array1d()
    {
        var path = @"unit\regression\empty-array1d.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_enigma_1568()
    {
        var path = @"unit\regression\enigma_1568.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_error_in_comprehension()
    {
        var path = @"unit\regression\error_in_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_flatten_comp_in()
    {
        var path = @"unit\regression\flatten_comp_in.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_flatten_comp_in2()
    {
        var path = @"unit\regression\flatten_comp_in2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_flat_cv_call()
    {
        var path = @"unit\regression\flat_cv_call.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_flat_cv_let()
    {
        var path = @"unit\regression\flat_cv_let.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_flat_set_lit()
    {
        var path = @"unit\regression\flat_set_lit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_flipstrip_simple()
    {
        var path = @"unit\regression\flipstrip_simple.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_float_card()
    {
        var path = @"unit\regression\float_card.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_float_ceil_floor()
    {
        var path = @"unit\regression\float_ceil_floor.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_float_div_crash()
    {
        var path = @"unit\regression\float_div_crash.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_float_mod_crash()
    {
        var path = @"unit\regression\float_mod_crash.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_float_opt_crash()
    {
        var path = @"unit\regression\float_opt_crash.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_follow_id_absent_crash()
    {
        var path = @"unit\regression\follow_id_absent_crash.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github537()
    {
        var path = @"unit\regression\github537.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_638_reduced()
    {
        var path = @"unit\regression\github_638_reduced.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_639_part1()
    {
        var path = @"unit\regression\github_639_part1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_639_part2()
    {
        var path = @"unit\regression\github_639_part2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_644_a()
    {
        var path = @"unit\regression\github_644_a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_644_b()
    {
        var path = @"unit\regression\github_644_b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_644_c()
    {
        var path = @"unit\regression\github_644_c.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_644_d()
    {
        var path = @"unit\regression\github_644_d.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_644_e()
    {
        var path = @"unit\regression\github_644_e.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_646()
    {
        var path = @"unit\regression\github_646.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_648_par_array_decl()
    {
        var path = @"unit\regression\github_648_par_array_decl.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_648_par_decl()
    {
        var path = @"unit\regression\github_648_par_decl.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_656()
    {
        var path = @"unit\regression\github_656.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_660a()
    {
        var path = @"unit\regression\github_660a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_660b()
    {
        var path = @"unit\regression\github_660b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_661_part1()
    {
        var path = @"unit\regression\github_661_part1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_661_part2()
    {
        var path = @"unit\regression\github_661_part2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_664()
    {
        var path = @"unit\regression\github_664.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_666()
    {
        var path = @"unit\regression\github_666.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_667()
    {
        var path = @"unit\regression\github_667.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_668()
    {
        var path = @"unit\regression\github_668.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_669()
    {
        var path = @"unit\regression\github_669.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_670()
    {
        var path = @"unit\regression\github_670.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_671()
    {
        var path = @"unit\regression\github_671.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_673()
    {
        var path = @"unit\regression\github_673.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_674()
    {
        var path = @"unit\regression\github_674.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_675a()
    {
        var path = @"unit\regression\github_675a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_675b()
    {
        var path = @"unit\regression\github_675b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_680()
    {
        var path = @"unit\regression\github_680.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_681()
    {
        var path = @"unit\regression\github_681.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_683()
    {
        var path = @"unit\regression\github_683.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_685()
    {
        var path = @"unit\regression\github_685.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_687()
    {
        var path = @"unit\regression\github_687.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_691()
    {
        var path = @"unit\regression\github_691.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_693_part1()
    {
        var path = @"unit\regression\github_693_part1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_693_part2()
    {
        var path = @"unit\regression\github_693_part2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_695()
    {
        var path = @"unit\regression\github_695.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_700()
    {
        var path = @"unit\regression\github_700.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_700_bad_sol()
    {
        var path = @"unit\regression\github_700_bad_sol.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_716()
    {
        var path = @"unit\regression\github_716.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_725()
    {
        var path = @"unit\regression\github_725.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_726()
    {
        var path = @"unit\regression\github_726.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_728()
    {
        var path = @"unit\regression\github_728.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_730()
    {
        var path = @"unit\regression\github_730.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_732()
    {
        var path = @"unit\regression\github_732.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_748()
    {
        var path = @"unit\regression\github_748.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_749()
    {
        var path = @"unit\regression\github_749.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_752()
    {
        var path = @"unit\regression\github_752.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_754()
    {
        var path = @"unit\regression\github_754.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_758()
    {
        var path = @"unit\regression\github_758.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_760()
    {
        var path = @"unit\regression\github_760.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_761()
    {
        var path = @"unit\regression\github_761.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_762()
    {
        var path = @"unit\regression\github_762.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_github_765()
    {
        var path = @"unit\regression\github_765.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_hundred_doors_unoptimized()
    {
        var path = @"unit\regression\hundred_doors_unoptimized.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_if_then_else_absent()
    {
        var path = @"unit\regression\if_then_else_absent.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_int_times()
    {
        var path = @"unit\regression\int_times.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_in_array_eval_error()
    {
        var path = @"unit\regression\in_array_eval_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_is_fixed()
    {
        var path = @"unit\regression\is_fixed.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_is_fixed_comp()
    {
        var path = @"unit\regression\is_fixed_comp.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_lb_ub_dom_array_opt()
    {
        var path = @"unit\regression\lb_ub_dom_array_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_let_domain_from_generator()
    {
        var path = @"unit\regression\let_domain_from_generator.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_linear_bool_elem_bug()
    {
        var path = @"unit\regression\linear_bool_elem_bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_makepar_output()
    {
        var path = @"unit\regression\makepar_output.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_multi_goal_hierarchy_error()
    {
        var path = @"unit\regression\multi_goal_hierarchy_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_nested_clause()
    {
        var path = @"unit\regression\nested_clause.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_non_set_array_ti_location()
    {
        var path = @"unit\regression\non-set-array-ti-location.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_non_pos_pow()
    {
        var path = @"unit\regression\non_pos_pow.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_nosets_369()
    {
        var path = @"unit\regression\nosets_369.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_nosets_set_search()
    {
        var path = @"unit\regression\nosets_set_search.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_no_macro()
    {
        var path = @"unit\regression\no_macro.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_opt_minmax()
    {
        var path = @"unit\regression\opt_minmax.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_opt_noncontiguous_domain()
    {
        var path = @"unit\regression\opt_noncontiguous_domain.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_opt_removed_items()
    {
        var path = @"unit\regression\opt_removed_items.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_output_2d_array_enum()
    {
        var path = @"unit\regression\output_2d_array_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_output_fn_toplevel_var()
    {
        var path = @"unit\regression\output_fn_toplevel_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_output_only_fn()
    {
        var path = @"unit\regression\output_only_fn.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_output_only_no_rhs()
    {
        var path = @"unit\regression\output_only_no_rhs.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_overloading()
    {
        var path = @"unit\regression\overloading.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_parser_location()
    {
        var path = @"unit\regression\parser_location.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_parse_assignments()
    {
        var path = @"unit\regression\parse_assignments.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_par_opt_dom()
    {
        var path = @"unit\regression\par_opt_dom.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_par_opt_equal()
    {
        var path = @"unit\regression\par_opt_equal.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_polymorphic_var_and_par()
    {
        var path = @"unit\regression\polymorphic_var_and_par.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_pow_undefined()
    {
        var path = @"unit\regression\pow_undefined.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_pred_param_r7550()
    {
        var path = @"unit\regression\pred_param_r7550.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_round()
    {
        var path = @"unit\regression\round.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_seq_search_bug()
    {
        var path = @"unit\regression\seq_search_bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_set_inequality_par()
    {
        var path = @"unit\regression\set_inequality_par.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_slice_enum_indexset()
    {
        var path = @"unit\regression\slice_enum_indexset.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_string_test_arg()
    {
        var path = @"unit\regression\string-test-arg.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_subsets_100()
    {
        var path = @"unit\regression\subsets_100.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_annotation_on_exists()
    {
        var path = @"unit\regression\test_annotation_on_exists.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bool2int()
    {
        var path = @"unit\regression\test_bool2int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug218()
    {
        var path = @"unit\regression\test_bug218.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug359()
    {
        var path = @"unit\regression\test_bug359.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug45()
    {
        var path = @"unit\regression\test_bug45.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug53()
    {
        var path = @"unit\regression\test_bug53.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug54()
    {
        var path = @"unit\regression\test_bug54.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug55()
    {
        var path = @"unit\regression\test_bug55.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug57()
    {
        var path = @"unit\regression\test_bug57.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug65()
    {
        var path = @"unit\regression\test_bug65.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug66()
    {
        var path = @"unit\regression\test_bug66.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug67()
    {
        var path = @"unit\regression\test_bug67.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug70()
    {
        var path = @"unit\regression\test_bug70.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug71()
    {
        var path = @"unit\regression\test_bug71.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug72()
    {
        var path = @"unit\regression\test_bug72.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_129()
    {
        var path = @"unit\regression\test_bug_129.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_476()
    {
        var path = @"unit\regression\test_bug_476.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_483()
    {
        var path = @"unit\regression\test_bug_483.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_493()
    {
        var path = @"unit\regression\test_bug_493.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_494()
    {
        var path = @"unit\regression\test_bug_494.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_520()
    {
        var path = @"unit\regression\test_bug_520.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_521()
    {
        var path = @"unit\regression\test_bug_521.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_527()
    {
        var path = @"unit\regression\test_bug_527.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_529()
    {
        var path = @"unit\regression\test_bug_529.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_588()
    {
        var path = @"unit\regression\test_bug_588.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_637()
    {
        var path = @"unit\regression\test_bug_637.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_array_sum_bounds()
    {
        var path = @"unit\regression\test_bug_array_sum_bounds.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_ite_array_eq()
    {
        var path = @"unit\regression\test_bug_ite_array_eq.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_bug_pred_arg()
    {
        var path = @"unit\regression\test_bug_pred_arg.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_equality_of_indirect_annotations()
    {
        var path = @"unit\regression\test_equality_of_indirect_annotations.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_github_30()
    {
        var path = @"unit\regression\test_github_30.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_multioutput()
    {
        var path = @"unit\regression\test_multioutput.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_not_in()
    {
        var path = @"unit\regression\test_not_in.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_output_array_of_set()
    {
        var path = @"unit\regression\test_output_array_of_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_output_string_var()
    {
        var path = @"unit\regression\test_output_string_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_parout()
    {
        var path = @"unit\regression\test_parout.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_seq_precede_chain_set()
    {
        var path = @"unit\regression\test_seq_precede_chain_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_test_slice_1d_array()
    {
        var path = @"unit\regression\test_slice_1d_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_ti_error_location()
    {
        var path = @"unit\regression\ti_error_location.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_ts_bug()
    {
        var path = @"unit\regression\ts_bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_type_specialise_array_return()
    {
        var path = @"unit\regression\type_specialise_array_return.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_var_bool_comp()
    {
        var path = @"unit\regression\var_bool_comp.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_var_opt_unconstrained()
    {
        var path = @"unit\regression\var_opt_unconstrained.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_var_self_assign_bug()
    {
        var path = @"unit\regression\var_self_assign_bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_warm_start()
    {
        var path = @"unit\regression\warm_start.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_where_forall_bug()
    {
        var path = @"unit\regression\where-forall-bug.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_regression_xor_mixed_context()
    {
        var path = @"unit\regression\xor_mixed_context.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_choice_1()
    {
        var path = @"unit\search\int_choice_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_choice_2()
    {
        var path = @"unit\search\int_choice_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_choice_6()
    {
        var path = @"unit\search\int_choice_6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_var_select_1()
    {
        var path = @"unit\search\int_var_select_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_var_select_2()
    {
        var path = @"unit\search\int_var_select_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_var_select_3()
    {
        var path = @"unit\search\int_var_select_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_var_select_4()
    {
        var path = @"unit\search\int_var_select_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_int_var_select_6()
    {
        var path = @"unit\search\int_var_select_6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_test_ff1()
    {
        var path = @"unit\search\test-ff1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_test_ff2()
    {
        var path = @"unit\search\test-ff2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_test_ff3()
    {
        var path = @"unit\search\test-ff3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_test_large1()
    {
        var path = @"unit\search\test-large1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_test_med1()
    {
        var path = @"unit\search\test-med1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_search_test_small1()
    {
        var path = @"unit\search\test-small1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_alias()
    {
        var path = @"unit\types\alias.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_alias_call()
    {
        var path = @"unit\types\alias_call.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_alias_extern_dom()
    {
        var path = @"unit\types\alias_extern_dom.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_alias_set_of_array()
    {
        var path = @"unit\types\alias_set_of_array.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_comprehension_type()
    {
        var path = @"unit\types\comprehension_type.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_cv_comprehension()
    {
        var path = @"unit\types\cv_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_enum_decl()
    {
        var path = @"unit\types\enum_decl.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_enum_refl()
    {
        var path = @"unit\types\enum_refl.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_github_647()
    {
        var path = @"unit\types\github_647.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_nested_type_inst_id()
    {
        var path = @"unit\types\nested_type_inst_id.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_nonbool_constraint()
    {
        var path = @"unit\types\nonbool_constraint.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_nonbool_constraint_let()
    {
        var path = @"unit\types\nonbool_constraint_let.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_non_contig_enum()
    {
        var path = @"unit\types\non_contig_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_overload_inst_tuple_return()
    {
        var path = @"unit\types\overload_inst_tuple_return.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_par_struct_tiid()
    {
        var path = @"unit\types\par_struct_tiid.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_polymorphic_overloading()
    {
        var path = @"unit\types\polymorphic_overloading.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_access_error()
    {
        var path = @"unit\types\record_access_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_access_success()
    {
        var path = @"unit\types\record_access_success.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_array_access_error()
    {
        var path = @"unit\types\record_array_access_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_binop_par()
    {
        var path = @"unit\types\record_binop_par.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_binop_var()
    {
        var path = @"unit\types\record_binop_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_comprehensions()
    {
        var path = @"unit\types\record_comprehensions.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_decl_error()
    {
        var path = @"unit\types\record_decl_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_ite_error()
    {
        var path = @"unit\types\record_ite_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_lit_dup()
    {
        var path = @"unit\types\record_lit_dup.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_nested()
    {
        var path = @"unit\types\record_nested.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_output()
    {
        var path = @"unit\types\record_output.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_subtyping()
    {
        var path = @"unit\types\record_subtyping.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_var_element()
    {
        var path = @"unit\types\record_var_element.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_record_var_ite()
    {
        var path = @"unit\types\record_var_ite.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_specialise_large_struct()
    {
        var path = @"unit\types\specialise_large_struct.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_array_coercion()
    {
        var path = @"unit\types\struct_array_coercion.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_bind_1()
    {
        var path = @"unit\types\struct_bind_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_bind_2()
    {
        var path = @"unit\types\struct_bind_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_domain_1()
    {
        var path = @"unit\types\struct_domain_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_domain_2()
    {
        var path = @"unit\types\struct_domain_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_domain_3()
    {
        var path = @"unit\types\struct_domain_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_domain_4()
    {
        var path = @"unit\types\struct_domain_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_domain_5()
    {
        var path = @"unit\types\struct_domain_5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_domain_6()
    {
        var path = @"unit\types\struct_domain_6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_index_sets_1()
    {
        var path = @"unit\types\struct_index_sets_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_index_sets_2()
    {
        var path = @"unit\types\struct_index_sets_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_par_function_version()
    {
        var path = @"unit\types\struct_par_function_version.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_return_ti_1()
    {
        var path = @"unit\types\struct_return_ti_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_return_ti_2()
    {
        var path = @"unit\types\struct_return_ti_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_return_ti_3()
    {
        var path = @"unit\types\struct_return_ti_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_return_ti_4()
    {
        var path = @"unit\types\struct_return_ti_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_specialise()
    {
        var path = @"unit\types\struct_specialise.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_struct_specialise_return()
    {
        var path = @"unit\types\struct_specialise_return.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_test_any_enum_typeinstid()
    {
        var path = @"unit\types\test_any_enum_typeinstid.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_access_error1()
    {
        var path = @"unit\types\tuple_access_error1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_access_error2()
    {
        var path = @"unit\types\tuple_access_error2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_access_success()
    {
        var path = @"unit\types\tuple_access_success.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_array_access_error()
    {
        var path = @"unit\types\tuple_array_access_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_binop_par()
    {
        var path = @"unit\types\tuple_binop_par.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_binop_var()
    {
        var path = @"unit\types\tuple_binop_var.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_comprehensions()
    {
        var path = @"unit\types\tuple_comprehensions.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_int_set_of_int_specialisation()
    {
        var path = @"unit\types\tuple_int_set_of_int_specialisation.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_ite_error()
    {
        var path = @"unit\types\tuple_ite_error.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_lit()
    {
        var path = @"unit\types\tuple_lit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_mkpar()
    {
        var path = @"unit\types\tuple_mkpar.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_output()
    {
        var path = @"unit\types\tuple_output.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_subtyping()
    {
        var path = @"unit\types\tuple_subtyping.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_var_element()
    {
        var path = @"unit\types\tuple_var_element.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_tuple_var_ite()
    {
        var path = @"unit\types\tuple_var_ite.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_ann_a()
    {
        var path = @"unit\types\var_ann_a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_ann_b()
    {
        var path = @"unit\types\var_ann_b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_ann_comprehension()
    {
        var path = @"unit\types\var_ann_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_set_bool()
    {
        var path = @"unit\types\var_set_bool.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_set_float()
    {
        var path = @"unit\types\var_set_float.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_set_float_comprehension()
    {
        var path = @"unit\types\var_set_float_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_string_a()
    {
        var path = @"unit\types\var_string_a.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_string_b()
    {
        var path = @"unit\types\var_string_b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_types_var_string_comprehension()
    {
        var path = @"unit\types\var_string_comprehension.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_alldifferent_globals_alldiff_set_nosets()
    {
        var path = @"unit\globals\alldifferent\globals_alldiff_set_nosets.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_alldifferent_globals_all_different_int()
    {
        var path = @"unit\globals\alldifferent\globals_all_different_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_alldifferent_globals_all_different_int_opt()
    {
        var path = @"unit\globals\alldifferent\globals_all_different_int_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_alldifferent_globals_all_different_set()
    {
        var path = @"unit\globals\alldifferent\globals_all_different_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_alldifferent_except_0_test_alldiff_except0()
    {
        var path = @"unit\globals\alldifferent_except_0\test_alldiff_except0.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_alldifferent_except_0_test_alldiff_except0b()
    {
        var path = @"unit\globals\alldifferent_except_0\test_alldiff_except0b.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_all_disjoint_globals_all_disjoint()
    {
        var path = @"unit\globals\all_disjoint\globals_all_disjoint.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_all_equal_globals_all_equal_int()
    {
        var path = @"unit\globals\all_equal\globals_all_equal_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_all_equal_globals_all_equal_set()
    {
        var path = @"unit\globals\all_equal\globals_all_equal_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_among_globals_among()
    {
        var path = @"unit\globals\among\globals_among.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_max_globals_arg_max()
    {
        var path = @"unit\globals\arg_max\globals_arg_max.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_max_globals_arg_max_opt()
    {
        var path = @"unit\globals\arg_max\globals_arg_max_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_max_globals_arg_max_opt_weak()
    {
        var path = @"unit\globals\arg_max\globals_arg_max_opt_weak.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_min_globals_arg_max_opt()
    {
        var path = @"unit\globals\arg_min\globals_arg_max_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_min_globals_arg_min()
    {
        var path = @"unit\globals\arg_min\globals_arg_min.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_min_globals_arg_min_opt_weak()
    {
        var path = @"unit\globals\arg_min\globals_arg_min_opt_weak.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_arg_val_arg_val_enum()
    {
        var path = @"unit\globals\arg_val\arg_val_enum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_atleast_globals_at_least()
    {
        var path = @"unit\globals\atleast\globals_at_least.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_atmost1_globals_at_most1()
    {
        var path = @"unit\globals\atmost1\globals_at_most1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_bin_packing_globals_bin_packing()
    {
        var path = @"unit\globals\bin_packing\globals_bin_packing.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_bin_packing_capa_globals_bin_packing_capa()
    {
        var path = @"unit\globals\bin_packing_capa\globals_bin_packing_capa.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_circuit_test_circuit()
    {
        var path = @"unit\globals\circuit\test_circuit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_count_globals_count()
    {
        var path = @"unit\globals\count\globals_count.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_cumulative_github_589()
    {
        var path = @"unit\globals\cumulative\github_589.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_cumulative_globals_cumulative()
    {
        var path = @"unit\globals\cumulative\globals_cumulative.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_cumulative_unsat_resource_cumulative()
    {
        var path = @"unit\globals\cumulative\unsat_resource_cumulative.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_cumulatives_globals_cumulatives()
    {
        var path = @"unit\globals\cumulatives\globals_cumulatives.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_decreasing_globals_decreasing()
    {
        var path = @"unit\globals\decreasing\globals_decreasing.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_disjoint_globals_disjoint()
    {
        var path = @"unit\globals\disjoint\globals_disjoint.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_distribute_globals_distribute()
    {
        var path = @"unit\globals\distribute\globals_distribute.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_globals_global_cardinality()
    {
        var path = @"unit\globals\global_cardinality\globals_global_cardinality.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_low_up()
    {
        var path = @"unit\globals\global_cardinality\globals_global_cardinality_low_up.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_low_up_opt()
    {
        var path = @"unit\globals\global_cardinality\globals_global_cardinality_low_up_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_low_up_set()
    {
        var path = @"unit\globals\global_cardinality\globals_global_cardinality_low_up_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_opt()
    {
        var path = @"unit\globals\global_cardinality\globals_global_cardinality_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_globals_global_cardinality_set()
    {
        var path = @"unit\globals\global_cardinality\globals_global_cardinality_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed()
    {
        var path = @"unit\globals\global_cardinality_closed\globals_global_cardinality_closed.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed_opt()
    {
        var path =
            @"unit\globals\global_cardinality_closed\globals_global_cardinality_closed_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_closed_set()
    {
        var path =
            @"unit\globals\global_cardinality_closed\globals_global_cardinality_closed_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed()
    {
        var path =
            @"unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed_opt()
    {
        var path =
            @"unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_global_cardinality_closed_globals_global_cardinality_low_up_closed_set()
    {
        var path =
            @"unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_increasing_globals_increasing()
    {
        var path = @"unit\globals\increasing\globals_increasing.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_increasing_globals_strictly_increasing_opt()
    {
        var path = @"unit\globals\increasing\globals_strictly_increasing_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_int_set_channel_globals_int_set_channel()
    {
        var path = @"unit\globals\int_set_channel\globals_int_set_channel.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_int_set_channel_test_int_set_channel2()
    {
        var path = @"unit\globals\int_set_channel\test_int_set_channel2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_inverse_globals_inverse()
    {
        var path = @"unit\globals\inverse\globals_inverse.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_inverse_inverse_opt()
    {
        var path = @"unit\globals\inverse\inverse_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_inverse_in_range_globals_inverse_in_range()
    {
        var path = @"unit\globals\inverse_in_range\globals_inverse_in_range.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_inverse_set_globals_inverse_set()
    {
        var path = @"unit\globals\inverse_set\globals_inverse_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex2_globals_lex2()
    {
        var path = @"unit\globals\lex2\globals_lex2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_chain_globals_lex_chain()
    {
        var path = @"unit\globals\lex_chain\globals_lex_chain.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_chain_globals_lex_chain__orbitope()
    {
        var path = @"unit\globals\lex_chain\globals_lex_chain__orbitope.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_greater_globals_lex_greater()
    {
        var path = @"unit\globals\lex_greater\globals_lex_greater.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_greatereq_globals_lex_greatereq()
    {
        var path = @"unit\globals\lex_greatereq\globals_lex_greatereq.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_less_globals_lex_less()
    {
        var path = @"unit\globals\lex_less\globals_lex_less.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_less_test_bool_lex_less()
    {
        var path = @"unit\globals\lex_less\test_bool_lex_less.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_lesseq_globals_lex_lesseq()
    {
        var path = @"unit\globals\lex_lesseq\globals_lex_lesseq.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_lex_lesseq_test_bool_lex_lesseq()
    {
        var path = @"unit\globals\lex_lesseq\test_bool_lex_lesseq.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_link_set_to_booleans_globals_link_set_to_booleans()
    {
        var path = @"unit\globals\link_set_to_booleans\globals_link_set_to_booleans.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_maximum_globals_maximum_int()
    {
        var path = @"unit\globals\maximum\globals_maximum_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_minimum_globals_minimum_int()
    {
        var path = @"unit\globals\minimum\globals_minimum_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_nvalue_globals_nvalue()
    {
        var path = @"unit\globals\nvalue\globals_nvalue.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_nvalue_nvalue_total()
    {
        var path = @"unit\globals\nvalue\nvalue_total.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_partition_set_globals_partition_set()
    {
        var path = @"unit\globals\partition_set\globals_partition_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_range_globals_range()
    {
        var path = @"unit\globals\range\globals_range.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular()
    {
        var path = @"unit\globals\regular\globals_regular.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular_regex_1()
    {
        var path = @"unit\globals\regular\globals_regular_regex_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular_regex_2()
    {
        var path = @"unit\globals\regular\globals_regular_regex_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular_regex_3()
    {
        var path = @"unit\globals\regular\globals_regular_regex_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular_regex_4()
    {
        var path = @"unit\globals\regular\globals_regular_regex_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular_regex_5()
    {
        var path = @"unit\globals\regular\globals_regular_regex_5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_regular_globals_regular_regex_6()
    {
        var path = @"unit\globals\regular\globals_regular_regex_6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_roots_roots_bad()
    {
        var path = @"unit\globals\roots\roots_bad.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_roots_test_roots()
    {
        var path = @"unit\globals\roots\test_roots.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_roots_test_roots2()
    {
        var path = @"unit\globals\roots\test_roots2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_roots_test_roots3()
    {
        var path = @"unit\globals\roots\test_roots3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_sliding_sum_globals_sliding_sum()
    {
        var path = @"unit\globals\sliding_sum\globals_sliding_sum.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_sort_globals_sort()
    {
        var path = @"unit\globals\sort\globals_sort.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_strict_lex2_globals_strict_lex2()
    {
        var path = @"unit\globals\strict_lex2\globals_strict_lex2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_subcircuit_test_subcircuit()
    {
        var path = @"unit\globals\subcircuit\test_subcircuit.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_sum_pred_globals_sum_pred()
    {
        var path = @"unit\globals\sum_pred\globals_sum_pred.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_table_globals_table()
    {
        var path = @"unit\globals\table\globals_table.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_table_globals_table_opt()
    {
        var path = @"unit\globals\table\globals_table_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_value_precede_globals_value_precede_int()
    {
        var path = @"unit\globals\value_precede\globals_value_precede_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_value_precede_globals_value_precede_int_opt()
    {
        var path = @"unit\globals\value_precede\globals_value_precede_int_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_value_precede_globals_value_precede_set()
    {
        var path = @"unit\globals\value_precede\globals_value_precede_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_value_precede_chain_globals_value_precede_chain_int()
    {
        var path = @"unit\globals\value_precede_chain\globals_value_precede_chain_int.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_value_precede_chain_globals_value_precede_chain_int_opt()
    {
        var path = @"unit\globals\value_precede_chain\globals_value_precede_chain_int_opt.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_value_precede_chain_globals_value_precede_chain_set()
    {
        var path = @"unit\globals\value_precede_chain\globals_value_precede_chain_set.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_unit_globals_var_sqr_sym_globals_var_sqr_sym()
    {
        var path = @"unit\globals\var_sqr_sym\globals_var_sqr_sym.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_2DPacking()
    {
        var path = @"examples\2DPacking.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_alpha()
    {
        var path = @"examples\alpha.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships10()
    {
        var path = @"examples\battleships10.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_1()
    {
        var path = @"examples\battleships_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_2()
    {
        var path = @"examples\battleships_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_3()
    {
        var path = @"examples\battleships_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_4()
    {
        var path = @"examples\battleships_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_5()
    {
        var path = @"examples\battleships_5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_7()
    {
        var path = @"examples\battleships_7.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_battleships_9()
    {
        var path = @"examples\battleships_9.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_blocksworld_instance_1()
    {
        var path = @"examples\blocksworld_instance_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_blocksworld_instance_2()
    {
        var path = @"examples\blocksworld_instance_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_cutstock()
    {
        var path = @"examples\cutstock.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_eq20()
    {
        var path = @"examples\eq20.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_factory_planning_instance()
    {
        var path = @"examples\factory_planning_instance.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_golomb()
    {
        var path = @"examples\golomb.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_halfreif()
    {
        var path = @"examples\halfreif.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_jobshop2x2()
    {
        var path = @"examples\jobshop2x2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_knights()
    {
        var path = @"examples\knights.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_langford()
    {
        var path = @"examples\langford.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_langford2()
    {
        var path = @"examples\langford2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_latin_squares_fd()
    {
        var path = @"examples\latin_squares_fd.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_magicsq_3()
    {
        var path = @"examples\magicsq_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_magicsq_4()
    {
        var path = @"examples\magicsq_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_magicsq_5()
    {
        var path = @"examples\magicsq_5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_multidimknapsack_simple()
    {
        var path = @"examples\multidimknapsack_simple.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_oss()
    {
        var path = @"examples\oss.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_packing()
    {
        var path = @"examples\packing.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_perfsq()
    {
        var path = @"examples\perfsq.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_perfsq2()
    {
        var path = @"examples\perfsq2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_photo()
    {
        var path = @"examples\photo.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_product_fd()
    {
        var path = @"examples\product_fd.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_product_lp()
    {
        var path = @"examples\product_lp.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_quasigroup_qg5()
    {
        var path = @"examples\quasigroup_qg5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_queen_cp2()
    {
        var path = @"examples\queen_cp2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_queen_ip()
    {
        var path = @"examples\queen_ip.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_radiation()
    {
        var path = @"examples\radiation.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_simple_sat()
    {
        var path = @"examples\simple_sat.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_singHoist2()
    {
        var path = @"examples\singHoist2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_steiner_triples()
    {
        var path = @"examples\steiner-triples.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_sudoku()
    {
        var path = @"examples\sudoku.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_template_design()
    {
        var path = @"examples\template_design.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_tenpenki_1()
    {
        var path = @"examples\tenpenki_1.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_tenpenki_2()
    {
        var path = @"examples\tenpenki_2.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_tenpenki_3()
    {
        var path = @"examples\tenpenki_3.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_tenpenki_4()
    {
        var path = @"examples\tenpenki_4.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_tenpenki_5()
    {
        var path = @"examples\tenpenki_5.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_tenpenki_6()
    {
        var path = @"examples\tenpenki_6.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_timetabling()
    {
        var path = @"examples\timetabling.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_trucking()
    {
        var path = @"examples\trucking.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_warehouses()
    {
        var path = @"examples\warehouses.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_wolf_goat_cabbage()
    {
        var path = @"examples\wolf_goat_cabbage.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }

    [Fact]
    public void test_examples_zebra()
    {
        var path = @"examples\zebra.mzn";
        var lexer = Lexer.LexFile(path);
        var tokens = lexer.ToArray();
    }
}
