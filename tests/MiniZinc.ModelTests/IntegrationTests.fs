namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module IntegrationTests =

    let cwd =
        Directory.GetCurrentDirectory()
        |> DirectoryInfo
        
    let projectDir =
        cwd.Parent.Parent.Parent.Parent.Parent
        
    let specDir =
        projectDir <//> "tests" <//> "libminizinc"
        
    let testParseFile filePath =
        let file = projectDir </> filePath
        let result = parseModelFile file.FullName
        match result with
        | Result.Ok model ->
            ()
        | Result.Error err ->
            Assert.Fail(err.Message)            

    [<Fact>]
    let ``test test-globals-float`` () =
        testParseFile @"tests\libminizinc\unit\test-globals-float.mzn"

    [<Fact>]
    let ``test aggregation`` () =
        testParseFile @"tests\libminizinc\unit\compilation\aggregation.mzn"

    [<Fact>]
    let ``test annotate_bool_literal`` () =
        testParseFile @"tests\libminizinc\unit\compilation\annotate_bool_literal.mzn"

    [<Fact>]
    let ``test annotate_from_array`` () =
        testParseFile @"tests\libminizinc\unit\compilation\annotate_from_array.mzn"

    [<Fact>]
    let ``test assert_dbg_flag`` () =
        testParseFile @"tests\libminizinc\unit\compilation\assert_dbg_flag.mzn"

    [<Fact>]
    let ``test assert_dbg_ignore`` () =
        testParseFile @"tests\libminizinc\unit\compilation\assert_dbg_ignore.mzn"

    [<Fact>]
    let ``test call_root_ctx`` () =
        testParseFile @"tests\libminizinc\unit\compilation\call_root_ctx.mzn"

    [<Fact>]
    let ``test chain_compr_mult_clause`` () =
        testParseFile @"tests\libminizinc\unit\compilation\chain_compr_mult_clause.mzn"

    [<Fact>]
    let ``test count_rewrite`` () =
        testParseFile @"tests\libminizinc\unit\compilation\count_rewrite.mzn"

    [<Fact>]
    let ``test cv_domain`` () =
        testParseFile @"tests\libminizinc\unit\compilation\cv_domain.mzn"

    [<Fact>]
    let ``test debug_mode_false`` () =
        testParseFile @"tests\libminizinc\unit\compilation\debug_mode_false.mzn"

    [<Fact>]
    let ``test debug_mode_true`` () =
        testParseFile @"tests\libminizinc\unit\compilation\debug_mode_true.mzn"

    [<Fact>]
    let ``test defines_var_cycle_breaking`` () =
        testParseFile @"tests\libminizinc\unit\compilation\defines_var_cycle_breaking.mzn"

    [<Fact>]
    let ``test float_inf_range_dom`` () =
        testParseFile @"tests\libminizinc\unit\compilation\float_inf_range_dom.mzn"

    [<Fact>]
    let ``test has_ann`` () =
        testParseFile @"tests\libminizinc\unit\compilation\has_ann.mzn"

    [<Fact>]
    let ``test if_then_no_else`` () =
        testParseFile @"tests\libminizinc\unit\compilation\if_then_no_else.mzn"

    [<Fact>]
    let ``test implied_exists_chain`` () =
        testParseFile @"tests\libminizinc\unit\compilation\implied_exists_chain.mzn"

    [<Fact>]
    let ``test implied_hr`` () =
        testParseFile @"tests\libminizinc\unit\compilation\implied_hr.mzn"

    [<Fact>]
    let ``test int_inf_dom`` () =
        testParseFile @"tests\libminizinc\unit\compilation\int_inf_dom.mzn"

    [<Fact>]
    let ``test multiple_neg`` () =
        testParseFile @"tests\libminizinc\unit\compilation\multiple_neg.mzn"

    [<Fact>]
    let ``test optimization`` () =
        testParseFile @"tests\libminizinc\unit\compilation\optimization.mzn"

    [<Fact>]
    let ``test par_arg_out_of_bounds`` () =
        testParseFile @"tests\libminizinc\unit\compilation\par_arg_out_of_bounds.mzn"

    [<Fact>]
    let ``test poly_overload`` () =
        testParseFile @"tests\libminizinc\unit\compilation\poly_overload.mzn"

    [<Fact>]
    let ``test quoted_id_flatzinc`` () =
        testParseFile @"tests\libminizinc\unit\compilation\quoted_id_flatzinc.mzn"

    [<Fact>]
    let ``test time_limit`` () =
        testParseFile @"tests\libminizinc\unit\compilation\time_limit.mzn"

    [<Fact>]
    let ``test test_div1`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div1.mzn"

    [<Fact>]
    let ``test test_div10`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div10.mzn"

    [<Fact>]
    let ``test test_div11`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div11.mzn"

    [<Fact>]
    let ``test test_div12`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div12.mzn"

    [<Fact>]
    let ``test test_div2`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div2.mzn"

    [<Fact>]
    let ``test test_div3`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div3.mzn"

    [<Fact>]
    let ``test test_div4`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div4.mzn"

    [<Fact>]
    let ``test test_div5`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div5.mzn"

    [<Fact>]
    let ``test test_div6`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div6.mzn"

    [<Fact>]
    let ``test test_div7`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div7.mzn"

    [<Fact>]
    let ``test test_div8`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div8.mzn"

    [<Fact>]
    let ``test test_div9`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div9.mzn"

    [<Fact>]
    let ``test test_div_mod_bounds`` () =
        testParseFile @"tests\libminizinc\unit\division\test_div_mod_bounds.mzn"

    [<Fact>]
    let ``test test_fldiv_01`` () =
        testParseFile @"tests\libminizinc\unit\division\test_fldiv_01.mzn"

    [<Fact>]
    let ``test test_fldiv_02`` () =
        testParseFile @"tests\libminizinc\unit\division\test_fldiv_02.mzn"

    [<Fact>]
    let ``test test_lp_solve_satisfy`` () =
        testParseFile @"tests\libminizinc\unit\fdlp\test_lp_solve_satisfy.mzn"

    [<Fact>]
    let ``test anon_var_flatten`` () =
        testParseFile @"tests\libminizinc\unit\general\anon_var_flatten.mzn"

    [<Fact>]
    let ``test array_access_record_out_of_bounds`` () =
        testParseFile @"tests\libminizinc\unit\general\array_access_record_out_of_bounds.mzn"

    [<Fact>]
    let ``test array_access_tuple_out_of_bounds`` () =
        testParseFile @"tests\libminizinc\unit\general\array_access_tuple_out_of_bounds.mzn"

    [<Fact>]
    let ``test array_string_gen`` () =
        testParseFile @"tests\libminizinc\unit\general\array_string_gen.mzn"

    [<Fact>]
    let ``test assert_bad_1`` () =
        testParseFile @"tests\libminizinc\unit\general\assert_bad_1.mzn"

    [<Fact>]
    let ``test assert_bad_2`` () =
        testParseFile @"tests\libminizinc\unit\general\assert_bad_2.mzn"

    [<Fact>]
    let ``test assert_good`` () =
        testParseFile @"tests\libminizinc\unit\general\assert_good.mzn"

    [<Fact>]
    let ``test bin_pack_multiobj`` () =
        testParseFile @"tests\libminizinc\unit\general\bin_pack_multiobj.mzn"

    [<Fact>]
    let ``test builtins_arg_max`` () =
        testParseFile @"tests\libminizinc\unit\general\builtins_arg_max.mzn"

    [<Fact>]
    let ``test builtins_debug`` () =
        testParseFile @"tests\libminizinc\unit\general\builtins_debug.mzn"

    [<Fact>]
    let ``test comprehension_var_ub`` () =
        testParseFile @"tests\libminizinc\unit\general\comprehension_var_ub.mzn"

    [<Fact>]
    let ``test cse_ctx`` () =
        testParseFile @"tests\libminizinc\unit\general\cse_ctx.mzn"

    [<Fact>]
    let ``test enum_constructor_quoting`` () =
        testParseFile @"tests\libminizinc\unit\general\enum_constructor_quoting.mzn"

    [<Fact>]
    let ``test enum_order`` () =
        testParseFile @"tests\libminizinc\unit\general\enum_order.mzn"

    [<Fact>]
    let ``test enum_out_of_range_1`` () =
        testParseFile @"tests\libminizinc\unit\general\enum_out_of_range_1.mzn"

    [<Fact>]
    let ``test enum_out_of_range_2`` () =
        testParseFile @"tests\libminizinc\unit\general\enum_out_of_range_2.mzn"

    [<Fact>]
    let ``test function_param_out_of_range`` () =
        testParseFile @"tests\libminizinc\unit\general\function_param_out_of_range.mzn"

    [<Fact>]
    let ``test function_return_out_of_range`` () =
        testParseFile @"tests\libminizinc\unit\general\function_return_out_of_range.mzn"

    [<Fact>]
    let ``test function_return_out_of_range_opt`` () =
        testParseFile @"tests\libminizinc\unit\general\function_return_out_of_range_opt.mzn"

    [<Fact>]
    let ``test iffall_bv`` () =
        testParseFile @"tests\libminizinc\unit\general\iffall_bv.mzn"

    [<Fact>]
    let ``test infinite_domain_bind`` () =
        testParseFile @"tests\libminizinc\unit\general\infinite_domain_bind.mzn"

    [<Fact>]
    let ``test json_ignore`` () =
        testParseFile @"tests\libminizinc\unit\general\json_ignore.mzn"

    [<Fact>]
    let ``test md_exists`` () =
        testParseFile @"tests\libminizinc\unit\general\md_exists.mzn"

    [<Fact>]
    let ``test md_forall`` () =
        testParseFile @"tests\libminizinc\unit\general\md_forall.mzn"

    [<Fact>]
    let ``test md_iffall`` () =
        testParseFile @"tests\libminizinc\unit\general\md_iffall.mzn"

    [<Fact>]
    let ``test md_product_int`` () =
        testParseFile @"tests\libminizinc\unit\general\md_product_int.mzn"

    [<Fact>]
    let ``test md_sum_float`` () =
        testParseFile @"tests\libminizinc\unit\general\md_sum_float.mzn"

    [<Fact>]
    let ``test md_sum_int`` () =
        testParseFile @"tests\libminizinc\unit\general\md_sum_int.mzn"

    [<Fact>]
    let ``test md_xorall`` () =
        testParseFile @"tests\libminizinc\unit\general\md_xorall.mzn"

    [<Fact>]
    let ``test missing_ozn_decl`` () =
        testParseFile @"tests\libminizinc\unit\general\missing_ozn_decl.mzn"

    [<Fact>]
    let ``test mortgage`` () =
        testParseFile @"tests\libminizinc\unit\general\mortgage.mzn"

    [<Fact>]
    let ``test mzn-implicit1`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit1.mzn"

    [<Fact>]
    let ``test mzn-implicit2`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit2.mzn"

    [<Fact>]
    let ``test mzn-implicit3`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit3.mzn"

    [<Fact>]
    let ``test mzn-implicit4`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit4.mzn"

    [<Fact>]
    let ``test mzn-implicit5`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit5.mzn"

    [<Fact>]
    let ``test mzn-implicit6`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit6.mzn"

    [<Fact>]
    let ``test mzn-implicit7`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit7.mzn"

    [<Fact>]
    let ``test mzn-implicit8`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn-implicit8.mzn"

    [<Fact>]
    let ``test mzn_div`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn_div.mzn"

    [<Fact>]
    let ``test mzn_mod`` () =
        testParseFile @"tests\libminizinc\unit\general\mzn_mod.mzn"

    [<Fact>]
    let ``test overload_bottom`` () =
        testParseFile @"tests\libminizinc\unit\general\overload_bottom.mzn"

    [<Fact>]
    let ``test param_out_of_range_float`` () =
        testParseFile @"tests\libminizinc\unit\general\param_out_of_range_float.mzn"

    [<Fact>]
    let ``test param_out_of_range_int`` () =
        testParseFile @"tests\libminizinc\unit\general\param_out_of_range_int.mzn"

    [<Fact>]
    let ``test pow_1`` () =
        testParseFile @"tests\libminizinc\unit\general\pow_1.mzn"

    [<Fact>]
    let ``test pow_2`` () =
        testParseFile @"tests\libminizinc\unit\general\pow_2.mzn"

    [<Fact>]
    let ``test pow_3`` () =
        testParseFile @"tests\libminizinc\unit\general\pow_3.mzn"

    [<Fact>]
    let ``test pow_4`` () =
        testParseFile @"tests\libminizinc\unit\general\pow_4.mzn"

    [<Fact>]
    let ``test pow_bounds`` () =
        testParseFile @"tests\libminizinc\unit\general\pow_bounds.mzn"

    [<Fact>]
    let ``test quoted_id_1`` () =
        testParseFile @"tests\libminizinc\unit\general\quoted_id_1.mzn"

    [<Fact>]
    let ``test quoted_id_2`` () =
        testParseFile @"tests\libminizinc\unit\general\quoted_id_2.mzn"

    [<Fact>]
    let ``test quoted_id_3`` () =
        testParseFile @"tests\libminizinc\unit\general\quoted_id_3.mzn"

    [<Fact>]
    let ``test quoted_id_4`` () =
        testParseFile @"tests\libminizinc\unit\general\quoted_id_4.mzn"

    [<Fact>]
    let ``test stack_overflow`` () =
        testParseFile @"tests\libminizinc\unit\general\stack_overflow.mzn"

    [<Fact>]
    let ``test test-search1`` () =
        testParseFile @"tests\libminizinc\unit\general\test-search1.mzn"

    [<Fact>]
    let ``test test_array1`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array1.mzn"

    [<Fact>]
    let ``test test_array1and2d`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array1and2d.mzn"

    [<Fact>]
    let ``test test_array1d_bad_1`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array1d_bad_1.mzn"

    [<Fact>]
    let ``test test_array1d_bad_2`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array1d_bad_2.mzn"

    [<Fact>]
    let ``test test_array1d_bad_3`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array1d_bad_3.mzn"

    [<Fact>]
    let ``test test_array1d_bad_4`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array1d_bad_4.mzn"

    [<Fact>]
    let ``test test_array2`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array2.mzn"

    [<Fact>]
    let ``test test_array_as_generator`` () =
        testParseFile @"tests\libminizinc\unit\general\test_array_as_generator.mzn"

    [<Fact>]
    let ``test test_bad_array_size-bad`` () =
        testParseFile @"tests\libminizinc\unit\general\test_bad_array_size-bad.mzn"

    [<Fact>]
    let ``test test_bad_lb_ub_dom-bad`` () =
        testParseFile @"tests\libminizinc\unit\general\test_bad_lb_ub_dom-bad.mzn"

    [<Fact>]
    let ``test test_bool_var_array_access`` () =
        testParseFile @"tests\libminizinc\unit\general\test_bool_var_array_access.mzn"

    [<Fact>]
    let ``test test_concat1`` () =
        testParseFile @"tests\libminizinc\unit\general\test_concat1.mzn"

    [<Fact>]
    let ``test test_concat2`` () =
        testParseFile @"tests\libminizinc\unit\general\test_concat2.mzn"

    [<Fact>]
    let ``test test_concat3`` () =
        testParseFile @"tests\libminizinc\unit\general\test_concat3.mzn"

    [<Fact>]
    let ``test test_empty_enum`` () =
        testParseFile @"tests\libminizinc\unit\general\test_empty_enum.mzn"

    [<Fact>]
    let ``test test_is_fixed`` () =
        testParseFile @"tests\libminizinc\unit\general\test_is_fixed.mzn"

    [<Fact>]
    let ``test test_join1`` () =
        testParseFile @"tests\libminizinc\unit\general\test_join1.mzn"

    [<Fact>]
    let ``test test_join2`` () =
        testParseFile @"tests\libminizinc\unit\general\test_join2.mzn"

    [<Fact>]
    let ``test test_join3`` () =
        testParseFile @"tests\libminizinc\unit\general\test_join3.mzn"

    [<Fact>]
    let ``test test_lb_ub_array_int`` () =
        testParseFile @"tests\libminizinc\unit\general\test_lb_ub_array_int.mzn"

    [<Fact>]
    let ``test test_lb_ub_dom_int`` () =
        testParseFile @"tests\libminizinc\unit\general\test_lb_ub_dom_int.mzn"

    [<Fact>]
    let ``test test_lb_ub_float`` () =
        testParseFile @"tests\libminizinc\unit\general\test_lb_ub_float.mzn"

    [<Fact>]
    let ``test test_let_complex`` () =
        testParseFile @"tests\libminizinc\unit\general\test_let_complex.mzn"

    [<Fact>]
    let ``test test_let_par_array`` () =
        testParseFile @"tests\libminizinc\unit\general\test_let_par_array.mzn"

    [<Fact>]
    let ``test test_let_simple`` () =
        testParseFile @"tests\libminizinc\unit\general\test_let_simple.mzn"

    [<Fact>]
    let ``test test_let_var_array`` () =
        testParseFile @"tests\libminizinc\unit\general\test_let_var_array.mzn"

    [<Fact>]
    let ``test test_let_with_annotation`` () =
        testParseFile @"tests\libminizinc\unit\general\test_let_with_annotation.mzn"

    [<Fact>]
    let ``test test_min_var_array`` () =
        testParseFile @"tests\libminizinc\unit\general\test_min_var_array.mzn"

    [<Fact>]
    let ``test test_negated_and`` () =
        testParseFile @"tests\libminizinc\unit\general\test_negated_and.mzn"

    [<Fact>]
    let ``test test_negated_and_or`` () =
        testParseFile @"tests\libminizinc\unit\general\test_negated_and_or.mzn"

    [<Fact>]
    let ``test test_negated_let_bad`` () =
        testParseFile @"tests\libminizinc\unit\general\test_negated_let_bad.mzn"

    [<Fact>]
    let ``test test_negated_let_good`` () =
        testParseFile @"tests\libminizinc\unit\general\test_negated_let_good.mzn"

    [<Fact>]
    let ``test test_negated_let_good_2`` () =
        testParseFile @"tests\libminizinc\unit\general\test_negated_let_good_2.mzn"

    [<Fact>]
    let ``test test_negated_or`` () =
        testParseFile @"tests\libminizinc\unit\general\test_negated_or.mzn"

    [<Fact>]
    let ``test test_par_set_element`` () =
        testParseFile @"tests\libminizinc\unit\general\test_par_set_element.mzn"

    [<Fact>]
    let ``test test_par_set_id_array_index_sets`` () =
        testParseFile @"tests\libminizinc\unit\general\test_par_set_id_array_index_sets.mzn"

    [<Fact>]
    let ``test test_queens`` () =
        testParseFile @"tests\libminizinc\unit\general\test_queens.mzn"

    [<Fact>]
    let ``test test_reified_element_constraint`` () =
        testParseFile @"tests\libminizinc\unit\general\test_reified_element_constraint.mzn"

    [<Fact>]
    let ``test test_reified_let_bad`` () =
        testParseFile @"tests\libminizinc\unit\general\test_reified_let_bad.mzn"

    [<Fact>]
    let ``test test_reified_let_good`` () =
        testParseFile @"tests\libminizinc\unit\general\test_reified_let_good.mzn"

    [<Fact>]
    let ``test test_rounding_a`` () =
        testParseFile @"tests\libminizinc\unit\general\test_rounding_a.mzn"

    [<Fact>]
    let ``test test_rounding_b`` () =
        testParseFile @"tests\libminizinc\unit\general\test_rounding_b.mzn"

    [<Fact>]
    let ``test test_rounding_c`` () =
        testParseFile @"tests\libminizinc\unit\general\test_rounding_c.mzn"

    [<Fact>]
    let ``test test_same`` () =
        testParseFile @"tests\libminizinc\unit\general\test_same.mzn"

    [<Fact>]
    let ``test test_set_inequality_par`` () =
        testParseFile @"tests\libminizinc\unit\general\test_set_inequality_par.mzn"

    [<Fact>]
    let ``test test_set_lt_1`` () =
        testParseFile @"tests\libminizinc\unit\general\test_set_lt_1.mzn"

    [<Fact>]
    let ``test test_set_lt_2`` () =
        testParseFile @"tests\libminizinc\unit\general\test_set_lt_2.mzn"

    [<Fact>]
    let ``test test_set_lt_3`` () =
        testParseFile @"tests\libminizinc\unit\general\test_set_lt_3.mzn"

    [<Fact>]
    let ``test test_string_array_var`` () =
        testParseFile @"tests\libminizinc\unit\general\test_string_array_var.mzn"

    [<Fact>]
    let ``test test_string_cmp`` () =
        testParseFile @"tests\libminizinc\unit\general\test_string_cmp.mzn"

    [<Fact>]
    let ``test test_string_var`` () =
        testParseFile @"tests\libminizinc\unit\general\test_string_var.mzn"

    [<Fact>]
    let ``test test_string_with_quote`` () =
        testParseFile @"tests\libminizinc\unit\general\test_string_with_quote.mzn"

    [<Fact>]
    let ``test test_times_int_float_eq`` () =
        testParseFile @"tests\libminizinc\unit\general\test_times_int_float_eq.mzn"

    [<Fact>]
    let ``test test_times_int_float_eq__defaultopt`` () =
        testParseFile @"tests\libminizinc\unit\general\test_times_int_float_eq__defaultopt.mzn"

    [<Fact>]
    let ``test test_to_enum`` () =
        testParseFile @"tests\libminizinc\unit\general\test_to_enum.mzn"

    [<Fact>]
    let ``test test_undefined_enum`` () =
        testParseFile @"tests\libminizinc\unit\general\test_undefined_enum.mzn"

    [<Fact>]
    let ``test test_var_array`` () =
        testParseFile @"tests\libminizinc\unit\general\test_var_array.mzn"

    [<Fact>]
    let ``test test_var_array_access`` () =
        testParseFile @"tests\libminizinc\unit\general\test_var_array_access.mzn"

    [<Fact>]
    let ``test test_var_prod`` () =
        testParseFile @"tests\libminizinc\unit\general\test_var_prod.mzn"

    [<Fact>]
    let ``test test_var_set_assignment`` () =
        testParseFile @"tests\libminizinc\unit\general\test_var_set_assignment.mzn"

    [<Fact>]
    let ``test test_var_set_element`` () =
        testParseFile @"tests\libminizinc\unit\general\test_var_set_element.mzn"

    [<Fact>]
    let ``test unicode_file_name_μ`` () =
        testParseFile @"tests\libminizinc\unit\general\unicode_file_name_μ.mzn"

    [<Fact>]
    let ``test wolfgoatetc`` () =
        testParseFile @"tests\libminizinc\unit\general\wolfgoatetc.mzn"

    [<Fact>]
    let ``test xorall_bv`` () =
        testParseFile @"tests\libminizinc\unit\general\xorall_bv.mzn"

    [<Fact>]
    let ``test anon_enum_json`` () =
        testParseFile @"tests\libminizinc\unit\json\anon_enum_json.mzn"

    [<Fact>]
    let ``test coerce_enum_str`` () =
        testParseFile @"tests\libminizinc\unit\json\coerce_enum_str.mzn"

    [<Fact>]
    let ``test coerce_enum_str_err`` () =
        testParseFile @"tests\libminizinc\unit\json\coerce_enum_str_err.mzn"

    [<Fact>]
    let ``test coerce_indices`` () =
        testParseFile @"tests\libminizinc\unit\json\coerce_indices.mzn"

    [<Fact>]
    let ``test coerce_set`` () =
        testParseFile @"tests\libminizinc\unit\json\coerce_set.mzn"

    [<Fact>]
    let ``test enum_array_xd`` () =
        testParseFile @"tests\libminizinc\unit\json\enum_array_xd.mzn"

    [<Fact>]
    let ``test enum_constructor_basic`` () =
        testParseFile @"tests\libminizinc\unit\json\enum_constructor_basic.mzn"

    [<Fact>]
    let ``test enum_constructor_basic_2`` () =
        testParseFile @"tests\libminizinc\unit\json\enum_constructor_basic_2.mzn"

    [<Fact>]
    let ``test enum_constructor_int`` () =
        testParseFile @"tests\libminizinc\unit\json\enum_constructor_int.mzn"

    [<Fact>]
    let ``test enum_constructor_nested`` () =
        testParseFile @"tests\libminizinc\unit\json\enum_constructor_nested.mzn"

    [<Fact>]
    let ``test enum_escaping`` () =
        testParseFile @"tests\libminizinc\unit\json\enum_escaping.mzn"

    [<Fact>]
    let ``test float_json_exponent`` () =
        testParseFile @"tests\libminizinc\unit\json\float_json_exponent.mzn"

    [<Fact>]
    let ``test json_array2d_set`` () =
        testParseFile @"tests\libminizinc\unit\json\json_array2d_set.mzn"

    [<Fact>]
    let ``test json_enum_def`` () =
        testParseFile @"tests\libminizinc\unit\json\json_enum_def.mzn"

    [<Fact>]
    let ``test json_input_1`` () =
        testParseFile @"tests\libminizinc\unit\json\json_input_1.mzn"

    [<Fact>]
    let ``test json_input_multidim_enum_str`` () =
        testParseFile @"tests\libminizinc\unit\json\json_input_multidim_enum_str.mzn"

    [<Fact>]
    let ``test json_input_set`` () =
        testParseFile @"tests\libminizinc\unit\json\json_input_set.mzn"

    [<Fact>]
    let ``test json_input_str`` () =
        testParseFile @"tests\libminizinc\unit\json\json_input_str.mzn"

    [<Fact>]
    let ``test json_unicode_escapes`` () =
        testParseFile @"tests\libminizinc\unit\json\json_unicode_escapes.mzn"

    [<Fact>]
    let ``test mult_dim_enum`` () =
        testParseFile @"tests\libminizinc\unit\json\mult_dim_enum.mzn"

    [<Fact>]
    let ``test record_json_input`` () =
        testParseFile @"tests\libminizinc\unit\json\record_json_input.mzn"

    [<Fact>]
    let ``test tuple_json_input`` () =
        testParseFile @"tests\libminizinc\unit\json\tuple_json_input.mzn"

    [<Fact>]
    let ``test conj_absent_1`` () =
        testParseFile @"tests\libminizinc\unit\optional\conj_absent_1.mzn"

    [<Fact>]
    let ``test conj_absent_2`` () =
        testParseFile @"tests\libminizinc\unit\optional\conj_absent_2.mzn"

    [<Fact>]
    let ``test test-opt-binop-flatten`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-binop-flatten.mzn"

    [<Fact>]
    let ``test test-opt-bool-1`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-bool-1.mzn"

    [<Fact>]
    let ``test test-opt-bool-2`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-bool-2.mzn"

    [<Fact>]
    let ``test test-opt-bool-3`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-bool-3.mzn"

    [<Fact>]
    let ``test test-opt-bool-4`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-bool-4.mzn"

    [<Fact>]
    let ``test test-opt-bool-5`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-bool-5.mzn"

    [<Fact>]
    let ``test test-opt-bool-6`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-bool-6.mzn"

    [<Fact>]
    let ``test test-opt-compute-bounds`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-compute-bounds.mzn"

    [<Fact>]
    let ``test test-opt-float-1`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-float-1.mzn"

    [<Fact>]
    let ``test test-opt-float-2`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-float-2.mzn"

    [<Fact>]
    let ``test test-opt-if-then-else`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-if-then-else.mzn"

    [<Fact>]
    let ``test test-opt-int-1`` () =
        testParseFile @"tests\libminizinc\unit\optional\test-opt-int-1.mzn"

    [<Fact>]
    let ``test test_bug_456`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_bug_456.mzn"

    [<Fact>]
    let ``test test_count_set`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_count_set.mzn"

    [<Fact>]
    let ``test test_deopt_absent`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_deopt_absent.mzn"

    [<Fact>]
    let ``test test_if_then_else_opt_bool`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_if_then_else_opt_bool.mzn"

    [<Fact>]
    let ``test test_if_then_else_opt_float`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_if_then_else_opt_float.mzn"

    [<Fact>]
    let ``test test_if_then_else_opt_int`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_if_then_else_opt_int.mzn"

    [<Fact>]
    let ``test test_if_then_else_var_opt_bool`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_if_then_else_var_opt_bool.mzn"

    [<Fact>]
    let ``test test_if_then_else_var_opt_float`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_if_then_else_var_opt_float.mzn"

    [<Fact>]
    let ``test test_if_then_else_var_opt_int`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_if_then_else_var_opt_int.mzn"

    [<Fact>]
    let ``test test_optional_not_absent`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_optional_not_absent.mzn"

    [<Fact>]
    let ``test test_opt_comprehension`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_opt_comprehension.mzn"

    [<Fact>]
    let ``test test_opt_dom_empty`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_opt_dom_empty.mzn"

    [<Fact>]
    let ``test test_opt_dom_empty_no_absent_zero`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_opt_dom_empty_no_absent_zero.mzn"

    [<Fact>]
    let ``test test_opt_max`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_opt_max.mzn"

    [<Fact>]
    let ``test test_opt_min`` () =
        testParseFile @"tests\libminizinc\unit\optional\test_opt_min.mzn"

    [<Fact>]
    let ``test arg-reif-output`` () =
        testParseFile @"tests\libminizinc\unit\output\arg-reif-output.mzn"

    [<Fact>]
    let ``test array-ann`` () =
        testParseFile @"tests\libminizinc\unit\output\array-ann.mzn"

    [<Fact>]
    let ``test bug288a`` () =
        testParseFile @"tests\libminizinc\unit\output\bug288a.mzn"

    [<Fact>]
    let ``test bug288b`` () =
        testParseFile @"tests\libminizinc\unit\output\bug288b.mzn"

    [<Fact>]
    let ``test bug288c`` () =
        testParseFile @"tests\libminizinc\unit\output\bug288c.mzn"

    [<Fact>]
    let ``test ctx_ann`` () =
        testParseFile @"tests\libminizinc\unit\output\ctx_ann.mzn"

    [<Fact>]
    let ``test dzn_output_array`` () =
        testParseFile @"tests\libminizinc\unit\output\dzn_output_array.mzn"

    [<Fact>]
    let ``test json_ann`` () =
        testParseFile @"tests\libminizinc\unit\output\json_ann.mzn"

    [<Fact>]
    let ``test mzn_bottom1`` () =
        testParseFile @"tests\libminizinc\unit\output\mzn_bottom1.mzn"

    [<Fact>]
    let ``test mzn_bottom2`` () =
        testParseFile @"tests\libminizinc\unit\output\mzn_bottom2.mzn"

    [<Fact>]
    let ``test mzn_bottom3`` () =
        testParseFile @"tests\libminizinc\unit\output\mzn_bottom3.mzn"

    [<Fact>]
    let ``test mzn_bottom4`` () =
        testParseFile @"tests\libminizinc\unit\output\mzn_bottom4.mzn"

    [<Fact>]
    let ``test mzn_bottom5`` () =
        testParseFile @"tests\libminizinc\unit\output\mzn_bottom5.mzn"

    [<Fact>]
    let ``test mzn_bottom6`` () =
        testParseFile @"tests\libminizinc\unit\output\mzn_bottom6.mzn"

    [<Fact>]
    let ``test output_annotations_1`` () =
        testParseFile @"tests\libminizinc\unit\output\output_annotations_1.mzn"

    [<Fact>]
    let ``test output_annotations_2`` () =
        testParseFile @"tests\libminizinc\unit\output\output_annotations_2.mzn"

    [<Fact>]
    let ``test output_annotations_3`` () =
        testParseFile @"tests\libminizinc\unit\output\output_annotations_3.mzn"

    [<Fact>]
    let ``test output_annotations_4`` () =
        testParseFile @"tests\libminizinc\unit\output\output_annotations_4.mzn"

    [<Fact>]
    let ``test output_sections_1`` () =
        testParseFile @"tests\libminizinc\unit\output\output_sections_1.mzn"

    [<Fact>]
    let ``test output_sections_2`` () =
        testParseFile @"tests\libminizinc\unit\output\output_sections_2.mzn"

    [<Fact>]
    let ``test output_sections_3`` () =
        testParseFile @"tests\libminizinc\unit\output\output_sections_3.mzn"

    [<Fact>]
    let ``test output_sections_4`` () =
        testParseFile @"tests\libminizinc\unit\output\output_sections_4.mzn"

    [<Fact>]
    let ``test output_sections_5`` () =
        testParseFile @"tests\libminizinc\unit\output\output_sections_5.mzn"

    [<Fact>]
    let ``test output_sections_6`` () =
        testParseFile @"tests\libminizinc\unit\output\output_sections_6.mzn"

    [<Fact>]
    let ``test quoted_id_ozn`` () =
        testParseFile @"tests\libminizinc\unit\output\quoted_id_ozn.mzn"

    [<Fact>]
    let ``test show2d_empty`` () =
        testParseFile @"tests\libminizinc\unit\output\show2d_empty.mzn"

    [<Fact>]
    let ``test show3d_empty`` () =
        testParseFile @"tests\libminizinc\unit\output\show3d_empty.mzn"

    [<Fact>]
    let ``test show_empty`` () =
        testParseFile @"tests\libminizinc\unit\output\show_empty.mzn"

    [<Fact>]
    let ``test show_float`` () =
        testParseFile @"tests\libminizinc\unit\output\show_float.mzn"

    [<Fact>]
    let ``test show_float_set`` () =
        testParseFile @"tests\libminizinc\unit\output\show_float_set.mzn"

    [<Fact>]
    let ``test show_int`` () =
        testParseFile @"tests\libminizinc\unit\output\show_int.mzn"

    [<Fact>]
    let ``test show_int_set`` () =
        testParseFile @"tests\libminizinc\unit\output\show_int_set.mzn"

    [<Fact>]
    let ``test test-in-output`` () =
        testParseFile @"tests\libminizinc\unit\output\test-in-output.mzn"

    [<Fact>]
    let ``test var_enum`` () =
        testParseFile @"tests\libminizinc\unit\output\var_enum.mzn"

    [<Fact>]
    let ``test very_empty`` () =
        testParseFile @"tests\libminizinc\unit\output\very_empty.mzn"

    [<Fact>]
    let ``test very_empty_set`` () =
        testParseFile @"tests\libminizinc\unit\output\very_empty_set.mzn"

    [<Fact>]
    let ``test param_file_array`` () =
        testParseFile @"tests\libminizinc\unit\param_file\param_file_array.mzn"

    [<Fact>]
    let ``test param_file_blacklist`` () =
        testParseFile @"tests\libminizinc\unit\param_file\param_file_blacklist.mzn"

    [<Fact>]
    let ``test param_file_nested_object`` () =
        testParseFile @"tests\libminizinc\unit\param_file\param_file_nested_object.mzn"

    [<Fact>]
    let ``test param_file_recursive`` () =
        testParseFile @"tests\libminizinc\unit\param_file\param_file_recursive.mzn"

    [<Fact>]
    let ``test param_file_resolution`` () =
        testParseFile @"tests\libminizinc\unit\param_file\param_file_resolution.mzn"

    [<Fact>]
    let ``test absent_id_crash`` () =
        testParseFile @"tests\libminizinc\unit\regression\absent_id_crash.mzn"

    [<Fact>]
    let ``test abs_bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\abs_bug.mzn"

    [<Fact>]
    let ``test arg-reif-array-float`` () =
        testParseFile @"tests\libminizinc\unit\regression\arg-reif-array-float.mzn"

    [<Fact>]
    let ``test arg-reif-array-int`` () =
        testParseFile @"tests\libminizinc\unit\regression\arg-reif-array-int.mzn"

    [<Fact>]
    let ``test arg-reif-float`` () =
        testParseFile @"tests\libminizinc\unit\regression\arg-reif-float.mzn"

    [<Fact>]
    let ``test arg-reif-int-set`` () =
        testParseFile @"tests\libminizinc\unit\regression\arg-reif-int-set.mzn"

    [<Fact>]
    let ``test arg-reif-int`` () =
        testParseFile @"tests\libminizinc\unit\regression\arg-reif-int.mzn"

    [<Fact>]
    let ``test array_of_empty_sets`` () =
        testParseFile @"tests\libminizinc\unit\regression\array_of_empty_sets.mzn"

    [<Fact>]
    let ``test array_set_element_nosets`` () =
        testParseFile @"tests\libminizinc\unit\regression\array_set_element_nosets.mzn"

    [<Fact>]
    let ``test array_var_set_element_nosets`` () =
        testParseFile @"tests\libminizinc\unit\regression\array_var_set_element_nosets.mzn"

    [<Fact>]
    let ``test assign_reverse_map`` () =
        testParseFile @"tests\libminizinc\unit\regression\assign_reverse_map.mzn"

    [<Fact>]
    let ``test big_array_lit`` () =
        testParseFile @"tests\libminizinc\unit\regression\big_array_lit.mzn"

    [<Fact>]
    let ``test bind-defines-var`` () =
        testParseFile @"tests\libminizinc\unit\regression\bind-defines-var.mzn"

    [<Fact>]
    let ``test binop_mult_gclock`` () =
        testParseFile @"tests\libminizinc\unit\regression\binop_mult_gclock.mzn"

    [<Fact>]
    let ``test bizarre`` () =
        testParseFile @"tests\libminizinc\unit\regression\bizarre.mzn"

    [<Fact>]
    let ``test bool2float_let`` () =
        testParseFile @"tests\libminizinc\unit\regression\bool2float_let.mzn"

    [<Fact>]
    let ``test bounds_for_linear_01_max_0`` () =
        testParseFile @"tests\libminizinc\unit\regression\bounds_for_linear_01_max_0.mzn"

    [<Fact>]
    let ``test bounds_for_linear_01_max_1`` () =
        testParseFile @"tests\libminizinc\unit\regression\bounds_for_linear_01_max_1.mzn"

    [<Fact>]
    let ``test bounds_for_linear_01_min_0`` () =
        testParseFile @"tests\libminizinc\unit\regression\bounds_for_linear_01_min_0.mzn"

    [<Fact>]
    let ``test bounds_for_linear_01_min_1`` () =
        testParseFile @"tests\libminizinc\unit\regression\bounds_for_linear_01_min_1.mzn"

    [<Fact>]
    let ``test bug109`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug109.mzn"

    [<Fact>]
    let ``test bug110`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug110.mzn"

    [<Fact>]
    let ``test bug131`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug131.mzn"

    [<Fact>]
    let ``test bug141`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug141.mzn"

    [<Fact>]
    let ``test bug202`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug202.mzn"

    [<Fact>]
    let ``test bug212`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug212.mzn"

    [<Fact>]
    let ``test bug222`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug222.mzn"

    [<Fact>]
    let ``test bug244`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug244.mzn"

    [<Fact>]
    let ``test bug256`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug256.mzn"

    [<Fact>]
    let ``test bug256b`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug256b.mzn"

    [<Fact>]
    let ``test bug259`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug259.mzn"

    [<Fact>]
    let ``test bug269`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug269.mzn"

    [<Fact>]
    let ``test bug282`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug282.mzn"

    [<Fact>]
    let ``test bug283`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug283.mzn"

    [<Fact>]
    let ``test bug284`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug284.mzn"

    [<Fact>]
    let ``test bug287`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug287.mzn"

    [<Fact>]
    let ``test bug290_orig`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug290_orig.mzn"

    [<Fact>]
    let ``test bug290_simple`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug290_simple.mzn"

    [<Fact>]
    let ``test bug312`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug312.mzn"

    [<Fact>]
    let ``test bug318_orig`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug318_orig.mzn"

    [<Fact>]
    let ``test bug335`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug335.mzn"

    [<Fact>]
    let ``test bug337`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug337.mzn"

    [<Fact>]
    let ``test bug337_mod`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug337_mod.mzn"

    [<Fact>]
    let ``test bug341`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug341.mzn"

    [<Fact>]
    let ``test bug347`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug347.mzn"

    [<Fact>]
    let ``test bug380`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug380.mzn"

    [<Fact>]
    let ``test bug45`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug45.mzn"

    [<Fact>]
    let ``test bug52`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug52.mzn"

    [<Fact>]
    let ``test bug532`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug532.mzn"

    [<Fact>]
    let ``test bug534`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug534.mzn"

    [<Fact>]
    let ``test bug536`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug536.mzn"

    [<Fact>]
    let ``test bug552`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug552.mzn"

    [<Fact>]
    let ``test bug565`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug565.mzn"

    [<Fact>]
    let ``test bug570`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug570.mzn"

    [<Fact>]
    let ``test bug620`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug620.mzn"

    [<Fact>]
    let ``test bug635`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug635.mzn"

    [<Fact>]
    let ``test bug67`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug67.mzn"

    [<Fact>]
    let ``test bug68b`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug68b.mzn"

    [<Fact>]
    let ``test bug69_1`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug69_1.mzn"

    [<Fact>]
    let ``test bug69_2`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug69_2.mzn"

    [<Fact>]
    let ``test bug69_3`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug69_3.mzn"

    [<Fact>]
    let ``test bug69_4`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug69_4.mzn"

    [<Fact>]
    let ``test bug70`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug70.mzn"

    [<Fact>]
    let ``test bug71_1`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug71_1.mzn"

    [<Fact>]
    let ``test bug71_2`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug71_2.mzn"

    [<Fact>]
    let ``test bug82`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug82.mzn"

    [<Fact>]
    let ``test bug85`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug85.mzn"

    [<Fact>]
    let ``test bug_08`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug_08.mzn"

    [<Fact>]
    let ``test bug_629`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug_629.mzn"

    [<Fact>]
    let ``test bug_empty_enum_extension`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug_empty_enum_extension.mzn"

    [<Fact>]
    let ``test bug_opt_polymorphic`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug_opt_polymorphic.mzn"

    [<Fact>]
    let ``test bug_r7995`` () =
        testParseFile @"tests\libminizinc\unit\regression\bug_r7995.mzn"

    [<Fact>]
    let ``test cardinality_atmost_partition`` () =
        testParseFile @"tests\libminizinc\unit\regression\cardinality_atmost_partition.mzn"

    [<Fact>]
    let ``test card_flatten_lb`` () =
        testParseFile @"tests\libminizinc\unit\regression\card_flatten_lb.mzn"

    [<Fact>]
    let ``test change_partition`` () =
        testParseFile @"tests\libminizinc\unit\regression\change_partition.mzn"

    [<Fact>]
    let ``test checker_mzn_check_var`` () =
        testParseFile @"tests\libminizinc\unit\regression\checker_mzn_check_var.mzn"

    [<Fact>]
    let ``test checker_opt`` () =
        testParseFile @"tests\libminizinc\unit\regression\checker_opt.mzn"

    [<Fact>]
    let ``test checker_params`` () =
        testParseFile @"tests\libminizinc\unit\regression\checker_params.mzn"

    [<Fact>]
    let ``test checker_same_var`` () =
        testParseFile @"tests\libminizinc\unit\regression\checker_same_var.mzn"

    [<Fact>]
    let ``test checker_var_bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\checker_var_bug.mzn"

    [<Fact>]
    let ``test check_dom_float_array`` () =
        testParseFile @"tests\libminizinc\unit\regression\check_dom_float_array.mzn"

    [<Fact>]
    let ``test coerce_set_to_array_1`` () =
        testParseFile @"tests\libminizinc\unit\regression\coerce_set_to_array_1.mzn"

    [<Fact>]
    let ``test coerce_set_to_array_2`` () =
        testParseFile @"tests\libminizinc\unit\regression\coerce_set_to_array_2.mzn"

    [<Fact>]
    let ``test coercion_par`` () =
        testParseFile @"tests\libminizinc\unit\regression\coercion_par.mzn"

    [<Fact>]
    let ``test comprehension_where`` () =
        testParseFile @"tests\libminizinc\unit\regression\comprehension_where.mzn"

    [<Fact>]
    let ``test comp_in_empty_set`` () =
        testParseFile @"tests\libminizinc\unit\regression\comp_in_empty_set.mzn"

    [<Fact>]
    let ``test constructor_of_set`` () =
        testParseFile @"tests\libminizinc\unit\regression\constructor_of_set.mzn"

    [<Fact>]
    let ``test cse_array_lit`` () =
        testParseFile @"tests\libminizinc\unit\regression\cse_array_lit.mzn"

    [<Fact>]
    let ``test cyclic_include`` () =
        testParseFile @"tests\libminizinc\unit\regression\cyclic_include.mzn"

    [<Fact>]
    let ``test decision_tree_binary`` () =
        testParseFile @"tests\libminizinc\unit\regression\decision_tree_binary.mzn"

    [<Fact>]
    let ``test empty-array1d`` () =
        testParseFile @"tests\libminizinc\unit\regression\empty-array1d.mzn"

    [<Fact>]
    let ``test enigma_1568`` () =
        testParseFile @"tests\libminizinc\unit\regression\enigma_1568.mzn"

    [<Fact>]
    let ``test error_in_comprehension`` () =
        testParseFile @"tests\libminizinc\unit\regression\error_in_comprehension.mzn"

    [<Fact>]
    let ``test flatten_comp_in`` () =
        testParseFile @"tests\libminizinc\unit\regression\flatten_comp_in.mzn"

    [<Fact>]
    let ``test flatten_comp_in2`` () =
        testParseFile @"tests\libminizinc\unit\regression\flatten_comp_in2.mzn"

    [<Fact>]
    let ``test flat_cv_call`` () =
        testParseFile @"tests\libminizinc\unit\regression\flat_cv_call.mzn"

    [<Fact>]
    let ``test flat_cv_let`` () =
        testParseFile @"tests\libminizinc\unit\regression\flat_cv_let.mzn"

    [<Fact>]
    let ``test flat_set_lit`` () =
        testParseFile @"tests\libminizinc\unit\regression\flat_set_lit.mzn"

    [<Fact>]
    let ``test flipstrip_simple`` () =
        testParseFile @"tests\libminizinc\unit\regression\flipstrip_simple.mzn"

    [<Fact>]
    let ``test float_ceil_floor`` () =
        testParseFile @"tests\libminizinc\unit\regression\float_ceil_floor.mzn"

    [<Fact>]
    let ``test float_div_crash`` () =
        testParseFile @"tests\libminizinc\unit\regression\float_div_crash.mzn"

    [<Fact>]
    let ``test float_mod_crash`` () =
        testParseFile @"tests\libminizinc\unit\regression\float_mod_crash.mzn"

    [<Fact>]
    let ``test float_opt_crash`` () =
        testParseFile @"tests\libminizinc\unit\regression\float_opt_crash.mzn"

    [<Fact>]
    let ``test follow_id_absent_crash`` () =
        testParseFile @"tests\libminizinc\unit\regression\follow_id_absent_crash.mzn"

    [<Fact>]
    let ``test github537`` () =
        testParseFile @"tests\libminizinc\unit\regression\github537.mzn"

    [<Fact>]
    let ``test github_638_reduced`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_638_reduced.mzn"

    [<Fact>]
    let ``test github_639_part1`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_639_part1.mzn"

    [<Fact>]
    let ``test github_639_part2`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_639_part2.mzn"

    [<Fact>]
    let ``test github_644_a`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_644_a.mzn"

    [<Fact>]
    let ``test github_644_b`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_644_b.mzn"

    [<Fact>]
    let ``test github_644_c`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_644_c.mzn"

    [<Fact>]
    let ``test github_644_d`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_644_d.mzn"

    [<Fact>]
    let ``test github_644_e`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_644_e.mzn"

    [<Fact>]
    let ``test github_646`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_646.mzn"

    [<Fact>]
    let ``test github_648_par_array_decl`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_648_par_array_decl.mzn"

    [<Fact>]
    let ``test github_648_par_decl`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_648_par_decl.mzn"

    [<Fact>]
    let ``test github_656`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_656.mzn"

    [<Fact>]
    let ``test github_660a`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_660a.mzn"

    [<Fact>]
    let ``test github_660b`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_660b.mzn"

    [<Fact>]
    let ``test github_661_part1`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_661_part1.mzn"

    [<Fact>]
    let ``test github_661_part2`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_661_part2.mzn"

    [<Fact>]
    let ``test github_664`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_664.mzn"

    [<Fact>]
    let ``test github_666`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_666.mzn"

    [<Fact>]
    let ``test github_667`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_667.mzn"

    [<Fact>]
    let ``test github_668`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_668.mzn"

    [<Fact>]
    let ``test github_669`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_669.mzn"

    [<Fact>]
    let ``test github_670`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_670.mzn"

    [<Fact>]
    let ``test github_671`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_671.mzn"

    [<Fact>]
    let ``test github_673`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_673.mzn"

    [<Fact>]
    let ``test github_674`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_674.mzn"

    [<Fact>]
    let ``test github_675a`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_675a.mzn"

    [<Fact>]
    let ``test github_675b`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_675b.mzn"

    [<Fact>]
    let ``test github_680`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_680.mzn"

    [<Fact>]
    let ``test github_681`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_681.mzn"

    [<Fact>]
    let ``test github_683`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_683.mzn"

    [<Fact>]
    let ``test github_685`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_685.mzn"

    [<Fact>]
    let ``test github_687`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_687.mzn"

    [<Fact>]
    let ``test github_691`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_691.mzn"

    [<Fact>]
    let ``test github_693_part1`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_693_part1.mzn"

    [<Fact>]
    let ``test github_693_part2`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_693_part2.mzn"

    [<Fact>]
    let ``test github_695`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_695.mzn"

    [<Fact>]
    let ``test github_700`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_700.mzn"

    [<Fact>]
    let ``test github_700_bad_sol`` () =
        testParseFile @"tests\libminizinc\unit\regression\github_700_bad_sol.mzn"

    [<Fact>]
    let ``test hundred_doors_unoptimized`` () =
        testParseFile @"tests\libminizinc\unit\regression\hundred_doors_unoptimized.mzn"

    [<Fact>]
    let ``test if_then_else_absent`` () =
        testParseFile @"tests\libminizinc\unit\regression\if_then_else_absent.mzn"

    [<Fact>]
    let ``test int_times`` () =
        testParseFile @"tests\libminizinc\unit\regression\int_times.mzn"

    [<Fact>]
    let ``test is_fixed`` () =
        testParseFile @"tests\libminizinc\unit\regression\is_fixed.mzn"

    [<Fact>]
    let ``test is_fixed_comp`` () =
        testParseFile @"tests\libminizinc\unit\regression\is_fixed_comp.mzn"

    [<Fact>]
    let ``test lb_ub_dom_array_opt`` () =
        testParseFile @"tests\libminizinc\unit\regression\lb_ub_dom_array_opt.mzn"

    [<Fact>]
    let ``test let_domain_from_generator`` () =
        testParseFile @"tests\libminizinc\unit\regression\let_domain_from_generator.mzn"

    [<Fact>]
    let ``test linear_bool_elem_bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\linear_bool_elem_bug.mzn"

    [<Fact>]
    let ``test makepar_output`` () =
        testParseFile @"tests\libminizinc\unit\regression\makepar_output.mzn"

    [<Fact>]
    let ``test multi_goal_hierarchy_error`` () =
        testParseFile @"tests\libminizinc\unit\regression\multi_goal_hierarchy_error.mzn"

    [<Fact>]
    let ``test nested_clause`` () =
        testParseFile @"tests\libminizinc\unit\regression\nested_clause.mzn"

    [<Fact>]
    let ``test non-set-array-ti-location`` () =
        testParseFile @"tests\libminizinc\unit\regression\non-set-array-ti-location.mzn"

    [<Fact>]
    let ``test non_pos_pow`` () =
        testParseFile @"tests\libminizinc\unit\regression\non_pos_pow.mzn"

    [<Fact>]
    let ``test nosets_369`` () =
        testParseFile @"tests\libminizinc\unit\regression\nosets_369.mzn"

    [<Fact>]
    let ``test nosets_set_search`` () =
        testParseFile @"tests\libminizinc\unit\regression\nosets_set_search.mzn"

    [<Fact>]
    let ``test no_macro`` () =
        testParseFile @"tests\libminizinc\unit\regression\no_macro.mzn"

    [<Fact>]
    let ``test opt_minmax`` () =
        testParseFile @"tests\libminizinc\unit\regression\opt_minmax.mzn"

    [<Fact>]
    let ``test opt_noncontiguous_domain`` () =
        testParseFile @"tests\libminizinc\unit\regression\opt_noncontiguous_domain.mzn"

    [<Fact>]
    let ``test opt_removed_items`` () =
        testParseFile @"tests\libminizinc\unit\regression\opt_removed_items.mzn"

    [<Fact>]
    let ``test output_2d_array_enum`` () =
        testParseFile @"tests\libminizinc\unit\regression\output_2d_array_enum.mzn"

    [<Fact>]
    let ``test output_fn_toplevel_var`` () =
        testParseFile @"tests\libminizinc\unit\regression\output_fn_toplevel_var.mzn"

    [<Fact>]
    let ``test output_only_fn`` () =
        testParseFile @"tests\libminizinc\unit\regression\output_only_fn.mzn"

    [<Fact>]
    let ``test output_only_no_rhs`` () =
        testParseFile @"tests\libminizinc\unit\regression\output_only_no_rhs.mzn"

    [<Fact>]
    let ``test overloading`` () =
        testParseFile @"tests\libminizinc\unit\regression\overloading.mzn"

    [<Fact>]
    let ``test parser_location`` () =
        testParseFile @"tests\libminizinc\unit\regression\parser_location.mzn"

    [<Fact>]
    let ``test parse_assignments`` () =
        testParseFile @"tests\libminizinc\unit\regression\parse_assignments.mzn"

    [<Fact>]
    let ``test par_opt_dom`` () =
        testParseFile @"tests\libminizinc\unit\regression\par_opt_dom.mzn"

    [<Fact>]
    let ``test par_opt_equal`` () =
        testParseFile @"tests\libminizinc\unit\regression\par_opt_equal.mzn"

    [<Fact>]
    let ``test pow_undefined`` () =
        testParseFile @"tests\libminizinc\unit\regression\pow_undefined.mzn"

    [<Fact>]
    let ``test pred_param_r7550`` () =
        testParseFile @"tests\libminizinc\unit\regression\pred_param_r7550.mzn"

    [<Fact>]
    let ``test round`` () =
        testParseFile @"tests\libminizinc\unit\regression\round.mzn"

    [<Fact>]
    let ``test seq_search_bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\seq_search_bug.mzn"

    [<Fact>]
    let ``test set_inequality_par`` () =
        testParseFile @"tests\libminizinc\unit\regression\set_inequality_par.mzn"

    [<Fact>]
    let ``test slice_enum_indexset`` () =
        testParseFile @"tests\libminizinc\unit\regression\slice_enum_indexset.mzn"

    [<Fact>]
    let ``test string-test-arg`` () =
        testParseFile @"tests\libminizinc\unit\regression\string-test-arg.mzn"

    [<Fact>]
    let ``test subsets_100`` () =
        testParseFile @"tests\libminizinc\unit\regression\subsets_100.mzn"

    [<Fact>]
    let ``test test_annotation_on_exists`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_annotation_on_exists.mzn"

    [<Fact>]
    let ``test test_bool2int`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bool2int.mzn"

    [<Fact>]
    let ``test test_bug218`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug218.mzn"

    [<Fact>]
    let ``test test_bug359`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug359.mzn"

    [<Fact>]
    let ``test test_bug45`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug45.mzn"

    [<Fact>]
    let ``test test_bug53`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug53.mzn"

    [<Fact>]
    let ``test test_bug54`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug54.mzn"

    [<Fact>]
    let ``test test_bug55`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug55.mzn"

    [<Fact>]
    let ``test test_bug57`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug57.mzn"

    [<Fact>]
    let ``test test_bug65`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug65.mzn"

    [<Fact>]
    let ``test test_bug66`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug66.mzn"

    [<Fact>]
    let ``test test_bug67`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug67.mzn"

    [<Fact>]
    let ``test test_bug70`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug70.mzn"

    [<Fact>]
    let ``test test_bug71`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug71.mzn"

    [<Fact>]
    let ``test test_bug72`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug72.mzn"

    [<Fact>]
    let ``test test_bug_129`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_129.mzn"

    [<Fact>]
    let ``test test_bug_476`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_476.mzn"

    [<Fact>]
    let ``test test_bug_483`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_483.mzn"

    [<Fact>]
    let ``test test_bug_493`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_493.mzn"

    [<Fact>]
    let ``test test_bug_494`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_494.mzn"

    [<Fact>]
    let ``test test_bug_520`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_520.mzn"

    [<Fact>]
    let ``test test_bug_521`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_521.mzn"

    [<Fact>]
    let ``test test_bug_527`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_527.mzn"

    [<Fact>]
    let ``test test_bug_529`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_529.mzn"

    [<Fact>]
    let ``test test_bug_588`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_588.mzn"

    [<Fact>]
    let ``test test_bug_637`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_637.mzn"

    [<Fact>]
    let ``test test_bug_array_sum_bounds`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_array_sum_bounds.mzn"

    [<Fact>]
    let ``test test_bug_ite_array_eq`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_ite_array_eq.mzn"

    [<Fact>]
    let ``test test_bug_pred_arg`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_bug_pred_arg.mzn"

    [<Fact>]
    let ``test test_equality_of_indirect_annotations`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_equality_of_indirect_annotations.mzn"

    [<Fact>]
    let ``test test_github_30`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_github_30.mzn"

    [<Fact>]
    let ``test test_multioutput`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_multioutput.mzn"

    [<Fact>]
    let ``test test_not_in`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_not_in.mzn"

    [<Fact>]
    let ``test test_output_array_of_set`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_output_array_of_set.mzn"

    [<Fact>]
    let ``test test_output_string_var`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_output_string_var.mzn"

    [<Fact>]
    let ``test test_parout`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_parout.mzn"

    [<Fact>]
    let ``test test_seq_precede_chain_set`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_seq_precede_chain_set.mzn"

    [<Fact>]
    let ``test test_slice_1d_array`` () =
        testParseFile @"tests\libminizinc\unit\regression\test_slice_1d_array.mzn"

    [<Fact>]
    let ``test ti_error_location`` () =
        testParseFile @"tests\libminizinc\unit\regression\ti_error_location.mzn"

    [<Fact>]
    let ``test ts_bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\ts_bug.mzn"

    [<Fact>]
    let ``test type_specialise_array_return`` () =
        testParseFile @"tests\libminizinc\unit\regression\type_specialise_array_return.mzn"

    [<Fact>]
    let ``test var_bool_comp`` () =
        testParseFile @"tests\libminizinc\unit\regression\var_bool_comp.mzn"

    [<Fact>]
    let ``test var_opt_unconstrained`` () =
        testParseFile @"tests\libminizinc\unit\regression\var_opt_unconstrained.mzn"

    [<Fact>]
    let ``test var_self_assign_bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\var_self_assign_bug.mzn"

    [<Fact>]
    let ``test warm_start`` () =
        testParseFile @"tests\libminizinc\unit\regression\warm_start.mzn"

    [<Fact>]
    let ``test where-forall-bug`` () =
        testParseFile @"tests\libminizinc\unit\regression\where-forall-bug.mzn"

    [<Fact>]
    let ``test xor_mixed_context`` () =
        testParseFile @"tests\libminizinc\unit\regression\xor_mixed_context.mzn"

    [<Fact>]
    let ``test int_choice_1`` () =
        testParseFile @"tests\libminizinc\unit\search\int_choice_1.mzn"

    [<Fact>]
    let ``test int_choice_2`` () =
        testParseFile @"tests\libminizinc\unit\search\int_choice_2.mzn"

    [<Fact>]
    let ``test int_choice_6`` () =
        testParseFile @"tests\libminizinc\unit\search\int_choice_6.mzn"

    [<Fact>]
    let ``test int_var_select_1`` () =
        testParseFile @"tests\libminizinc\unit\search\int_var_select_1.mzn"

    [<Fact>]
    let ``test int_var_select_2`` () =
        testParseFile @"tests\libminizinc\unit\search\int_var_select_2.mzn"

    [<Fact>]
    let ``test int_var_select_3`` () =
        testParseFile @"tests\libminizinc\unit\search\int_var_select_3.mzn"

    [<Fact>]
    let ``test int_var_select_4`` () =
        testParseFile @"tests\libminizinc\unit\search\int_var_select_4.mzn"

    [<Fact>]
    let ``test int_var_select_6`` () =
        testParseFile @"tests\libminizinc\unit\search\int_var_select_6.mzn"

    [<Fact>]
    let ``test test-ff1`` () =
        testParseFile @"tests\libminizinc\unit\search\test-ff1.mzn"

    [<Fact>]
    let ``test test-ff2`` () =
        testParseFile @"tests\libminizinc\unit\search\test-ff2.mzn"

    [<Fact>]
    let ``test test-ff3`` () =
        testParseFile @"tests\libminizinc\unit\search\test-ff3.mzn"

    [<Fact>]
    let ``test test-large1`` () =
        testParseFile @"tests\libminizinc\unit\search\test-large1.mzn"

    [<Fact>]
    let ``test test-med1`` () =
        testParseFile @"tests\libminizinc\unit\search\test-med1.mzn"

    [<Fact>]
    let ``test test-small1`` () =
        testParseFile @"tests\libminizinc\unit\search\test-small1.mzn"

    [<Fact>]
    let ``test alias`` () =
        testParseFile @"tests\libminizinc\unit\types\alias.mzn"

    [<Fact>]
    let ``test alias_call`` () =
        testParseFile @"tests\libminizinc\unit\types\alias_call.mzn"

    [<Fact>]
    let ``test alias_extern_dom`` () =
        testParseFile @"tests\libminizinc\unit\types\alias_extern_dom.mzn"

    [<Fact>]
    let ``test alias_set_of_array`` () =
        testParseFile @"tests\libminizinc\unit\types\alias_set_of_array.mzn"

    [<Fact>]
    let ``test comprehension_type`` () =
        testParseFile @"tests\libminizinc\unit\types\comprehension_type.mzn"

    [<Fact>]
    let ``test cv_comprehension`` () =
        testParseFile @"tests\libminizinc\unit\types\cv_comprehension.mzn"

    [<Fact>]
    let ``test enum_decl`` () =
        testParseFile @"tests\libminizinc\unit\types\enum_decl.mzn"

    [<Fact>]
    let ``test github_647`` () =
        testParseFile @"tests\libminizinc\unit\types\github_647.mzn"

    [<Fact>]
    let ``test nonbool_constraint`` () =
        testParseFile @"tests\libminizinc\unit\types\nonbool_constraint.mzn"

    [<Fact>]
    let ``test nonbool_constraint_let`` () =
        testParseFile @"tests\libminizinc\unit\types\nonbool_constraint_let.mzn"

    [<Fact>]
    let ``test non_contig_enum`` () =
        testParseFile @"tests\libminizinc\unit\types\non_contig_enum.mzn"

    [<Fact>]
    let ``test polymorphic_overloading`` () =
        testParseFile @"tests\libminizinc\unit\types\polymorphic_overloading.mzn"

    [<Fact>]
    let ``test record_access_error`` () =
        testParseFile @"tests\libminizinc\unit\types\record_access_error.mzn"

    [<Fact>]
    let ``test record_access_success`` () =
        testParseFile @"tests\libminizinc\unit\types\record_access_success.mzn"

    [<Fact>]
    let ``test record_array_access_error`` () =
        testParseFile @"tests\libminizinc\unit\types\record_array_access_error.mzn"

    [<Fact>]
    let ``test record_binop_par`` () =
        testParseFile @"tests\libminizinc\unit\types\record_binop_par.mzn"

    [<Fact>]
    let ``test record_binop_var`` () =
        testParseFile @"tests\libminizinc\unit\types\record_binop_var.mzn"

    [<Fact>]
    let ``test record_comprehensions`` () =
        testParseFile @"tests\libminizinc\unit\types\record_comprehensions.mzn"

    [<Fact>]
    let ``test record_decl_error`` () =
        testParseFile @"tests\libminizinc\unit\types\record_decl_error.mzn"

    [<Fact>]
    let ``test record_ite_error`` () =
        testParseFile @"tests\libminizinc\unit\types\record_ite_error.mzn"

    [<Fact>]
    let ``test record_lit_dup`` () =
        testParseFile @"tests\libminizinc\unit\types\record_lit_dup.mzn"

    [<Fact>]
    let ``test record_nested`` () =
        testParseFile @"tests\libminizinc\unit\types\record_nested.mzn"

    [<Fact>]
    let ``test record_output`` () =
        testParseFile @"tests\libminizinc\unit\types\record_output.mzn"

    [<Fact>]
    let ``test record_subtyping`` () =
        testParseFile @"tests\libminizinc\unit\types\record_subtyping.mzn"

    [<Fact>]
    let ``test record_var_element`` () =
        testParseFile @"tests\libminizinc\unit\types\record_var_element.mzn"

    [<Fact>]
    let ``test record_var_ite`` () =
        testParseFile @"tests\libminizinc\unit\types\record_var_ite.mzn"

    [<Fact>]
    let ``test struct_bind_1`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_bind_1.mzn"

    [<Fact>]
    let ``test struct_bind_2`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_bind_2.mzn"

    [<Fact>]
    let ``test struct_domain_1`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_domain_1.mzn"

    [<Fact>]
    let ``test struct_domain_2`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_domain_2.mzn"

    [<Fact>]
    let ``test struct_domain_3`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_domain_3.mzn"

    [<Fact>]
    let ``test struct_domain_4`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_domain_4.mzn"

    [<Fact>]
    let ``test struct_domain_5`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_domain_5.mzn"

    [<Fact>]
    let ``test struct_domain_6`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_domain_6.mzn"

    [<Fact>]
    let ``test struct_index_sets_1`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_index_sets_1.mzn"

    [<Fact>]
    let ``test struct_index_sets_2`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_index_sets_2.mzn"

    [<Fact>]
    let ``test struct_return_ti_1`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_return_ti_1.mzn"

    [<Fact>]
    let ``test struct_return_ti_2`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_return_ti_2.mzn"

    [<Fact>]
    let ``test struct_return_ti_3`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_return_ti_3.mzn"

    [<Fact>]
    let ``test struct_return_ti_4`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_return_ti_4.mzn"

    [<Fact>]
    let ``test struct_specialise`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_specialise.mzn"

    [<Fact>]
    let ``test struct_specialise_return`` () =
        testParseFile @"tests\libminizinc\unit\types\struct_specialise_return.mzn"

    [<Fact>]
    let ``test test_any_enum_typeinstid`` () =
        testParseFile @"tests\libminizinc\unit\types\test_any_enum_typeinstid.mzn"

    [<Fact>]
    let ``test tuple_access_error1`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_access_error1.mzn"

    [<Fact>]
    let ``test tuple_access_error2`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_access_error2.mzn"

    [<Fact>]
    let ``test tuple_access_success`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_access_success.mzn"

    [<Fact>]
    let ``test tuple_array_access_error`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_array_access_error.mzn"

    [<Fact>]
    let ``test tuple_binop_par`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_binop_par.mzn"

    [<Fact>]
    let ``test tuple_binop_var`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_binop_var.mzn"

    [<Fact>]
    let ``test tuple_comprehensions`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_comprehensions.mzn"

    [<Fact>]
    let ``test tuple_int_set_of_int_specialisation`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_int_set_of_int_specialisation.mzn"

    [<Fact>]
    let ``test tuple_ite_error`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_ite_error.mzn"

    [<Fact>]
    let ``test tuple_lit`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_lit.mzn"

    [<Fact>]
    let ``test tuple_mkpar`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_mkpar.mzn"

    [<Fact>]
    let ``test tuple_output`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_output.mzn"

    [<Fact>]
    let ``test tuple_subtyping`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_subtyping.mzn"

    [<Fact>]
    let ``test tuple_var_element`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_var_element.mzn"

    [<Fact>]
    let ``test tuple_var_ite`` () =
        testParseFile @"tests\libminizinc\unit\types\tuple_var_ite.mzn"

    [<Fact>]
    let ``test var_ann_a`` () =
        testParseFile @"tests\libminizinc\unit\types\var_ann_a.mzn"

    [<Fact>]
    let ``test var_ann_b`` () =
        testParseFile @"tests\libminizinc\unit\types\var_ann_b.mzn"

    [<Fact>]
    let ``test var_ann_comprehension`` () =
        testParseFile @"tests\libminizinc\unit\types\var_ann_comprehension.mzn"

    [<Fact>]
    let ``test var_set_bool`` () =
        testParseFile @"tests\libminizinc\unit\types\var_set_bool.mzn"

    [<Fact>]
    let ``test var_set_float`` () =
        testParseFile @"tests\libminizinc\unit\types\var_set_float.mzn"

    [<Fact>]
    let ``test var_set_float_comprehension`` () =
        testParseFile @"tests\libminizinc\unit\types\var_set_float_comprehension.mzn"

    [<Fact>]
    let ``test var_string_a`` () =
        testParseFile @"tests\libminizinc\unit\types\var_string_a.mzn"

    [<Fact>]
    let ``test var_string_b`` () =
        testParseFile @"tests\libminizinc\unit\types\var_string_b.mzn"

    [<Fact>]
    let ``test var_string_comprehension`` () =
        testParseFile @"tests\libminizinc\unit\types\var_string_comprehension.mzn"

    [<Fact>]
    let ``test globals_alldiff_set_nosets`` () =
        testParseFile @"tests\libminizinc\unit\globals\alldifferent\globals_alldiff_set_nosets.mzn"

    [<Fact>]
    let ``test globals_all_different_int`` () =
        testParseFile @"tests\libminizinc\unit\globals\alldifferent\globals_all_different_int.mzn"

    [<Fact>]
    let ``test globals_all_different_int_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\alldifferent\globals_all_different_int_opt.mzn"

    [<Fact>]
    let ``test globals_all_different_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\alldifferent\globals_all_different_set.mzn"

    [<Fact>]
    let ``test test_alldiff_except0`` () =
        testParseFile @"tests\libminizinc\unit\globals\alldifferent_except_0\test_alldiff_except0.mzn"

    [<Fact>]
    let ``test test_alldiff_except0b`` () =
        testParseFile @"tests\libminizinc\unit\globals\alldifferent_except_0\test_alldiff_except0b.mzn"

    [<Fact>]
    let ``test globals_all_disjoint`` () =
        testParseFile @"tests\libminizinc\unit\globals\all_disjoint\globals_all_disjoint.mzn"

    [<Fact>]
    let ``test globals_all_equal_int`` () =
        testParseFile @"tests\libminizinc\unit\globals\all_equal\globals_all_equal_int.mzn"

    [<Fact>]
    let ``test globals_all_equal_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\all_equal\globals_all_equal_set.mzn"

    [<Fact>]
    let ``test globals_among`` () =
        testParseFile @"tests\libminizinc\unit\globals\among\globals_among.mzn"

    [<Fact>]
    let ``test globals_arg_max`` () =
        testParseFile @"tests\libminizinc\unit\globals\arg_max\globals_arg_max.mzn"

    [<Fact>]
    let ``test globals_arg_max_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\arg_max\globals_arg_max_opt.mzn"

    [<Fact>]
    let ``test globals_arg_max_opt_weak`` () =
        testParseFile @"tests\libminizinc\unit\globals\arg_max\globals_arg_max_opt_weak.mzn"

    [<Fact>]
    let ``test globals_arg_min`` () =
        testParseFile @"tests\libminizinc\unit\globals\arg_min\globals_arg_min.mzn"

    [<Fact>]
    let ``test globals_arg_min_opt_weak`` () =
        testParseFile @"tests\libminizinc\unit\globals\arg_min\globals_arg_min_opt_weak.mzn"

    [<Fact>]
    let ``test arg_val_enum`` () =
        testParseFile @"tests\libminizinc\unit\globals\arg_val\arg_val_enum.mzn"

    [<Fact>]
    let ``test globals_at_least`` () =
        testParseFile @"tests\libminizinc\unit\globals\atleast\globals_at_least.mzn"

    [<Fact>]
    let ``test globals_at_most1`` () =
        testParseFile @"tests\libminizinc\unit\globals\atmost1\globals_at_most1.mzn"

    [<Fact>]
    let ``test globals_bin_packing`` () =
        testParseFile @"tests\libminizinc\unit\globals\bin_packing\globals_bin_packing.mzn"

    [<Fact>]
    let ``test globals_bin_packing_capa`` () =
        testParseFile @"tests\libminizinc\unit\globals\bin_packing_capa\globals_bin_packing_capa.mzn"

    [<Fact>]
    let ``test test_circuit`` () =
        testParseFile @"tests\libminizinc\unit\globals\circuit\test_circuit.mzn"

    [<Fact>]
    let ``test globals_count`` () =
        testParseFile @"tests\libminizinc\unit\globals\count\globals_count.mzn"

    [<Fact>]
    let ``test github_589`` () =
        testParseFile @"tests\libminizinc\unit\globals\cumulative\github_589.mzn"

    [<Fact>]
    let ``test globals_cumulative`` () =
        testParseFile @"tests\libminizinc\unit\globals\cumulative\globals_cumulative.mzn"

    [<Fact>]
    let ``test unsat_resource_cumulative`` () =
        testParseFile @"tests\libminizinc\unit\globals\cumulative\unsat_resource_cumulative.mzn"

    [<Fact>]
    let ``test globals_decreasing`` () =
        testParseFile @"tests\libminizinc\unit\globals\decreasing\globals_decreasing.mzn"

    [<Fact>]
    let ``test globals_disjoint`` () =
        testParseFile @"tests\libminizinc\unit\globals\disjoint\globals_disjoint.mzn"

    [<Fact>]
    let ``test globals_distribute`` () =
        testParseFile @"tests\libminizinc\unit\globals\distribute\globals_distribute.mzn"

    [<Fact>]
    let ``test globals_global_cardinality`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality\globals_global_cardinality.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_low_up`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality\globals_global_cardinality_low_up.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_low_up_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality\globals_global_cardinality_low_up_opt.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_low_up_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality\globals_global_cardinality_low_up_set.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality\globals_global_cardinality_opt.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality\globals_global_cardinality_set.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_closed`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality_closed\globals_global_cardinality_closed.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_closed_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality_closed\globals_global_cardinality_closed_opt.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_closed_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality_closed\globals_global_cardinality_closed_set.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_low_up_closed`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_low_up_closed_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed_opt.mzn"

    [<Fact>]
    let ``test globals_global_cardinality_low_up_closed_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed_set.mzn"

    [<Fact>]
    let ``test globals_increasing`` () =
        testParseFile @"tests\libminizinc\unit\globals\increasing\globals_increasing.mzn"

    [<Fact>]
    let ``test globals_strictly_increasing_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\increasing\globals_strictly_increasing_opt.mzn"

    [<Fact>]
    let ``test globals_int_set_channel`` () =
        testParseFile @"tests\libminizinc\unit\globals\int_set_channel\globals_int_set_channel.mzn"

    [<Fact>]
    let ``test test_int_set_channel2`` () =
        testParseFile @"tests\libminizinc\unit\globals\int_set_channel\test_int_set_channel2.mzn"

    [<Fact>]
    let ``test globals_inverse`` () =
        testParseFile @"tests\libminizinc\unit\globals\inverse\globals_inverse.mzn"

    [<Fact>]
    let ``test inverse_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\inverse\inverse_opt.mzn"

    [<Fact>]
    let ``test globals_inverse_in_range`` () =
        testParseFile @"tests\libminizinc\unit\globals\inverse_in_range\globals_inverse_in_range.mzn"

    [<Fact>]
    let ``test globals_inverse_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\inverse_set\globals_inverse_set.mzn"

    [<Fact>]
    let ``test globals_lex2`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex2\globals_lex2.mzn"

    [<Fact>]
    let ``test globals_lex_chain`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_chain\globals_lex_chain.mzn"

    [<Fact>]
    let ``test globals_lex_chain__orbitope`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_chain\globals_lex_chain__orbitope.mzn"

    [<Fact>]
    let ``test globals_lex_greater`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_greater\globals_lex_greater.mzn"

    [<Fact>]
    let ``test globals_lex_greatereq`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_greatereq\globals_lex_greatereq.mzn"

    [<Fact>]
    let ``test globals_lex_less`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_less\globals_lex_less.mzn"

    [<Fact>]
    let ``test test_bool_lex_less`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_less\test_bool_lex_less.mzn"

    [<Fact>]
    let ``test globals_lex_lesseq`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_lesseq\globals_lex_lesseq.mzn"

    [<Fact>]
    let ``test test_bool_lex_lesseq`` () =
        testParseFile @"tests\libminizinc\unit\globals\lex_lesseq\test_bool_lex_lesseq.mzn"

    [<Fact>]
    let ``test globals_link_set_to_booleans`` () =
        testParseFile @"tests\libminizinc\unit\globals\link_set_to_booleans\globals_link_set_to_booleans.mzn"

    [<Fact>]
    let ``test globals_maximum_int`` () =
        testParseFile @"tests\libminizinc\unit\globals\maximum\globals_maximum_int.mzn"

    [<Fact>]
    let ``test globals_minimum_int`` () =
        testParseFile @"tests\libminizinc\unit\globals\minimum\globals_minimum_int.mzn"

    [<Fact>]
    let ``test globals_nvalue`` () =
        testParseFile @"tests\libminizinc\unit\globals\nvalue\globals_nvalue.mzn"

    [<Fact>]
    let ``test nvalue_total`` () =
        testParseFile @"tests\libminizinc\unit\globals\nvalue\nvalue_total.mzn"

    [<Fact>]
    let ``test globals_partition_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\partition_set\globals_partition_set.mzn"

    [<Fact>]
    let ``test globals_range`` () =
        testParseFile @"tests\libminizinc\unit\globals\range\globals_range.mzn"

    [<Fact>]
    let ``test globals_regular`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular.mzn"

    [<Fact>]
    let ``test globals_regular_regex_1`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular_regex_1.mzn"

    [<Fact>]
    let ``test globals_regular_regex_2`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular_regex_2.mzn"

    [<Fact>]
    let ``test globals_regular_regex_3`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular_regex_3.mzn"

    [<Fact>]
    let ``test globals_regular_regex_4`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular_regex_4.mzn"

    [<Fact>]
    let ``test globals_regular_regex_5`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular_regex_5.mzn"

    [<Fact>]
    let ``test globals_regular_regex_6`` () =
        testParseFile @"tests\libminizinc\unit\globals\regular\globals_regular_regex_6.mzn"

    [<Fact>]
    let ``test roots_bad`` () =
        testParseFile @"tests\libminizinc\unit\globals\roots\roots_bad.mzn"

    [<Fact>]
    let ``test test_roots`` () =
        testParseFile @"tests\libminizinc\unit\globals\roots\test_roots.mzn"

    [<Fact>]
    let ``test test_roots2`` () =
        testParseFile @"tests\libminizinc\unit\globals\roots\test_roots2.mzn"

    [<Fact>]
    let ``test test_roots3`` () =
        testParseFile @"tests\libminizinc\unit\globals\roots\test_roots3.mzn"

    [<Fact>]
    let ``test globals_sliding_sum`` () =
        testParseFile @"tests\libminizinc\unit\globals\sliding_sum\globals_sliding_sum.mzn"

    [<Fact>]
    let ``test globals_sort`` () =
        testParseFile @"tests\libminizinc\unit\globals\sort\globals_sort.mzn"

    [<Fact>]
    let ``test globals_strict_lex2`` () =
        testParseFile @"tests\libminizinc\unit\globals\strict_lex2\globals_strict_lex2.mzn"

    [<Fact>]
    let ``test test_subcircuit`` () =
        testParseFile @"tests\libminizinc\unit\globals\subcircuit\test_subcircuit.mzn"

    [<Fact>]
    let ``test globals_sum_pred`` () =
        testParseFile @"tests\libminizinc\unit\globals\sum_pred\globals_sum_pred.mzn"

    [<Fact>]
    let ``test globals_table`` () =
        testParseFile @"tests\libminizinc\unit\globals\table\globals_table.mzn"

    [<Fact>]
    let ``test globals_table_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\table\globals_table_opt.mzn"

    [<Fact>]
    let ``test globals_value_precede_int`` () =
        testParseFile @"tests\libminizinc\unit\globals\value_precede\globals_value_precede_int.mzn"

    [<Fact>]
    let ``test globals_value_precede_int_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\value_precede\globals_value_precede_int_opt.mzn"

    [<Fact>]
    let ``test globals_value_precede_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\value_precede\globals_value_precede_set.mzn"

    [<Fact>]
    let ``test globals_value_precede_chain_int`` () =
        testParseFile @"tests\libminizinc\unit\globals\value_precede_chain\globals_value_precede_chain_int.mzn"

    [<Fact>]
    let ``test globals_value_precede_chain_int_opt`` () =
        testParseFile @"tests\libminizinc\unit\globals\value_precede_chain\globals_value_precede_chain_int_opt.mzn"

    [<Fact>]
    let ``test globals_value_precede_chain_set`` () =
        testParseFile @"tests\libminizinc\unit\globals\value_precede_chain\globals_value_precede_chain_set.mzn"

    [<Fact>]
    let ``test globals_var_sqr_sym`` () =
        testParseFile @"tests\libminizinc\unit\globals\var_sqr_sym\globals_var_sqr_sym.mzn"

    [<Fact>]
    let ``test 2DPacking`` () =
        testParseFile @"tests\libminizinc\examples\2DPacking.mzn"

    [<Fact>]
    let ``test alpha`` () =
        testParseFile @"tests\libminizinc\examples\alpha.mzn"

    [<Fact>]
    let ``test battleships10`` () =
        testParseFile @"tests\libminizinc\examples\battleships10.mzn"

    [<Fact>]
    let ``test battleships_1`` () =
        testParseFile @"tests\libminizinc\examples\battleships_1.mzn"

    [<Fact>]
    let ``test battleships_2`` () =
        testParseFile @"tests\libminizinc\examples\battleships_2.mzn"

    [<Fact>]
    let ``test battleships_3`` () =
        testParseFile @"tests\libminizinc\examples\battleships_3.mzn"

    [<Fact>]
    let ``test battleships_4`` () =
        testParseFile @"tests\libminizinc\examples\battleships_4.mzn"

    [<Fact>]
    let ``test battleships_5`` () =
        testParseFile @"tests\libminizinc\examples\battleships_5.mzn"

    [<Fact>]
    let ``test battleships_7`` () =
        testParseFile @"tests\libminizinc\examples\battleships_7.mzn"

    [<Fact>]
    let ``test battleships_9`` () =
        testParseFile @"tests\libminizinc\examples\battleships_9.mzn"

    [<Fact>]
    let ``test blocksworld_instance_1`` () =
        testParseFile @"tests\libminizinc\examples\blocksworld_instance_1.mzn"

    [<Fact>]
    let ``test blocksworld_instance_2`` () =
        testParseFile @"tests\libminizinc\examples\blocksworld_instance_2.mzn"

    [<Fact>]
    let ``test cutstock`` () =
        testParseFile @"tests\libminizinc\examples\cutstock.mzn"

    [<Fact>]
    let ``test eq20`` () =
        testParseFile @"tests\libminizinc\examples\eq20.mzn"

    [<Fact>]
    let ``test factory_planning_instance`` () =
        testParseFile @"tests\libminizinc\examples\factory_planning_instance.mzn"

    [<Fact>]
    let ``test golomb`` () =
        testParseFile @"tests\libminizinc\examples\golomb.mzn"

    [<Fact>]
    let ``test halfreif`` () =
        testParseFile @"tests\libminizinc\examples\halfreif.mzn"

    [<Fact>]
    let ``test jobshop2x2`` () =
        testParseFile @"tests\libminizinc\examples\jobshop2x2.mzn"

    [<Fact>]
    let ``test knights`` () =
        testParseFile @"tests\libminizinc\examples\knights.mzn"

    [<Fact>]
    let ``test langford`` () =
        testParseFile @"tests\libminizinc\examples\langford.mzn"

    [<Fact>]
    let ``test langford2`` () =
        testParseFile @"tests\libminizinc\examples\langford2.mzn"

    [<Fact>]
    let ``test latin_squares_fd`` () =
        testParseFile @"tests\libminizinc\examples\latin_squares_fd.mzn"

    [<Fact>]
    let ``test magicsq_3`` () =
        testParseFile @"tests\libminizinc\examples\magicsq_3.mzn"

    [<Fact>]
    let ``test magicsq_4`` () =
        testParseFile @"tests\libminizinc\examples\magicsq_4.mzn"

    [<Fact>]
    let ``test magicsq_5`` () =
        testParseFile @"tests\libminizinc\examples\magicsq_5.mzn"

    [<Fact>]
    let ``test multidimknapsack_simple`` () =
        testParseFile @"tests\libminizinc\examples\multidimknapsack_simple.mzn"

    [<Fact>]
    let ``test oss`` () =
        testParseFile @"tests\libminizinc\examples\oss.mzn"

    [<Fact>]
    let ``test packing`` () =
        testParseFile @"tests\libminizinc\examples\packing.mzn"

    [<Fact>]
    let ``test perfsq`` () =
        testParseFile @"tests\libminizinc\examples\perfsq.mzn"

    [<Fact>]
    let ``test perfsq2`` () =
        testParseFile @"tests\libminizinc\examples\perfsq2.mzn"

    [<Fact>]
    let ``test photo`` () =
        testParseFile @"tests\libminizinc\examples\photo.mzn"

    [<Fact>]
    let ``test product_fd`` () =
        testParseFile @"tests\libminizinc\examples\product_fd.mzn"

    [<Fact>]
    let ``test product_lp`` () =
        testParseFile @"tests\libminizinc\examples\product_lp.mzn"

    [<Fact>]
    let ``test quasigroup_qg5`` () =
        testParseFile @"tests\libminizinc\examples\quasigroup_qg5.mzn"

    [<Fact>]
    let ``test queen_cp2`` () =
        testParseFile @"tests\libminizinc\examples\queen_cp2.mzn"

    [<Fact>]
    let ``test queen_ip`` () =
        testParseFile @"tests\libminizinc\examples\queen_ip.mzn"

    [<Fact>]
    let ``test radiation`` () =
        testParseFile @"tests\libminizinc\examples\radiation.mzn"

    [<Fact>]
    let ``test simple_sat`` () =
        testParseFile @"tests\libminizinc\examples\simple_sat.mzn"

    [<Fact>]
    let ``test singHoist2`` () =
        testParseFile @"tests\libminizinc\examples\singHoist2.mzn"

    [<Fact>]
    let ``test steiner-triples`` () =
        testParseFile @"tests\libminizinc\examples\steiner-triples.mzn"

    [<Fact>]
    let ``test sudoku`` () =
        testParseFile @"tests\libminizinc\examples\sudoku.mzn"

    [<Fact>]
    let ``test template_design`` () =
        testParseFile @"tests\libminizinc\examples\template_design.mzn"

    [<Fact>]
    let ``test tenpenki_1`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_1.mzn"

    [<Fact>]
    let ``test tenpenki_2`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_2.mzn"

    [<Fact>]
    let ``test tenpenki_3`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_3.mzn"

    [<Fact>]
    let ``test tenpenki_4`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_4.mzn"

    [<Fact>]
    let ``test tenpenki_5`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_5.mzn"

    [<Fact>]
    let ``test tenpenki_6`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_6.mzn"

    [<Fact>]
    let ``test timetabling`` () =
        testParseFile @"tests\libminizinc\examples\timetabling.mzn"

    [<Fact>]
    let ``test trucking`` () =
        testParseFile @"tests\libminizinc\examples\trucking.mzn"

    [<Fact>]
    let ``test warehouses`` () =
        testParseFile @"tests\libminizinc\examples\warehouses.mzn"

    [<Fact>]
    let ``test wolf_goat_cabbage`` () =
        testParseFile @"tests\libminizinc\examples\wolf_goat_cabbage.mzn"

    [<Fact>]
    let ``test zebra`` () =
        testParseFile @"tests\libminizinc\examples\zebra.mzn"
