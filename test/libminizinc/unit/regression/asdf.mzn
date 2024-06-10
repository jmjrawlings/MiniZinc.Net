include "experimental/on_restart.mzn";
var 1..3: x;
var float: y;
function var bool: foo=
    status() != START -> y = sol(int2float(x));
solve maximize 
    x;
