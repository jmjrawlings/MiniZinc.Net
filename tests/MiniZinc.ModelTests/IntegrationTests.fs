    namespace MiniZinc.Tests
    open MiniZinc
    open MiniZinc.Tests
    open Xunit
    open System.IO

    module IntegrationTests =

        let test filePath =
            let file = LibMiniZinc.testDir </> filePath
            let result = parseModelFile file.FullName
            match result with
            | Result.Ok model -> ()
            | Result.Error err -> Assert.Fail(err.Message)

        module ``Unit`` =

            [<Fact>]
            let ``test test-globals-float`` () =
                test @"unit\test-globals-float.mzn"

        module ``Compilation`` =

            [<Fact>]
            let ``test aggregation`` () =
                test @"unit\compilation\aggregation.mzn"

            [<Fact>]
            let ``test annotate_bool_literal`` () =
                test @"unit\compilation\annotate_bool_literal.mzn"

            [<Fact>]
            let ``test annotate_from_array`` () =
                test @"unit\compilation\annotate_from_array.mzn"

            [<Fact>]
            let ``test assert_dbg_flag`` () =
                test @"unit\compilation\assert_dbg_flag.mzn"

            [<Fact>]
            let ``test assert_dbg_ignore`` () =
                test @"unit\compilation\assert_dbg_ignore.mzn"

            [<Fact>]
            let ``test call_root_ctx`` () =
                test @"unit\compilation\call_root_ctx.mzn"

            [<Fact>]
            let ``test chain_compr_mult_clause`` () =
                test @"unit\compilation\chain_compr_mult_clause.mzn"

            [<Fact>]
            let ``test count_rewrite`` () =
                test @"unit\compilation\count_rewrite.mzn"

            [<Fact>]
            let ``test cv_domain`` () =
                test @"unit\compilation\cv_domain.mzn"

            [<Fact>]
            let ``test debug_mode_false`` () =
                test @"unit\compilation\debug_mode_false.mzn"

            [<Fact>]
            let ``test debug_mode_true`` () =
                test @"unit\compilation\debug_mode_true.mzn"

            [<Fact>]
            let ``test defines_var_cycle_breaking`` () =
                test @"unit\compilation\defines_var_cycle_breaking.mzn"

            [<Fact>]
            let ``test float_inf_range_dom`` () =
                test @"unit\compilation\float_inf_range_dom.mzn"

            [<Fact>]
            let ``test has_ann`` () =
                test @"unit\compilation\has_ann.mzn"

            [<Fact>]
            let ``test if_then_no_else`` () =
                test @"unit\compilation\if_then_no_else.mzn"

            [<Fact>]
            let ``test implied_exists_chain`` () =
                test @"unit\compilation\implied_exists_chain.mzn"

            [<Fact>]
            let ``test implied_hr`` () =
                test @"unit\compilation\implied_hr.mzn"

            [<Fact>]
            let ``test int_inf_dom`` () =
                test @"unit\compilation\int_inf_dom.mzn"

            [<Fact>]
            let ``test multiple_neg`` () =
                test @"unit\compilation\multiple_neg.mzn"

            [<Fact>]
            let ``test optimization`` () =
                test @"unit\compilation\optimization.mzn"

            [<Fact>]
            let ``test par_arg_out_of_bounds`` () =
                test @"unit\compilation\par_arg_out_of_bounds.mzn"

            [<Fact>]
            let ``test poly_overload`` () =
                test @"unit\compilation\poly_overload.mzn"

            [<Fact>]
            let ``test quoted_id_flatzinc`` () =
                test @"unit\compilation\quoted_id_flatzinc.mzn"

            [<Fact>]
            let ``test time_limit`` () =
                test @"unit\compilation\time_limit.mzn"

        module ``Division`` =

            [<Fact>]
            let ``test test_div1`` () =
                test @"unit\division\test_div1.mzn"

            [<Fact>]
            let ``test test_div10`` () =
                test @"unit\division\test_div10.mzn"

            [<Fact>]
            let ``test test_div11`` () =
                test @"unit\division\test_div11.mzn"

            [<Fact>]
            let ``test test_div12`` () =
                test @"unit\division\test_div12.mzn"

            [<Fact>]
            let ``test test_div2`` () =
                test @"unit\division\test_div2.mzn"

            [<Fact>]
            let ``test test_div3`` () =
                test @"unit\division\test_div3.mzn"

            [<Fact>]
            let ``test test_div4`` () =
                test @"unit\division\test_div4.mzn"

            [<Fact>]
            let ``test test_div5`` () =
                test @"unit\division\test_div5.mzn"

            [<Fact>]
            let ``test test_div6`` () =
                test @"unit\division\test_div6.mzn"

            [<Fact>]
            let ``test test_div7`` () =
                test @"unit\division\test_div7.mzn"

            [<Fact>]
            let ``test test_div8`` () =
                test @"unit\division\test_div8.mzn"

            [<Fact>]
            let ``test test_div9`` () =
                test @"unit\division\test_div9.mzn"

            [<Fact>]
            let ``test test_div_mod_bounds`` () =
                test @"unit\division\test_div_mod_bounds.mzn"

            [<Fact>]
            let ``test test_fldiv_01`` () =
                test @"unit\division\test_fldiv_01.mzn"

            [<Fact>]
            let ``test test_fldiv_02`` () =
                test @"unit\division\test_fldiv_02.mzn"

        module ``Fdlp`` =

            [<Fact>]
            let ``test test_lp_solve_satisfy`` () =
                test @"unit\fdlp\test_lp_solve_satisfy.mzn"

        module ``General`` =

            [<Fact>]
            let ``test anon_var_flatten`` () =
                test @"unit\general\anon_var_flatten.mzn"

            [<Fact>]
            let ``test array_access_record_out_of_bounds`` () =
                test @"unit\general\array_access_record_out_of_bounds.mzn"

            [<Fact>]
            let ``test array_access_tuple_out_of_bounds`` () =
                test @"unit\general\array_access_tuple_out_of_bounds.mzn"

            [<Fact>]
            let ``test array_string_gen`` () =
                test @"unit\general\array_string_gen.mzn"

            [<Fact>]
            let ``test assert_bad_1`` () =
                test @"unit\general\assert_bad_1.mzn"

            [<Fact>]
            let ``test assert_bad_2`` () =
                test @"unit\general\assert_bad_2.mzn"

            [<Fact>]
            let ``test assert_good`` () =
                test @"unit\general\assert_good.mzn"

            [<Fact>]
            let ``test bin_pack_multiobj`` () =
                test @"unit\general\bin_pack_multiobj.mzn"

            [<Fact>]
            let ``test builtins_arg_max`` () =
                test @"unit\general\builtins_arg_max.mzn"

            [<Fact>]
            let ``test builtins_debug`` () =
                test @"unit\general\builtins_debug.mzn"

            [<Fact>]
            let ``test comprehension_var_ub`` () =
                test @"unit\general\comprehension_var_ub.mzn"

            [<Fact>]
            let ``test cse_ctx`` () =
                test @"unit\general\cse_ctx.mzn"

            [<Fact>]
            let ``test enum_constructor_quoting`` () =
                test @"unit\general\enum_constructor_quoting.mzn"

            [<Fact>]
            let ``test enum_order`` () =
                test @"unit\general\enum_order.mzn"

            [<Fact>]
            let ``test enum_out_of_range_1`` () =
                test @"unit\general\enum_out_of_range_1.mzn"

            [<Fact>]
            let ``test enum_out_of_range_2`` () =
                test @"unit\general\enum_out_of_range_2.mzn"

            [<Fact>]
            let ``test function_param_out_of_range`` () =
                test @"unit\general\function_param_out_of_range.mzn"

            [<Fact>]
            let ``test function_return_out_of_range`` () =
                test @"unit\general\function_return_out_of_range.mzn"

            [<Fact>]
            let ``test function_return_out_of_range_opt`` () =
                test @"unit\general\function_return_out_of_range_opt.mzn"

            [<Fact>]
            let ``test iffall_bv`` () =
                test @"unit\general\iffall_bv.mzn"

            [<Fact>]
            let ``test infinite_domain_bind`` () =
                test @"unit\general\infinite_domain_bind.mzn"

            [<Fact>]
            let ``test json_ignore`` () =
                test @"unit\general\json_ignore.mzn"

            [<Fact>]
            let ``test md_exists`` () =
                test @"unit\general\md_exists.mzn"

            [<Fact>]
            let ``test md_forall`` () =
                test @"unit\general\md_forall.mzn"

            [<Fact>]
            let ``test md_iffall`` () =
                test @"unit\general\md_iffall.mzn"

            [<Fact>]
            let ``test md_product_int`` () =
                test @"unit\general\md_product_int.mzn"

            [<Fact>]
            let ``test md_sum_float`` () =
                test @"unit\general\md_sum_float.mzn"

            [<Fact>]
            let ``test md_sum_int`` () =
                test @"unit\general\md_sum_int.mzn"

            [<Fact>]
            let ``test md_xorall`` () =
                test @"unit\general\md_xorall.mzn"

            [<Fact>]
            let ``test missing_ozn_decl`` () =
                test @"unit\general\missing_ozn_decl.mzn"

            [<Fact>]
            let ``test mortgage`` () =
                test @"unit\general\mortgage.mzn"

            [<Fact>]
            let ``test mzn-implicit1`` () =
                test @"unit\general\mzn-implicit1.mzn"

            [<Fact>]
            let ``test mzn-implicit2`` () =
                test @"unit\general\mzn-implicit2.mzn"

            [<Fact>]
            let ``test mzn-implicit3`` () =
                test @"unit\general\mzn-implicit3.mzn"

            [<Fact>]
            let ``test mzn-implicit4`` () =
                test @"unit\general\mzn-implicit4.mzn"

            [<Fact>]
            let ``test mzn-implicit5`` () =
                test @"unit\general\mzn-implicit5.mzn"

            [<Fact>]
            let ``test mzn-implicit6`` () =
                test @"unit\general\mzn-implicit6.mzn"

            [<Fact>]
            let ``test mzn-implicit7`` () =
                test @"unit\general\mzn-implicit7.mzn"

            [<Fact>]
            let ``test mzn-implicit8`` () =
                test @"unit\general\mzn-implicit8.mzn"

            [<Fact>]
            let ``test mzn_div`` () =
                test @"unit\general\mzn_div.mzn"

            [<Fact>]
            let ``test mzn_mod`` () =
                test @"unit\general\mzn_mod.mzn"

            [<Fact>]
            let ``test overload_bottom`` () =
                test @"unit\general\overload_bottom.mzn"

            [<Fact>]
            let ``test param_out_of_range_float`` () =
                test @"unit\general\param_out_of_range_float.mzn"

            [<Fact>]
            let ``test param_out_of_range_int`` () =
                test @"unit\general\param_out_of_range_int.mzn"

            [<Fact>]
            let ``test pow_1`` () =
                test @"unit\general\pow_1.mzn"

            [<Fact>]
            let ``test pow_2`` () =
                test @"unit\general\pow_2.mzn"

            [<Fact>]
            let ``test pow_3`` () =
                test @"unit\general\pow_3.mzn"

            [<Fact>]
            let ``test pow_4`` () =
                test @"unit\general\pow_4.mzn"

            [<Fact>]
            let ``test pow_bounds`` () =
                test @"unit\general\pow_bounds.mzn"

            [<Fact>]
            let ``test quoted_id_1`` () =
                test @"unit\general\quoted_id_1.mzn"

            [<Fact>]
            let ``test quoted_id_2`` () =
                test @"unit\general\quoted_id_2.mzn"

            [<Fact>]
            let ``test quoted_id_3`` () =
                test @"unit\general\quoted_id_3.mzn"

            [<Fact>]
            let ``test quoted_id_4`` () =
                test @"unit\general\quoted_id_4.mzn"

            [<Fact>]
            let ``test stack_overflow`` () =
                test @"unit\general\stack_overflow.mzn"

            [<Fact>]
            let ``test test-search1`` () =
                test @"unit\general\test-search1.mzn"

            [<Fact>]
            let ``test test_array1`` () =
                test @"unit\general\test_array1.mzn"

            [<Fact>]
            let ``test test_array1and2d`` () =
                test @"unit\general\test_array1and2d.mzn"

            [<Fact>]
            let ``test test_array1d_bad_1`` () =
                test @"unit\general\test_array1d_bad_1.mzn"

            [<Fact>]
            let ``test test_array1d_bad_2`` () =
                test @"unit\general\test_array1d_bad_2.mzn"

            [<Fact>]
            let ``test test_array1d_bad_3`` () =
                test @"unit\general\test_array1d_bad_3.mzn"

            [<Fact>]
            let ``test test_array1d_bad_4`` () =
                test @"unit\general\test_array1d_bad_4.mzn"

            [<Fact>]
            let ``test test_array2`` () =
                test @"unit\general\test_array2.mzn"

            [<Fact>]
            let ``test test_array_as_generator`` () =
                test @"unit\general\test_array_as_generator.mzn"

            [<Fact>]
            let ``test test_bad_array_size-bad`` () =
                test @"unit\general\test_bad_array_size-bad.mzn"

            [<Fact>]
            let ``test test_bad_lb_ub_dom-bad`` () =
                test @"unit\general\test_bad_lb_ub_dom-bad.mzn"

            [<Fact>]
            let ``test test_bool_var_array_access`` () =
                test @"unit\general\test_bool_var_array_access.mzn"

            [<Fact>]
            let ``test test_concat1`` () =
                test @"unit\general\test_concat1.mzn"

            [<Fact>]
            let ``test test_concat2`` () =
                test @"unit\general\test_concat2.mzn"

            [<Fact>]
            let ``test test_concat3`` () =
                test @"unit\general\test_concat3.mzn"

            [<Fact>]
            let ``test test_empty_enum`` () =
                test @"unit\general\test_empty_enum.mzn"

            [<Fact>]
            let ``test test_is_fixed`` () =
                test @"unit\general\test_is_fixed.mzn"

            [<Fact>]
            let ``test test_join1`` () =
                test @"unit\general\test_join1.mzn"

            [<Fact>]
            let ``test test_join2`` () =
                test @"unit\general\test_join2.mzn"

            [<Fact>]
            let ``test test_join3`` () =
                test @"unit\general\test_join3.mzn"

            [<Fact>]
            let ``test test_lb_ub_array_int`` () =
                test @"unit\general\test_lb_ub_array_int.mzn"

            [<Fact>]
            let ``test test_lb_ub_dom_int`` () =
                test @"unit\general\test_lb_ub_dom_int.mzn"

            [<Fact>]
            let ``test test_lb_ub_float`` () =
                test @"unit\general\test_lb_ub_float.mzn"

            [<Fact>]
            let ``test test_let_complex`` () =
                test @"unit\general\test_let_complex.mzn"

            [<Fact>]
            let ``test test_let_par_array`` () =
                test @"unit\general\test_let_par_array.mzn"

            [<Fact>]
            let ``test test_let_simple`` () =
                test @"unit\general\test_let_simple.mzn"

            [<Fact>]
            let ``test test_let_var_array`` () =
                test @"unit\general\test_let_var_array.mzn"

            [<Fact>]
            let ``test test_let_with_annotation`` () =
                test @"unit\general\test_let_with_annotation.mzn"

            [<Fact>]
            let ``test test_min_var_array`` () =
                test @"unit\general\test_min_var_array.mzn"

            [<Fact>]
            let ``test test_negated_and`` () =
                test @"unit\general\test_negated_and.mzn"

            [<Fact>]
            let ``test test_negated_and_or`` () =
                test @"unit\general\test_negated_and_or.mzn"

            [<Fact>]
            let ``test test_negated_let_bad`` () =
                test @"unit\general\test_negated_let_bad.mzn"

            [<Fact>]
            let ``test test_negated_let_good`` () =
                test @"unit\general\test_negated_let_good.mzn"

            [<Fact>]
            let ``test test_negated_let_good_2`` () =
                test @"unit\general\test_negated_let_good_2.mzn"

            [<Fact>]
            let ``test test_negated_or`` () =
                test @"unit\general\test_negated_or.mzn"

            [<Fact>]
            let ``test test_par_set_element`` () =
                test @"unit\general\test_par_set_element.mzn"

            [<Fact>]
            let ``test test_par_set_id_array_index_sets`` () =
                test @"unit\general\test_par_set_id_array_index_sets.mzn"

            [<Fact>]
            let ``test test_queens`` () =
                test @"unit\general\test_queens.mzn"

            [<Fact>]
            let ``test test_reified_element_constraint`` () =
                test @"unit\general\test_reified_element_constraint.mzn"

            [<Fact>]
            let ``test test_reified_let_bad`` () =
                test @"unit\general\test_reified_let_bad.mzn"

            [<Fact>]
            let ``test test_reified_let_good`` () =
                test @"unit\general\test_reified_let_good.mzn"

            [<Fact>]
            let ``test test_rounding_a`` () =
                test @"unit\general\test_rounding_a.mzn"

            [<Fact>]
            let ``test test_rounding_b`` () =
                test @"unit\general\test_rounding_b.mzn"

            [<Fact>]
            let ``test test_rounding_c`` () =
                test @"unit\general\test_rounding_c.mzn"

            [<Fact>]
            let ``test test_same`` () =
                test @"unit\general\test_same.mzn"

            [<Fact>]
            let ``test test_set_inequality_par`` () =
                test @"unit\general\test_set_inequality_par.mzn"

            [<Fact>]
            let ``test test_set_lt_1`` () =
                test @"unit\general\test_set_lt_1.mzn"

            [<Fact>]
            let ``test test_set_lt_2`` () =
                test @"unit\general\test_set_lt_2.mzn"

            [<Fact>]
            let ``test test_set_lt_3`` () =
                test @"unit\general\test_set_lt_3.mzn"

            [<Fact>]
            let ``test test_string_array_var`` () =
                test @"unit\general\test_string_array_var.mzn"

            [<Fact>]
            let ``test test_string_cmp`` () =
                test @"unit\general\test_string_cmp.mzn"

            [<Fact>]
            let ``test test_string_var`` () =
                test @"unit\general\test_string_var.mzn"

            [<Fact>]
            let ``test test_string_with_quote`` () =
                test @"unit\general\test_string_with_quote.mzn"

            [<Fact>]
            let ``test test_times_int_float_eq`` () =
                test @"unit\general\test_times_int_float_eq.mzn"

            [<Fact>]
            let ``test test_times_int_float_eq__defaultopt`` () =
                test @"unit\general\test_times_int_float_eq__defaultopt.mzn"

            [<Fact>]
            let ``test test_to_enum`` () =
                test @"unit\general\test_to_enum.mzn"

            [<Fact>]
            let ``test test_undefined_enum`` () =
                test @"unit\general\test_undefined_enum.mzn"

            [<Fact>]
            let ``test test_var_array`` () =
                test @"unit\general\test_var_array.mzn"

            [<Fact>]
            let ``test test_var_array_access`` () =
                test @"unit\general\test_var_array_access.mzn"

            [<Fact>]
            let ``test test_var_prod`` () =
                test @"unit\general\test_var_prod.mzn"

            [<Fact>]
            let ``test test_var_set_assignment`` () =
                test @"unit\general\test_var_set_assignment.mzn"

            [<Fact>]
            let ``test test_var_set_element`` () =
                test @"unit\general\test_var_set_element.mzn"

            [<Fact>]
            let ``test unicode_file_name_μ`` () =
                test @"unit\general\unicode_file_name_μ.mzn"

            [<Fact>]
            let ``test wolfgoatetc`` () =
                test @"unit\general\wolfgoatetc.mzn"

            [<Fact>]
            let ``test xorall_bv`` () =
                test @"unit\general\xorall_bv.mzn"

        module ``Json`` =

            [<Fact>]
            let ``test anon_enum_json`` () =
                test @"unit\json\anon_enum_json.mzn"

            [<Fact>]
            let ``test coerce_enum_str`` () =
                test @"unit\json\coerce_enum_str.mzn"

            [<Fact>]
            let ``test coerce_enum_str_err`` () =
                test @"unit\json\coerce_enum_str_err.mzn"

            [<Fact>]
            let ``test coerce_indices`` () =
                test @"unit\json\coerce_indices.mzn"

            [<Fact>]
            let ``test coerce_set`` () =
                test @"unit\json\coerce_set.mzn"

            [<Fact>]
            let ``test enum_array_xd`` () =
                test @"unit\json\enum_array_xd.mzn"

            [<Fact>]
            let ``test enum_constructor_basic`` () =
                test @"unit\json\enum_constructor_basic.mzn"

            [<Fact>]
            let ``test enum_constructor_basic_2`` () =
                test @"unit\json\enum_constructor_basic_2.mzn"

            [<Fact>]
            let ``test enum_constructor_int`` () =
                test @"unit\json\enum_constructor_int.mzn"

            [<Fact>]
            let ``test enum_constructor_nested`` () =
                test @"unit\json\enum_constructor_nested.mzn"

            [<Fact>]
            let ``test enum_escaping`` () =
                test @"unit\json\enum_escaping.mzn"

            [<Fact>]
            let ``test float_json_exponent`` () =
                test @"unit\json\float_json_exponent.mzn"

            [<Fact>]
            let ``test json_array2d_set`` () =
                test @"unit\json\json_array2d_set.mzn"

            [<Fact>]
            let ``test json_enum_def`` () =
                test @"unit\json\json_enum_def.mzn"

            [<Fact>]
            let ``test json_input_1`` () =
                test @"unit\json\json_input_1.mzn"

            [<Fact>]
            let ``test json_input_multidim_enum_str`` () =
                test @"unit\json\json_input_multidim_enum_str.mzn"

            [<Fact>]
            let ``test json_input_set`` () =
                test @"unit\json\json_input_set.mzn"

            [<Fact>]
            let ``test json_input_str`` () =
                test @"unit\json\json_input_str.mzn"

            [<Fact>]
            let ``test json_unicode_escapes`` () =
                test @"unit\json\json_unicode_escapes.mzn"

            [<Fact>]
            let ``test mult_dim_enum`` () =
                test @"unit\json\mult_dim_enum.mzn"

            [<Fact>]
            let ``test record_json_input`` () =
                test @"unit\json\record_json_input.mzn"

            [<Fact>]
            let ``test tuple_json_input`` () =
                test @"unit\json\tuple_json_input.mzn"

        module ``Optional`` =

            [<Fact>]
            let ``test conj_absent_1`` () =
                test @"unit\optional\conj_absent_1.mzn"

            [<Fact>]
            let ``test conj_absent_2`` () =
                test @"unit\optional\conj_absent_2.mzn"

            [<Fact>]
            let ``test test-opt-binop-flatten`` () =
                test @"unit\optional\test-opt-binop-flatten.mzn"

            [<Fact>]
            let ``test test-opt-bool-1`` () =
                test @"unit\optional\test-opt-bool-1.mzn"

            [<Fact>]
            let ``test test-opt-bool-2`` () =
                test @"unit\optional\test-opt-bool-2.mzn"

            [<Fact>]
            let ``test test-opt-bool-3`` () =
                test @"unit\optional\test-opt-bool-3.mzn"

            [<Fact>]
            let ``test test-opt-bool-4`` () =
                test @"unit\optional\test-opt-bool-4.mzn"

            [<Fact>]
            let ``test test-opt-bool-5`` () =
                test @"unit\optional\test-opt-bool-5.mzn"

            [<Fact>]
            let ``test test-opt-bool-6`` () =
                test @"unit\optional\test-opt-bool-6.mzn"

            [<Fact>]
            let ``test test-opt-compute-bounds`` () =
                test @"unit\optional\test-opt-compute-bounds.mzn"

            [<Fact>]
            let ``test test-opt-float-1`` () =
                test @"unit\optional\test-opt-float-1.mzn"

            [<Fact>]
            let ``test test-opt-float-2`` () =
                test @"unit\optional\test-opt-float-2.mzn"

            [<Fact>]
            let ``test test-opt-if-then-else`` () =
                test @"unit\optional\test-opt-if-then-else.mzn"

            [<Fact>]
            let ``test test-opt-int-1`` () =
                test @"unit\optional\test-opt-int-1.mzn"

            [<Fact>]
            let ``test test_bug_456`` () =
                test @"unit\optional\test_bug_456.mzn"

            [<Fact>]
            let ``test test_count_set`` () =
                test @"unit\optional\test_count_set.mzn"

            [<Fact>]
            let ``test test_deopt_absent`` () =
                test @"unit\optional\test_deopt_absent.mzn"

            [<Fact>]
            let ``test test_if_then_else_opt_bool`` () =
                test @"unit\optional\test_if_then_else_opt_bool.mzn"

            [<Fact>]
            let ``test test_if_then_else_opt_float`` () =
                test @"unit\optional\test_if_then_else_opt_float.mzn"

            [<Fact>]
            let ``test test_if_then_else_opt_int`` () =
                test @"unit\optional\test_if_then_else_opt_int.mzn"

            [<Fact>]
            let ``test test_if_then_else_var_opt_bool`` () =
                test @"unit\optional\test_if_then_else_var_opt_bool.mzn"

            [<Fact>]
            let ``test test_if_then_else_var_opt_float`` () =
                test @"unit\optional\test_if_then_else_var_opt_float.mzn"

            [<Fact>]
            let ``test test_if_then_else_var_opt_int`` () =
                test @"unit\optional\test_if_then_else_var_opt_int.mzn"

            [<Fact>]
            let ``test test_optional_not_absent`` () =
                test @"unit\optional\test_optional_not_absent.mzn"

            [<Fact>]
            let ``test test_opt_comprehension`` () =
                test @"unit\optional\test_opt_comprehension.mzn"

            [<Fact>]
            let ``test test_opt_dom_empty`` () =
                test @"unit\optional\test_opt_dom_empty.mzn"

            [<Fact>]
            let ``test test_opt_dom_empty_no_absent_zero`` () =
                test @"unit\optional\test_opt_dom_empty_no_absent_zero.mzn"

            [<Fact>]
            let ``test test_opt_max`` () =
                test @"unit\optional\test_opt_max.mzn"

            [<Fact>]
            let ``test test_opt_min`` () =
                test @"unit\optional\test_opt_min.mzn"

        module ``Output`` =

            [<Fact>]
            let ``test arg-reif-output`` () =
                test @"unit\output\arg-reif-output.mzn"

            [<Fact>]
            let ``test array-ann`` () =
                test @"unit\output\array-ann.mzn"

            [<Fact>]
            let ``test bug288a`` () =
                test @"unit\output\bug288a.mzn"

            [<Fact>]
            let ``test bug288b`` () =
                test @"unit\output\bug288b.mzn"

            [<Fact>]
            let ``test bug288c`` () =
                test @"unit\output\bug288c.mzn"

            [<Fact>]
            let ``test ctx_ann`` () =
                test @"unit\output\ctx_ann.mzn"

            [<Fact>]
            let ``test dzn_output_array`` () =
                test @"unit\output\dzn_output_array.mzn"

            [<Fact>]
            let ``test json_ann`` () =
                test @"unit\output\json_ann.mzn"

            [<Fact>]
            let ``test mzn_bottom1`` () =
                test @"unit\output\mzn_bottom1.mzn"

            [<Fact>]
            let ``test mzn_bottom2`` () =
                test @"unit\output\mzn_bottom2.mzn"

            [<Fact>]
            let ``test mzn_bottom3`` () =
                test @"unit\output\mzn_bottom3.mzn"

            [<Fact>]
            let ``test mzn_bottom4`` () =
                test @"unit\output\mzn_bottom4.mzn"

            [<Fact>]
            let ``test mzn_bottom5`` () =
                test @"unit\output\mzn_bottom5.mzn"

            [<Fact>]
            let ``test mzn_bottom6`` () =
                test @"unit\output\mzn_bottom6.mzn"

            [<Fact>]
            let ``test output_annotations_1`` () =
                test @"unit\output\output_annotations_1.mzn"

            [<Fact>]
            let ``test output_annotations_2`` () =
                test @"unit\output\output_annotations_2.mzn"

            [<Fact>]
            let ``test output_annotations_3`` () =
                test @"unit\output\output_annotations_3.mzn"

            [<Fact>]
            let ``test output_annotations_4`` () =
                test @"unit\output\output_annotations_4.mzn"

            [<Fact>]
            let ``test output_sections_1`` () =
                test @"unit\output\output_sections_1.mzn"

            [<Fact>]
            let ``test output_sections_2`` () =
                test @"unit\output\output_sections_2.mzn"

            [<Fact>]
            let ``test output_sections_3`` () =
                test @"unit\output\output_sections_3.mzn"

            [<Fact>]
            let ``test output_sections_4`` () =
                test @"unit\output\output_sections_4.mzn"

            [<Fact>]
            let ``test output_sections_5`` () =
                test @"unit\output\output_sections_5.mzn"

            [<Fact>]
            let ``test output_sections_6`` () =
                test @"unit\output\output_sections_6.mzn"

            [<Fact>]
            let ``test quoted_id_ozn`` () =
                test @"unit\output\quoted_id_ozn.mzn"

            [<Fact>]
            let ``test show2d_empty`` () =
                test @"unit\output\show2d_empty.mzn"

            [<Fact>]
            let ``test show3d_empty`` () =
                test @"unit\output\show3d_empty.mzn"

            [<Fact>]
            let ``test show_empty`` () =
                test @"unit\output\show_empty.mzn"

            [<Fact>]
            let ``test show_float`` () =
                test @"unit\output\show_float.mzn"

            [<Fact>]
            let ``test show_float_set`` () =
                test @"unit\output\show_float_set.mzn"

            [<Fact>]
            let ``test show_int`` () =
                test @"unit\output\show_int.mzn"

            [<Fact>]
            let ``test show_int_set`` () =
                test @"unit\output\show_int_set.mzn"

            [<Fact>]
            let ``test test-in-output`` () =
                test @"unit\output\test-in-output.mzn"

            [<Fact>]
            let ``test var_enum`` () =
                test @"unit\output\var_enum.mzn"

            [<Fact>]
            let ``test very_empty`` () =
                test @"unit\output\very_empty.mzn"

            [<Fact>]
            let ``test very_empty_set`` () =
                test @"unit\output\very_empty_set.mzn"

        module ``Param File`` =

            [<Fact>]
            let ``test param_file_array`` () =
                test @"unit\param_file\param_file_array.mzn"

            [<Fact>]
            let ``test param_file_blacklist`` () =
                test @"unit\param_file\param_file_blacklist.mzn"

            [<Fact>]
            let ``test param_file_nested_object`` () =
                test @"unit\param_file\param_file_nested_object.mzn"

            [<Fact>]
            let ``test param_file_recursive`` () =
                test @"unit\param_file\param_file_recursive.mzn"

            [<Fact>]
            let ``test param_file_resolution`` () =
                test @"unit\param_file\param_file_resolution.mzn"

        module ``Regression`` =

            [<Fact>]
            let ``test absent_id_crash`` () =
                test @"unit\regression\absent_id_crash.mzn"

            [<Fact>]
            let ``test abs_bug`` () =
                test @"unit\regression\abs_bug.mzn"

            [<Fact>]
            let ``test arg-reif-array-float`` () =
                test @"unit\regression\arg-reif-array-float.mzn"

            [<Fact>]
            let ``test arg-reif-array-int`` () =
                test @"unit\regression\arg-reif-array-int.mzn"

            [<Fact>]
            let ``test arg-reif-float`` () =
                test @"unit\regression\arg-reif-float.mzn"

            [<Fact>]
            let ``test arg-reif-int-set`` () =
                test @"unit\regression\arg-reif-int-set.mzn"

            [<Fact>]
            let ``test arg-reif-int`` () =
                test @"unit\regression\arg-reif-int.mzn"

            [<Fact>]
            let ``test array_of_empty_sets`` () =
                test @"unit\regression\array_of_empty_sets.mzn"

            [<Fact>]
            let ``test array_set_element_nosets`` () =
                test @"unit\regression\array_set_element_nosets.mzn"

            [<Fact>]
            let ``test array_var_set_element_nosets`` () =
                test @"unit\regression\array_var_set_element_nosets.mzn"

            [<Fact>]
            let ``test assign_reverse_map`` () =
                test @"unit\regression\assign_reverse_map.mzn"

            [<Fact>]
            let ``test big_array_lit`` () =
                test @"unit\regression\big_array_lit.mzn"

            [<Fact>]
            let ``test bind-defines-var`` () =
                test @"unit\regression\bind-defines-var.mzn"

            [<Fact>]
            let ``test binop_mult_gclock`` () =
                test @"unit\regression\binop_mult_gclock.mzn"

            [<Fact>]
            let ``test bizarre`` () =
                test @"unit\regression\bizarre.mzn"

            [<Fact>]
            let ``test bool2float_let`` () =
                test @"unit\regression\bool2float_let.mzn"

            [<Fact>]
            let ``test bounds_for_linear_01_max_0`` () =
                test @"unit\regression\bounds_for_linear_01_max_0.mzn"

            [<Fact>]
            let ``test bounds_for_linear_01_max_1`` () =
                test @"unit\regression\bounds_for_linear_01_max_1.mzn"

            [<Fact>]
            let ``test bounds_for_linear_01_min_0`` () =
                test @"unit\regression\bounds_for_linear_01_min_0.mzn"

            [<Fact>]
            let ``test bounds_for_linear_01_min_1`` () =
                test @"unit\regression\bounds_for_linear_01_min_1.mzn"

            [<Fact>]
            let ``test bug109`` () =
                test @"unit\regression\bug109.mzn"

            [<Fact>]
            let ``test bug110`` () =
                test @"unit\regression\bug110.mzn"

            [<Fact>]
            let ``test bug131`` () =
                test @"unit\regression\bug131.mzn"

            [<Fact>]
            let ``test bug141`` () =
                test @"unit\regression\bug141.mzn"

            [<Fact>]
            let ``test bug202`` () =
                test @"unit\regression\bug202.mzn"

            [<Fact>]
            let ``test bug212`` () =
                test @"unit\regression\bug212.mzn"

            [<Fact>]
            let ``test bug222`` () =
                test @"unit\regression\bug222.mzn"

            [<Fact>]
            let ``test bug244`` () =
                test @"unit\regression\bug244.mzn"

            [<Fact>]
            let ``test bug256`` () =
                test @"unit\regression\bug256.mzn"

            [<Fact>]
            let ``test bug256b`` () =
                test @"unit\regression\bug256b.mzn"

            [<Fact>]
            let ``test bug259`` () =
                test @"unit\regression\bug259.mzn"

            [<Fact>]
            let ``test bug269`` () =
                test @"unit\regression\bug269.mzn"

            [<Fact>]
            let ``test bug282`` () =
                test @"unit\regression\bug282.mzn"

            [<Fact>]
            let ``test bug283`` () =
                test @"unit\regression\bug283.mzn"

            [<Fact>]
            let ``test bug284`` () =
                test @"unit\regression\bug284.mzn"

            [<Fact>]
            let ``test bug287`` () =
                test @"unit\regression\bug287.mzn"

            [<Fact>]
            let ``test bug290_orig`` () =
                test @"unit\regression\bug290_orig.mzn"

            [<Fact>]
            let ``test bug290_simple`` () =
                test @"unit\regression\bug290_simple.mzn"

            [<Fact>]
            let ``test bug312`` () =
                test @"unit\regression\bug312.mzn"

            [<Fact>]
            let ``test bug318_orig`` () =
                test @"unit\regression\bug318_orig.mzn"

            [<Fact>]
            let ``test bug335`` () =
                test @"unit\regression\bug335.mzn"

            [<Fact>]
            let ``test bug337`` () =
                test @"unit\regression\bug337.mzn"

            [<Fact>]
            let ``test bug337_mod`` () =
                test @"unit\regression\bug337_mod.mzn"

            [<Fact>]
            let ``test bug341`` () =
                test @"unit\regression\bug341.mzn"

            [<Fact>]
            let ``test bug347`` () =
                test @"unit\regression\bug347.mzn"

            [<Fact>]
            let ``test bug380`` () =
                test @"unit\regression\bug380.mzn"

            [<Fact>]
            let ``test bug45`` () =
                test @"unit\regression\bug45.mzn"

            [<Fact>]
            let ``test bug52`` () =
                test @"unit\regression\bug52.mzn"

            [<Fact>]
            let ``test bug532`` () =
                test @"unit\regression\bug532.mzn"

            [<Fact>]
            let ``test bug534`` () =
                test @"unit\regression\bug534.mzn"

            [<Fact>]
            let ``test bug536`` () =
                test @"unit\regression\bug536.mzn"

            [<Fact>]
            let ``test bug552`` () =
                test @"unit\regression\bug552.mzn"

            [<Fact>]
            let ``test bug565`` () =
                test @"unit\regression\bug565.mzn"

            [<Fact>]
            let ``test bug570`` () =
                test @"unit\regression\bug570.mzn"

            [<Fact>]
            let ``test bug620`` () =
                test @"unit\regression\bug620.mzn"

            [<Fact>]
            let ``test bug635`` () =
                test @"unit\regression\bug635.mzn"

            [<Fact>]
            let ``test bug67`` () =
                test @"unit\regression\bug67.mzn"

            [<Fact>]
            let ``test bug68b`` () =
                test @"unit\regression\bug68b.mzn"

            [<Fact>]
            let ``test bug69_1`` () =
                test @"unit\regression\bug69_1.mzn"

            [<Fact>]
            let ``test bug69_2`` () =
                test @"unit\regression\bug69_2.mzn"

            [<Fact>]
            let ``test bug69_3`` () =
                test @"unit\regression\bug69_3.mzn"

            [<Fact>]
            let ``test bug69_4`` () =
                test @"unit\regression\bug69_4.mzn"

            [<Fact>]
            let ``test bug70`` () =
                test @"unit\regression\bug70.mzn"

            [<Fact>]
            let ``test bug71_1`` () =
                test @"unit\regression\bug71_1.mzn"

            [<Fact>]
            let ``test bug71_2`` () =
                test @"unit\regression\bug71_2.mzn"

            [<Fact>]
            let ``test bug82`` () =
                test @"unit\regression\bug82.mzn"

            [<Fact>]
            let ``test bug85`` () =
                test @"unit\regression\bug85.mzn"

            [<Fact>]
            let ``test bug_08`` () =
                test @"unit\regression\bug_08.mzn"

            [<Fact>]
            let ``test bug_629`` () =
                test @"unit\regression\bug_629.mzn"

            [<Fact>]
            let ``test bug_empty_enum_extension`` () =
                test @"unit\regression\bug_empty_enum_extension.mzn"

            [<Fact>]
            let ``test bug_opt_polymorphic`` () =
                test @"unit\regression\bug_opt_polymorphic.mzn"

            [<Fact>]
            let ``test bug_r7995`` () =
                test @"unit\regression\bug_r7995.mzn"

            [<Fact>]
            let ``test cardinality_atmost_partition`` () =
                test @"unit\regression\cardinality_atmost_partition.mzn"

            [<Fact>]
            let ``test card_flatten_lb`` () =
                test @"unit\regression\card_flatten_lb.mzn"

            [<Fact>]
            let ``test change_partition`` () =
                test @"unit\regression\change_partition.mzn"

            [<Fact>]
            let ``test checker_mzn_check_var`` () =
                test @"unit\regression\checker_mzn_check_var.mzn"

            [<Fact>]
            let ``test checker_opt`` () =
                test @"unit\regression\checker_opt.mzn"

            [<Fact>]
            let ``test checker_params`` () =
                test @"unit\regression\checker_params.mzn"

            [<Fact>]
            let ``test checker_same_var`` () =
                test @"unit\regression\checker_same_var.mzn"

            [<Fact>]
            let ``test checker_var_bug`` () =
                test @"unit\regression\checker_var_bug.mzn"

            [<Fact>]
            let ``test check_dom_float_array`` () =
                test @"unit\regression\check_dom_float_array.mzn"

            [<Fact>]
            let ``test coerce_set_to_array_1`` () =
                test @"unit\regression\coerce_set_to_array_1.mzn"

            [<Fact>]
            let ``test coerce_set_to_array_2`` () =
                test @"unit\regression\coerce_set_to_array_2.mzn"

            [<Fact>]
            let ``test coercion_par`` () =
                test @"unit\regression\coercion_par.mzn"

            [<Fact>]
            let ``test comprehension_where`` () =
                test @"unit\regression\comprehension_where.mzn"

            [<Fact>]
            let ``test comp_in_empty_set`` () =
                test @"unit\regression\comp_in_empty_set.mzn"

            [<Fact>]
            let ``test constructor_of_set`` () =
                test @"unit\regression\constructor_of_set.mzn"

            [<Fact>]
            let ``test cse_array_lit`` () =
                test @"unit\regression\cse_array_lit.mzn"

            [<Fact>]
            let ``test cyclic_include`` () =
                test @"unit\regression\cyclic_include.mzn"

            [<Fact>]
            let ``test decision_tree_binary`` () =
                test @"unit\regression\decision_tree_binary.mzn"

            [<Fact>]
            let ``test empty-array1d`` () =
                test @"unit\regression\empty-array1d.mzn"

            [<Fact>]
            let ``test enigma_1568`` () =
                test @"unit\regression\enigma_1568.mzn"

            [<Fact>]
            let ``test error_in_comprehension`` () =
                test @"unit\regression\error_in_comprehension.mzn"

            [<Fact>]
            let ``test flatten_comp_in`` () =
                test @"unit\regression\flatten_comp_in.mzn"

            [<Fact>]
            let ``test flatten_comp_in2`` () =
                test @"unit\regression\flatten_comp_in2.mzn"

            [<Fact>]
            let ``test flat_cv_call`` () =
                test @"unit\regression\flat_cv_call.mzn"

            [<Fact>]
            let ``test flat_cv_let`` () =
                test @"unit\regression\flat_cv_let.mzn"

            [<Fact>]
            let ``test flat_set_lit`` () =
                test @"unit\regression\flat_set_lit.mzn"

            [<Fact>]
            let ``test flipstrip_simple`` () =
                test @"unit\regression\flipstrip_simple.mzn"

            [<Fact>]
            let ``test float_ceil_floor`` () =
                test @"unit\regression\float_ceil_floor.mzn"

            [<Fact>]
            let ``test float_div_crash`` () =
                test @"unit\regression\float_div_crash.mzn"

            [<Fact>]
            let ``test float_mod_crash`` () =
                test @"unit\regression\float_mod_crash.mzn"

            [<Fact>]
            let ``test float_opt_crash`` () =
                test @"unit\regression\float_opt_crash.mzn"

            [<Fact>]
            let ``test follow_id_absent_crash`` () =
                test @"unit\regression\follow_id_absent_crash.mzn"

            [<Fact>]
            let ``test github537`` () =
                test @"unit\regression\github537.mzn"

            [<Fact>]
            let ``test github_638_reduced`` () =
                test @"unit\regression\github_638_reduced.mzn"

            [<Fact>]
            let ``test github_639_part1`` () =
                test @"unit\regression\github_639_part1.mzn"

            [<Fact>]
            let ``test github_639_part2`` () =
                test @"unit\regression\github_639_part2.mzn"

            [<Fact>]
            let ``test github_644_a`` () =
                test @"unit\regression\github_644_a.mzn"

            [<Fact>]
            let ``test github_644_b`` () =
                test @"unit\regression\github_644_b.mzn"

            [<Fact>]
            let ``test github_644_c`` () =
                test @"unit\regression\github_644_c.mzn"

            [<Fact>]
            let ``test github_644_d`` () =
                test @"unit\regression\github_644_d.mzn"

            [<Fact>]
            let ``test github_644_e`` () =
                test @"unit\regression\github_644_e.mzn"

            [<Fact>]
            let ``test github_646`` () =
                test @"unit\regression\github_646.mzn"

            [<Fact>]
            let ``test github_648_par_array_decl`` () =
                test @"unit\regression\github_648_par_array_decl.mzn"

            [<Fact>]
            let ``test github_648_par_decl`` () =
                test @"unit\regression\github_648_par_decl.mzn"

            [<Fact>]
            let ``test github_656`` () =
                test @"unit\regression\github_656.mzn"

            [<Fact>]
            let ``test github_660a`` () =
                test @"unit\regression\github_660a.mzn"

            [<Fact>]
            let ``test github_660b`` () =
                test @"unit\regression\github_660b.mzn"

            [<Fact>]
            let ``test github_661_part1`` () =
                test @"unit\regression\github_661_part1.mzn"

            [<Fact>]
            let ``test github_661_part2`` () =
                test @"unit\regression\github_661_part2.mzn"

            [<Fact>]
            let ``test github_664`` () =
                test @"unit\regression\github_664.mzn"

            [<Fact>]
            let ``test github_666`` () =
                test @"unit\regression\github_666.mzn"

            [<Fact>]
            let ``test github_667`` () =
                test @"unit\regression\github_667.mzn"

            [<Fact>]
            let ``test github_668`` () =
                test @"unit\regression\github_668.mzn"

            [<Fact>]
            let ``test github_669`` () =
                test @"unit\regression\github_669.mzn"

            [<Fact>]
            let ``test github_670`` () =
                test @"unit\regression\github_670.mzn"

            [<Fact>]
            let ``test github_671`` () =
                test @"unit\regression\github_671.mzn"

            [<Fact>]
            let ``test github_673`` () =
                test @"unit\regression\github_673.mzn"

            [<Fact>]
            let ``test github_674`` () =
                test @"unit\regression\github_674.mzn"

            [<Fact>]
            let ``test github_675a`` () =
                test @"unit\regression\github_675a.mzn"

            [<Fact>]
            let ``test github_675b`` () =
                test @"unit\regression\github_675b.mzn"

            [<Fact>]
            let ``test github_680`` () =
                test @"unit\regression\github_680.mzn"

            [<Fact>]
            let ``test github_681`` () =
                test @"unit\regression\github_681.mzn"

            [<Fact>]
            let ``test github_683`` () =
                test @"unit\regression\github_683.mzn"

            [<Fact>]
            let ``test github_685`` () =
                test @"unit\regression\github_685.mzn"

            [<Fact>]
            let ``test github_687`` () =
                test @"unit\regression\github_687.mzn"

            [<Fact>]
            let ``test github_691`` () =
                test @"unit\regression\github_691.mzn"

            [<Fact>]
            let ``test github_693_part1`` () =
                test @"unit\regression\github_693_part1.mzn"

            [<Fact>]
            let ``test github_693_part2`` () =
                test @"unit\regression\github_693_part2.mzn"

            [<Fact>]
            let ``test github_695`` () =
                test @"unit\regression\github_695.mzn"

            [<Fact>]
            let ``test github_700`` () =
                test @"unit\regression\github_700.mzn"

            [<Fact>]
            let ``test github_700_bad_sol`` () =
                test @"unit\regression\github_700_bad_sol.mzn"

            [<Fact>]
            let ``test hundred_doors_unoptimized`` () =
                test @"unit\regression\hundred_doors_unoptimized.mzn"

            [<Fact>]
            let ``test if_then_else_absent`` () =
                test @"unit\regression\if_then_else_absent.mzn"

            [<Fact>]
            let ``test int_times`` () =
                test @"unit\regression\int_times.mzn"

            [<Fact>]
            let ``test is_fixed`` () =
                test @"unit\regression\is_fixed.mzn"

            [<Fact>]
            let ``test is_fixed_comp`` () =
                test @"unit\regression\is_fixed_comp.mzn"

            [<Fact>]
            let ``test lb_ub_dom_array_opt`` () =
                test @"unit\regression\lb_ub_dom_array_opt.mzn"

            [<Fact>]
            let ``test let_domain_from_generator`` () =
                test @"unit\regression\let_domain_from_generator.mzn"

            [<Fact>]
            let ``test linear_bool_elem_bug`` () =
                test @"unit\regression\linear_bool_elem_bug.mzn"

            [<Fact>]
            let ``test makepar_output`` () =
                test @"unit\regression\makepar_output.mzn"

            [<Fact>]
            let ``test multi_goal_hierarchy_error`` () =
                test @"unit\regression\multi_goal_hierarchy_error.mzn"

            [<Fact>]
            let ``test nested_clause`` () =
                test @"unit\regression\nested_clause.mzn"

            [<Fact>]
            let ``test non-set-array-ti-location`` () =
                test @"unit\regression\non-set-array-ti-location.mzn"

            [<Fact>]
            let ``test non_pos_pow`` () =
                test @"unit\regression\non_pos_pow.mzn"

            [<Fact>]
            let ``test nosets_369`` () =
                test @"unit\regression\nosets_369.mzn"

            [<Fact>]
            let ``test nosets_set_search`` () =
                test @"unit\regression\nosets_set_search.mzn"

            [<Fact>]
            let ``test no_macro`` () =
                test @"unit\regression\no_macro.mzn"

            [<Fact>]
            let ``test opt_minmax`` () =
                test @"unit\regression\opt_minmax.mzn"

            [<Fact>]
            let ``test opt_noncontiguous_domain`` () =
                test @"unit\regression\opt_noncontiguous_domain.mzn"

            [<Fact>]
            let ``test opt_removed_items`` () =
                test @"unit\regression\opt_removed_items.mzn"

            [<Fact>]
            let ``test output_2d_array_enum`` () =
                test @"unit\regression\output_2d_array_enum.mzn"

            [<Fact>]
            let ``test output_fn_toplevel_var`` () =
                test @"unit\regression\output_fn_toplevel_var.mzn"

            [<Fact>]
            let ``test output_only_fn`` () =
                test @"unit\regression\output_only_fn.mzn"

            [<Fact>]
            let ``test output_only_no_rhs`` () =
                test @"unit\regression\output_only_no_rhs.mzn"

            [<Fact>]
            let ``test overloading`` () =
                test @"unit\regression\overloading.mzn"

            [<Fact>]
            let ``test parser_location`` () =
                test @"unit\regression\parser_location.mzn"

            [<Fact>]
            let ``test parse_assignments`` () =
                test @"unit\regression\parse_assignments.mzn"

            [<Fact>]
            let ``test par_opt_dom`` () =
                test @"unit\regression\par_opt_dom.mzn"

            [<Fact>]
            let ``test par_opt_equal`` () =
                test @"unit\regression\par_opt_equal.mzn"

            [<Fact>]
            let ``test pow_undefined`` () =
                test @"unit\regression\pow_undefined.mzn"

            [<Fact>]
            let ``test pred_param_r7550`` () =
                test @"unit\regression\pred_param_r7550.mzn"

            [<Fact>]
            let ``test round`` () =
                test @"unit\regression\round.mzn"

            [<Fact>]
            let ``test seq_search_bug`` () =
                test @"unit\regression\seq_search_bug.mzn"

            [<Fact>]
            let ``test set_inequality_par`` () =
                test @"unit\regression\set_inequality_par.mzn"

            [<Fact>]
            let ``test slice_enum_indexset`` () =
                test @"unit\regression\slice_enum_indexset.mzn"

            [<Fact>]
            let ``test string-test-arg`` () =
                test @"unit\regression\string-test-arg.mzn"

            [<Fact>]
            let ``test subsets_100`` () =
                test @"unit\regression\subsets_100.mzn"

            [<Fact>]
            let ``test test_annotation_on_exists`` () =
                test @"unit\regression\test_annotation_on_exists.mzn"

            [<Fact>]
            let ``test test_bool2int`` () =
                test @"unit\regression\test_bool2int.mzn"

            [<Fact>]
            let ``test test_bug218`` () =
                test @"unit\regression\test_bug218.mzn"

            [<Fact>]
            let ``test test_bug359`` () =
                test @"unit\regression\test_bug359.mzn"

            [<Fact>]
            let ``test test_bug45`` () =
                test @"unit\regression\test_bug45.mzn"

            [<Fact>]
            let ``test test_bug53`` () =
                test @"unit\regression\test_bug53.mzn"

            [<Fact>]
            let ``test test_bug54`` () =
                test @"unit\regression\test_bug54.mzn"

            [<Fact>]
            let ``test test_bug55`` () =
                test @"unit\regression\test_bug55.mzn"

            [<Fact>]
            let ``test test_bug57`` () =
                test @"unit\regression\test_bug57.mzn"

            [<Fact>]
            let ``test test_bug65`` () =
                test @"unit\regression\test_bug65.mzn"

            [<Fact>]
            let ``test test_bug66`` () =
                test @"unit\regression\test_bug66.mzn"

            [<Fact>]
            let ``test test_bug67`` () =
                test @"unit\regression\test_bug67.mzn"

            [<Fact>]
            let ``test test_bug70`` () =
                test @"unit\regression\test_bug70.mzn"

            [<Fact>]
            let ``test test_bug71`` () =
                test @"unit\regression\test_bug71.mzn"

            [<Fact>]
            let ``test test_bug72`` () =
                test @"unit\regression\test_bug72.mzn"

            [<Fact>]
            let ``test test_bug_129`` () =
                test @"unit\regression\test_bug_129.mzn"

            [<Fact>]
            let ``test test_bug_476`` () =
                test @"unit\regression\test_bug_476.mzn"

            [<Fact>]
            let ``test test_bug_483`` () =
                test @"unit\regression\test_bug_483.mzn"

            [<Fact>]
            let ``test test_bug_493`` () =
                test @"unit\regression\test_bug_493.mzn"

            [<Fact>]
            let ``test test_bug_494`` () =
                test @"unit\regression\test_bug_494.mzn"

            [<Fact>]
            let ``test test_bug_520`` () =
                test @"unit\regression\test_bug_520.mzn"

            [<Fact>]
            let ``test test_bug_521`` () =
                test @"unit\regression\test_bug_521.mzn"

            [<Fact>]
            let ``test test_bug_527`` () =
                test @"unit\regression\test_bug_527.mzn"

            [<Fact>]
            let ``test test_bug_529`` () =
                test @"unit\regression\test_bug_529.mzn"

            [<Fact>]
            let ``test test_bug_588`` () =
                test @"unit\regression\test_bug_588.mzn"

            [<Fact>]
            let ``test test_bug_637`` () =
                test @"unit\regression\test_bug_637.mzn"

            [<Fact>]
            let ``test test_bug_array_sum_bounds`` () =
                test @"unit\regression\test_bug_array_sum_bounds.mzn"

            [<Fact>]
            let ``test test_bug_ite_array_eq`` () =
                test @"unit\regression\test_bug_ite_array_eq.mzn"

            [<Fact>]
            let ``test test_bug_pred_arg`` () =
                test @"unit\regression\test_bug_pred_arg.mzn"

            [<Fact>]
            let ``test test_equality_of_indirect_annotations`` () =
                test @"unit\regression\test_equality_of_indirect_annotations.mzn"

            [<Fact>]
            let ``test test_github_30`` () =
                test @"unit\regression\test_github_30.mzn"

            [<Fact>]
            let ``test test_multioutput`` () =
                test @"unit\regression\test_multioutput.mzn"

            [<Fact>]
            let ``test test_not_in`` () =
                test @"unit\regression\test_not_in.mzn"

            [<Fact>]
            let ``test test_output_array_of_set`` () =
                test @"unit\regression\test_output_array_of_set.mzn"

            [<Fact>]
            let ``test test_output_string_var`` () =
                test @"unit\regression\test_output_string_var.mzn"

            [<Fact>]
            let ``test test_parout`` () =
                test @"unit\regression\test_parout.mzn"

            [<Fact>]
            let ``test test_seq_precede_chain_set`` () =
                test @"unit\regression\test_seq_precede_chain_set.mzn"

            [<Fact>]
            let ``test test_slice_1d_array`` () =
                test @"unit\regression\test_slice_1d_array.mzn"

            [<Fact>]
            let ``test ti_error_location`` () =
                test @"unit\regression\ti_error_location.mzn"

            [<Fact>]
            let ``test ts_bug`` () =
                test @"unit\regression\ts_bug.mzn"

            [<Fact>]
            let ``test type_specialise_array_return`` () =
                test @"unit\regression\type_specialise_array_return.mzn"

            [<Fact>]
            let ``test var_bool_comp`` () =
                test @"unit\regression\var_bool_comp.mzn"

            [<Fact>]
            let ``test var_opt_unconstrained`` () =
                test @"unit\regression\var_opt_unconstrained.mzn"

            [<Fact>]
            let ``test var_self_assign_bug`` () =
                test @"unit\regression\var_self_assign_bug.mzn"

            [<Fact>]
            let ``test warm_start`` () =
                test @"unit\regression\warm_start.mzn"

            [<Fact>]
            let ``test where-forall-bug`` () =
                test @"unit\regression\where-forall-bug.mzn"

            [<Fact>]
            let ``test xor_mixed_context`` () =
                test @"unit\regression\xor_mixed_context.mzn"

        module ``Search`` =

            [<Fact>]
            let ``test int_choice_1`` () =
                test @"unit\search\int_choice_1.mzn"

            [<Fact>]
            let ``test int_choice_2`` () =
                test @"unit\search\int_choice_2.mzn"

            [<Fact>]
            let ``test int_choice_6`` () =
                test @"unit\search\int_choice_6.mzn"

            [<Fact>]
            let ``test int_var_select_1`` () =
                test @"unit\search\int_var_select_1.mzn"

            [<Fact>]
            let ``test int_var_select_2`` () =
                test @"unit\search\int_var_select_2.mzn"

            [<Fact>]
            let ``test int_var_select_3`` () =
                test @"unit\search\int_var_select_3.mzn"

            [<Fact>]
            let ``test int_var_select_4`` () =
                test @"unit\search\int_var_select_4.mzn"

            [<Fact>]
            let ``test int_var_select_6`` () =
                test @"unit\search\int_var_select_6.mzn"

            [<Fact>]
            let ``test test-ff1`` () =
                test @"unit\search\test-ff1.mzn"

            [<Fact>]
            let ``test test-ff2`` () =
                test @"unit\search\test-ff2.mzn"

            [<Fact>]
            let ``test test-ff3`` () =
                test @"unit\search\test-ff3.mzn"

            [<Fact>]
            let ``test test-large1`` () =
                test @"unit\search\test-large1.mzn"

            [<Fact>]
            let ``test test-med1`` () =
                test @"unit\search\test-med1.mzn"

            [<Fact>]
            let ``test test-small1`` () =
                test @"unit\search\test-small1.mzn"

        module ``Types`` =

            [<Fact>]
            let ``test alias`` () =
                test @"unit\types\alias.mzn"

            [<Fact>]
            let ``test alias_call`` () =
                test @"unit\types\alias_call.mzn"

            [<Fact>]
            let ``test alias_extern_dom`` () =
                test @"unit\types\alias_extern_dom.mzn"

            [<Fact>]
            let ``test alias_set_of_array`` () =
                test @"unit\types\alias_set_of_array.mzn"

            [<Fact>]
            let ``test comprehension_type`` () =
                test @"unit\types\comprehension_type.mzn"

            [<Fact>]
            let ``test cv_comprehension`` () =
                test @"unit\types\cv_comprehension.mzn"

            [<Fact>]
            let ``test enum_decl`` () =
                test @"unit\types\enum_decl.mzn"

            [<Fact>]
            let ``test github_647`` () =
                test @"unit\types\github_647.mzn"

            [<Fact>]
            let ``test nonbool_constraint`` () =
                test @"unit\types\nonbool_constraint.mzn"

            [<Fact>]
            let ``test nonbool_constraint_let`` () =
                test @"unit\types\nonbool_constraint_let.mzn"

            [<Fact>]
            let ``test non_contig_enum`` () =
                test @"unit\types\non_contig_enum.mzn"

            [<Fact>]
            let ``test polymorphic_overloading`` () =
                test @"unit\types\polymorphic_overloading.mzn"

            [<Fact>]
            let ``test record_access_error`` () =
                test @"unit\types\record_access_error.mzn"

            [<Fact>]
            let ``test record_access_success`` () =
                test @"unit\types\record_access_success.mzn"

            [<Fact>]
            let ``test record_array_access_error`` () =
                test @"unit\types\record_array_access_error.mzn"

            [<Fact>]
            let ``test record_binop_par`` () =
                test @"unit\types\record_binop_par.mzn"

            [<Fact>]
            let ``test record_binop_var`` () =
                test @"unit\types\record_binop_var.mzn"

            [<Fact>]
            let ``test record_comprehensions`` () =
                test @"unit\types\record_comprehensions.mzn"

            [<Fact>]
            let ``test record_decl_error`` () =
                test @"unit\types\record_decl_error.mzn"

            [<Fact>]
            let ``test record_ite_error`` () =
                test @"unit\types\record_ite_error.mzn"

            [<Fact>]
            let ``test record_lit_dup`` () =
                test @"unit\types\record_lit_dup.mzn"

            [<Fact>]
            let ``test record_nested`` () =
                test @"unit\types\record_nested.mzn"

            [<Fact>]
            let ``test record_output`` () =
                test @"unit\types\record_output.mzn"

            [<Fact>]
            let ``test record_subtyping`` () =
                test @"unit\types\record_subtyping.mzn"

            [<Fact>]
            let ``test record_var_element`` () =
                test @"unit\types\record_var_element.mzn"

            [<Fact>]
            let ``test record_var_ite`` () =
                test @"unit\types\record_var_ite.mzn"

            [<Fact>]
            let ``test struct_bind_1`` () =
                test @"unit\types\struct_bind_1.mzn"

            [<Fact>]
            let ``test struct_bind_2`` () =
                test @"unit\types\struct_bind_2.mzn"

            [<Fact>]
            let ``test struct_domain_1`` () =
                test @"unit\types\struct_domain_1.mzn"

            [<Fact>]
            let ``test struct_domain_2`` () =
                test @"unit\types\struct_domain_2.mzn"

            [<Fact>]
            let ``test struct_domain_3`` () =
                test @"unit\types\struct_domain_3.mzn"

            [<Fact>]
            let ``test struct_domain_4`` () =
                test @"unit\types\struct_domain_4.mzn"

            [<Fact>]
            let ``test struct_domain_5`` () =
                test @"unit\types\struct_domain_5.mzn"

            [<Fact>]
            let ``test struct_domain_6`` () =
                test @"unit\types\struct_domain_6.mzn"

            [<Fact>]
            let ``test struct_index_sets_1`` () =
                test @"unit\types\struct_index_sets_1.mzn"

            [<Fact>]
            let ``test struct_index_sets_2`` () =
                test @"unit\types\struct_index_sets_2.mzn"

            [<Fact>]
            let ``test struct_return_ti_1`` () =
                test @"unit\types\struct_return_ti_1.mzn"

            [<Fact>]
            let ``test struct_return_ti_2`` () =
                test @"unit\types\struct_return_ti_2.mzn"

            [<Fact>]
            let ``test struct_return_ti_3`` () =
                test @"unit\types\struct_return_ti_3.mzn"

            [<Fact>]
            let ``test struct_return_ti_4`` () =
                test @"unit\types\struct_return_ti_4.mzn"

            [<Fact>]
            let ``test struct_specialise`` () =
                test @"unit\types\struct_specialise.mzn"

            [<Fact>]
            let ``test struct_specialise_return`` () =
                test @"unit\types\struct_specialise_return.mzn"

            [<Fact>]
            let ``test test_any_enum_typeinstid`` () =
                test @"unit\types\test_any_enum_typeinstid.mzn"

            [<Fact>]
            let ``test tuple_access_error1`` () =
                test @"unit\types\tuple_access_error1.mzn"

            [<Fact>]
            let ``test tuple_access_error2`` () =
                test @"unit\types\tuple_access_error2.mzn"

            [<Fact>]
            let ``test tuple_access_success`` () =
                test @"unit\types\tuple_access_success.mzn"

            [<Fact>]
            let ``test tuple_array_access_error`` () =
                test @"unit\types\tuple_array_access_error.mzn"

            [<Fact>]
            let ``test tuple_binop_par`` () =
                test @"unit\types\tuple_binop_par.mzn"

            [<Fact>]
            let ``test tuple_binop_var`` () =
                test @"unit\types\tuple_binop_var.mzn"

            [<Fact>]
            let ``test tuple_comprehensions`` () =
                test @"unit\types\tuple_comprehensions.mzn"

            [<Fact>]
            let ``test tuple_int_set_of_int_specialisation`` () =
                test @"unit\types\tuple_int_set_of_int_specialisation.mzn"

            [<Fact>]
            let ``test tuple_ite_error`` () =
                test @"unit\types\tuple_ite_error.mzn"

            [<Fact>]
            let ``test tuple_lit`` () =
                test @"unit\types\tuple_lit.mzn"

            [<Fact>]
            let ``test tuple_mkpar`` () =
                test @"unit\types\tuple_mkpar.mzn"

            [<Fact>]
            let ``test tuple_output`` () =
                test @"unit\types\tuple_output.mzn"

            [<Fact>]
            let ``test tuple_subtyping`` () =
                test @"unit\types\tuple_subtyping.mzn"

            [<Fact>]
            let ``test tuple_var_element`` () =
                test @"unit\types\tuple_var_element.mzn"

            [<Fact>]
            let ``test tuple_var_ite`` () =
                test @"unit\types\tuple_var_ite.mzn"

            [<Fact>]
            let ``test var_ann_a`` () =
                test @"unit\types\var_ann_a.mzn"

            [<Fact>]
            let ``test var_ann_b`` () =
                test @"unit\types\var_ann_b.mzn"

            [<Fact>]
            let ``test var_ann_comprehension`` () =
                test @"unit\types\var_ann_comprehension.mzn"

            [<Fact>]
            let ``test var_set_bool`` () =
                test @"unit\types\var_set_bool.mzn"

            [<Fact>]
            let ``test var_set_float`` () =
                test @"unit\types\var_set_float.mzn"

            [<Fact>]
            let ``test var_set_float_comprehension`` () =
                test @"unit\types\var_set_float_comprehension.mzn"

            [<Fact>]
            let ``test var_string_a`` () =
                test @"unit\types\var_string_a.mzn"

            [<Fact>]
            let ``test var_string_b`` () =
                test @"unit\types\var_string_b.mzn"

            [<Fact>]
            let ``test var_string_comprehension`` () =
                test @"unit\types\var_string_comprehension.mzn"

        module ``Alldifferent`` =

            [<Fact>]
            let ``test globals_alldiff_set_nosets`` () =
                test @"unit\globals\alldifferent\globals_alldiff_set_nosets.mzn"

            [<Fact>]
            let ``test globals_all_different_int`` () =
                test @"unit\globals\alldifferent\globals_all_different_int.mzn"

            [<Fact>]
            let ``test globals_all_different_int_opt`` () =
                test @"unit\globals\alldifferent\globals_all_different_int_opt.mzn"

            [<Fact>]
            let ``test globals_all_different_set`` () =
                test @"unit\globals\alldifferent\globals_all_different_set.mzn"

        module ``Alldifferent Except 0`` =

            [<Fact>]
            let ``test test_alldiff_except0`` () =
                test @"unit\globals\alldifferent_except_0\test_alldiff_except0.mzn"

            [<Fact>]
            let ``test test_alldiff_except0b`` () =
                test @"unit\globals\alldifferent_except_0\test_alldiff_except0b.mzn"

        module ``All Disjoint`` =

            [<Fact>]
            let ``test globals_all_disjoint`` () =
                test @"unit\globals\all_disjoint\globals_all_disjoint.mzn"

        module ``All Equal`` =

            [<Fact>]
            let ``test globals_all_equal_int`` () =
                test @"unit\globals\all_equal\globals_all_equal_int.mzn"

            [<Fact>]
            let ``test globals_all_equal_set`` () =
                test @"unit\globals\all_equal\globals_all_equal_set.mzn"

        module ``Among`` =

            [<Fact>]
            let ``test globals_among`` () =
                test @"unit\globals\among\globals_among.mzn"

        module ``Arg Max`` =

            [<Fact>]
            let ``test globals_arg_max`` () =
                test @"unit\globals\arg_max\globals_arg_max.mzn"

            [<Fact>]
            let ``test globals_arg_max_opt`` () =
                test @"unit\globals\arg_max\globals_arg_max_opt.mzn"

            [<Fact>]
            let ``test globals_arg_max_opt_weak`` () =
                test @"unit\globals\arg_max\globals_arg_max_opt_weak.mzn"

        module ``Arg Min`` =

            [<Fact>]
            let ``test globals_arg_min`` () =
                test @"unit\globals\arg_min\globals_arg_min.mzn"

            [<Fact>]
            let ``test globals_arg_min_opt_weak`` () =
                test @"unit\globals\arg_min\globals_arg_min_opt_weak.mzn"

        module ``Arg Val`` =

            [<Fact>]
            let ``test arg_val_enum`` () =
                test @"unit\globals\arg_val\arg_val_enum.mzn"

        module ``Atleast`` =

            [<Fact>]
            let ``test globals_at_least`` () =
                test @"unit\globals\atleast\globals_at_least.mzn"

        module ``Atmost 1`` =

            [<Fact>]
            let ``test globals_at_most1`` () =
                test @"unit\globals\atmost1\globals_at_most1.mzn"

        module ``Bin Packing`` =

            [<Fact>]
            let ``test globals_bin_packing`` () =
                test @"unit\globals\bin_packing\globals_bin_packing.mzn"

        module ``Bin Packing Capa`` =

            [<Fact>]
            let ``test globals_bin_packing_capa`` () =
                test @"unit\globals\bin_packing_capa\globals_bin_packing_capa.mzn"

        module ``Circuit`` =

            [<Fact>]
            let ``test test_circuit`` () =
                test @"unit\globals\circuit\test_circuit.mzn"

        module ``Count`` =

            [<Fact>]
            let ``test globals_count`` () =
                test @"unit\globals\count\globals_count.mzn"

        module ``Cumulative`` =

            [<Fact>]
            let ``test github_589`` () =
                test @"unit\globals\cumulative\github_589.mzn"

            [<Fact>]
            let ``test globals_cumulative`` () =
                test @"unit\globals\cumulative\globals_cumulative.mzn"

            [<Fact>]
            let ``test unsat_resource_cumulative`` () =
                test @"unit\globals\cumulative\unsat_resource_cumulative.mzn"

        module ``Decreasing`` =

            [<Fact>]
            let ``test globals_decreasing`` () =
                test @"unit\globals\decreasing\globals_decreasing.mzn"

        module ``Disjoint`` =

            [<Fact>]
            let ``test globals_disjoint`` () =
                test @"unit\globals\disjoint\globals_disjoint.mzn"

        module ``Distribute`` =

            [<Fact>]
            let ``test globals_distribute`` () =
                test @"unit\globals\distribute\globals_distribute.mzn"

        module ``Global Cardinality`` =

            [<Fact>]
            let ``test globals_global_cardinality`` () =
                test @"unit\globals\global_cardinality\globals_global_cardinality.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_low_up`` () =
                test @"unit\globals\global_cardinality\globals_global_cardinality_low_up.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_low_up_opt`` () =
                test @"unit\globals\global_cardinality\globals_global_cardinality_low_up_opt.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_low_up_set`` () =
                test @"unit\globals\global_cardinality\globals_global_cardinality_low_up_set.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_opt`` () =
                test @"unit\globals\global_cardinality\globals_global_cardinality_opt.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_set`` () =
                test @"unit\globals\global_cardinality\globals_global_cardinality_set.mzn"

        module ``Global Cardinality Closed`` =

            [<Fact>]
            let ``test globals_global_cardinality_closed`` () =
                test @"unit\globals\global_cardinality_closed\globals_global_cardinality_closed.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_closed_opt`` () =
                test @"unit\globals\global_cardinality_closed\globals_global_cardinality_closed_opt.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_closed_set`` () =
                test @"unit\globals\global_cardinality_closed\globals_global_cardinality_closed_set.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_low_up_closed`` () =
                test @"unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_low_up_closed_opt`` () =
                test @"unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed_opt.mzn"

            [<Fact>]
            let ``test globals_global_cardinality_low_up_closed_set`` () =
                test @"unit\globals\global_cardinality_closed\globals_global_cardinality_low_up_closed_set.mzn"

        module ``Increasing`` =

            [<Fact>]
            let ``test globals_increasing`` () =
                test @"unit\globals\increasing\globals_increasing.mzn"

            [<Fact>]
            let ``test globals_strictly_increasing_opt`` () =
                test @"unit\globals\increasing\globals_strictly_increasing_opt.mzn"

        module ``Int Set Channel`` =

            [<Fact>]
            let ``test globals_int_set_channel`` () =
                test @"unit\globals\int_set_channel\globals_int_set_channel.mzn"

            [<Fact>]
            let ``test test_int_set_channel2`` () =
                test @"unit\globals\int_set_channel\test_int_set_channel2.mzn"

        module ``Inverse`` =

            [<Fact>]
            let ``test globals_inverse`` () =
                test @"unit\globals\inverse\globals_inverse.mzn"

            [<Fact>]
            let ``test inverse_opt`` () =
                test @"unit\globals\inverse\inverse_opt.mzn"

        module ``Inverse in Range`` =

            [<Fact>]
            let ``test globals_inverse_in_range`` () =
                test @"unit\globals\inverse_in_range\globals_inverse_in_range.mzn"

        module ``Inverse Set`` =

            [<Fact>]
            let ``test globals_inverse_set`` () =
                test @"unit\globals\inverse_set\globals_inverse_set.mzn"

        module ``Lex 2`` =

            [<Fact>]
            let ``test globals_lex2`` () =
                test @"unit\globals\lex2\globals_lex2.mzn"

        module ``Lex Chain`` =

            [<Fact>]
            let ``test globals_lex_chain`` () =
                test @"unit\globals\lex_chain\globals_lex_chain.mzn"

            [<Fact>]
            let ``test globals_lex_chain__orbitope`` () =
                test @"unit\globals\lex_chain\globals_lex_chain__orbitope.mzn"

        module ``Lex Greater`` =

            [<Fact>]
            let ``test globals_lex_greater`` () =
                test @"unit\globals\lex_greater\globals_lex_greater.mzn"

        module ``Lex Greatereq`` =

            [<Fact>]
            let ``test globals_lex_greatereq`` () =
                test @"unit\globals\lex_greatereq\globals_lex_greatereq.mzn"

        module ``Lex Less`` =

            [<Fact>]
            let ``test globals_lex_less`` () =
                test @"unit\globals\lex_less\globals_lex_less.mzn"

            [<Fact>]
            let ``test test_bool_lex_less`` () =
                test @"unit\globals\lex_less\test_bool_lex_less.mzn"

        module ``Lex Lesseq`` =

            [<Fact>]
            let ``test globals_lex_lesseq`` () =
                test @"unit\globals\lex_lesseq\globals_lex_lesseq.mzn"

            [<Fact>]
            let ``test test_bool_lex_lesseq`` () =
                test @"unit\globals\lex_lesseq\test_bool_lex_lesseq.mzn"

        module ``Link Set to Booleans`` =

            [<Fact>]
            let ``test globals_link_set_to_booleans`` () =
                test @"unit\globals\link_set_to_booleans\globals_link_set_to_booleans.mzn"

        module ``Maximum`` =

            [<Fact>]
            let ``test globals_maximum_int`` () =
                test @"unit\globals\maximum\globals_maximum_int.mzn"

        module ``Minimum`` =

            [<Fact>]
            let ``test globals_minimum_int`` () =
                test @"unit\globals\minimum\globals_minimum_int.mzn"

        module ``Nvalue`` =

            [<Fact>]
            let ``test globals_nvalue`` () =
                test @"unit\globals\nvalue\globals_nvalue.mzn"

            [<Fact>]
            let ``test nvalue_total`` () =
                test @"unit\globals\nvalue\nvalue_total.mzn"

        module ``Partition Set`` =

            [<Fact>]
            let ``test globals_partition_set`` () =
                test @"unit\globals\partition_set\globals_partition_set.mzn"

        module ``Range`` =

            [<Fact>]
            let ``test globals_range`` () =
                test @"unit\globals\range\globals_range.mzn"

        module ``Regular`` =

            [<Fact>]
            let ``test globals_regular`` () =
                test @"unit\globals\regular\globals_regular.mzn"

            [<Fact>]
            let ``test globals_regular_regex_1`` () =
                test @"unit\globals\regular\globals_regular_regex_1.mzn"

            [<Fact>]
            let ``test globals_regular_regex_2`` () =
                test @"unit\globals\regular\globals_regular_regex_2.mzn"

            [<Fact>]
            let ``test globals_regular_regex_3`` () =
                test @"unit\globals\regular\globals_regular_regex_3.mzn"

            [<Fact>]
            let ``test globals_regular_regex_4`` () =
                test @"unit\globals\regular\globals_regular_regex_4.mzn"

            [<Fact>]
            let ``test globals_regular_regex_5`` () =
                test @"unit\globals\regular\globals_regular_regex_5.mzn"

            [<Fact>]
            let ``test globals_regular_regex_6`` () =
                test @"unit\globals\regular\globals_regular_regex_6.mzn"

        module ``Roots`` =

            [<Fact>]
            let ``test roots_bad`` () =
                test @"unit\globals\roots\roots_bad.mzn"

            [<Fact>]
            let ``test test_roots`` () =
                test @"unit\globals\roots\test_roots.mzn"

            [<Fact>]
            let ``test test_roots2`` () =
                test @"unit\globals\roots\test_roots2.mzn"

            [<Fact>]
            let ``test test_roots3`` () =
                test @"unit\globals\roots\test_roots3.mzn"

        module ``Sliding Sum`` =

            [<Fact>]
            let ``test globals_sliding_sum`` () =
                test @"unit\globals\sliding_sum\globals_sliding_sum.mzn"

        module ``Sort`` =

            [<Fact>]
            let ``test globals_sort`` () =
                test @"unit\globals\sort\globals_sort.mzn"

        module ``Strict Lex2`` =

            [<Fact>]
            let ``test globals_strict_lex2`` () =
                test @"unit\globals\strict_lex2\globals_strict_lex2.mzn"

        module ``Subcircuit`` =

            [<Fact>]
            let ``test test_subcircuit`` () =
                test @"unit\globals\subcircuit\test_subcircuit.mzn"

        module ``Sum Pred`` =

            [<Fact>]
            let ``test globals_sum_pred`` () =
                test @"unit\globals\sum_pred\globals_sum_pred.mzn"

        module ``Table`` =

            [<Fact>]
            let ``test globals_table`` () =
                test @"unit\globals\table\globals_table.mzn"

            [<Fact>]
            let ``test globals_table_opt`` () =
                test @"unit\globals\table\globals_table_opt.mzn"

        module ``Value Precede`` =

            [<Fact>]
            let ``test globals_value_precede_int`` () =
                test @"unit\globals\value_precede\globals_value_precede_int.mzn"

            [<Fact>]
            let ``test globals_value_precede_int_opt`` () =
                test @"unit\globals\value_precede\globals_value_precede_int_opt.mzn"

            [<Fact>]
            let ``test globals_value_precede_set`` () =
                test @"unit\globals\value_precede\globals_value_precede_set.mzn"

        module ``Value Precede Chain`` =

            [<Fact>]
            let ``test globals_value_precede_chain_int`` () =
                test @"unit\globals\value_precede_chain\globals_value_precede_chain_int.mzn"

            [<Fact>]
            let ``test globals_value_precede_chain_int_opt`` () =
                test @"unit\globals\value_precede_chain\globals_value_precede_chain_int_opt.mzn"

            [<Fact>]
            let ``test globals_value_precede_chain_set`` () =
                test @"unit\globals\value_precede_chain\globals_value_precede_chain_set.mzn"

        module ``Var Sqr Sym`` =

            [<Fact>]
            let ``test globals_var_sqr_sym`` () =
                test @"unit\globals\var_sqr_sym\globals_var_sqr_sym.mzn"

        module ``Examples`` =

            [<Fact>]
            let ``test 2DPacking`` () =
                test @"examples\2DPacking.mzn"

            [<Fact>]
            let ``test alpha`` () =
                test @"examples\alpha.mzn"

            [<Fact>]
            let ``test battleships10`` () =
                test @"examples\battleships10.mzn"

            [<Fact>]
            let ``test battleships_1`` () =
                test @"examples\battleships_1.mzn"

            [<Fact>]
            let ``test battleships_2`` () =
                test @"examples\battleships_2.mzn"

            [<Fact>]
            let ``test battleships_3`` () =
                test @"examples\battleships_3.mzn"

            [<Fact>]
            let ``test battleships_4`` () =
                test @"examples\battleships_4.mzn"

            [<Fact>]
            let ``test battleships_5`` () =
                test @"examples\battleships_5.mzn"

            [<Fact>]
            let ``test battleships_7`` () =
                test @"examples\battleships_7.mzn"

            [<Fact>]
            let ``test battleships_9`` () =
                test @"examples\battleships_9.mzn"

            [<Fact>]
            let ``test blocksworld_instance_1`` () =
                test @"examples\blocksworld_instance_1.mzn"

            [<Fact>]
            let ``test blocksworld_instance_2`` () =
                test @"examples\blocksworld_instance_2.mzn"

            [<Fact>]
            let ``test cutstock`` () =
                test @"examples\cutstock.mzn"

            [<Fact>]
            let ``test eq20`` () =
                test @"examples\eq20.mzn"

            [<Fact>]
            let ``test factory_planning_instance`` () =
                test @"examples\factory_planning_instance.mzn"

            [<Fact>]
            let ``test golomb`` () =
                test @"examples\golomb.mzn"

            [<Fact>]
            let ``test halfreif`` () =
                test @"examples\halfreif.mzn"

            [<Fact>]
            let ``test jobshop2x2`` () =
                test @"examples\jobshop2x2.mzn"

            [<Fact>]
            let ``test knights`` () =
                test @"examples\knights.mzn"

            [<Fact>]
            let ``test langford`` () =
                test @"examples\langford.mzn"

            [<Fact>]
            let ``test langford2`` () =
                test @"examples\langford2.mzn"

            [<Fact>]
            let ``test latin_squares_fd`` () =
                test @"examples\latin_squares_fd.mzn"

            [<Fact>]
            let ``test magicsq_3`` () =
                test @"examples\magicsq_3.mzn"

            [<Fact>]
            let ``test magicsq_4`` () =
                test @"examples\magicsq_4.mzn"

            [<Fact>]
            let ``test magicsq_5`` () =
                test @"examples\magicsq_5.mzn"

            [<Fact>]
            let ``test multidimknapsack_simple`` () =
                test @"examples\multidimknapsack_simple.mzn"

            [<Fact>]
            let ``test oss`` () =
                test @"examples\oss.mzn"

            [<Fact>]
            let ``test packing`` () =
                test @"examples\packing.mzn"

            [<Fact>]
            let ``test perfsq`` () =
                test @"examples\perfsq.mzn"

            [<Fact>]
            let ``test perfsq2`` () =
                test @"examples\perfsq2.mzn"

            [<Fact>]
            let ``test photo`` () =
                test @"examples\photo.mzn"

            [<Fact>]
            let ``test product_fd`` () =
                test @"examples\product_fd.mzn"

            [<Fact>]
            let ``test product_lp`` () =
                test @"examples\product_lp.mzn"

            [<Fact>]
            let ``test quasigroup_qg5`` () =
                test @"examples\quasigroup_qg5.mzn"

            [<Fact>]
            let ``test queen_cp2`` () =
                test @"examples\queen_cp2.mzn"

            [<Fact>]
            let ``test queen_ip`` () =
                test @"examples\queen_ip.mzn"

            [<Fact>]
            let ``test radiation`` () =
                test @"examples\radiation.mzn"

            [<Fact>]
            let ``test simple_sat`` () =
                test @"examples\simple_sat.mzn"

            [<Fact>]
            let ``test singHoist2`` () =
                test @"examples\singHoist2.mzn"

            [<Fact>]
            let ``test steiner-triples`` () =
                test @"examples\steiner-triples.mzn"

            [<Fact>]
            let ``test sudoku`` () =
                test @"examples\sudoku.mzn"

            [<Fact>]
            let ``test template_design`` () =
                test @"examples\template_design.mzn"

            [<Fact>]
            let ``test tenpenki_1`` () =
                test @"examples\tenpenki_1.mzn"

            [<Fact>]
            let ``test tenpenki_2`` () =
                test @"examples\tenpenki_2.mzn"

            [<Fact>]
            let ``test tenpenki_3`` () =
                test @"examples\tenpenki_3.mzn"

            [<Fact>]
            let ``test tenpenki_4`` () =
                test @"examples\tenpenki_4.mzn"

            [<Fact>]
            let ``test tenpenki_5`` () =
                test @"examples\tenpenki_5.mzn"

            [<Fact>]
            let ``test tenpenki_6`` () =
                test @"examples\tenpenki_6.mzn"

            [<Fact>]
            let ``test timetabling`` () =
                test @"examples\timetabling.mzn"

            [<Fact>]
            let ``test trucking`` () =
                test @"examples\trucking.mzn"

            [<Fact>]
            let ``test warehouses`` () =
                test @"examples\warehouses.mzn"

            [<Fact>]
            let ``test wolf_goat_cabbage`` () =
                test @"examples\wolf_goat_cabbage.mzn"

            [<Fact>]
            let ``test zebra`` () =
                test @"examples\zebra.mzn"

