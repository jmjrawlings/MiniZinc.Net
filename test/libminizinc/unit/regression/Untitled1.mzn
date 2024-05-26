array[1..40] of var int: pl;
var 9..15: K;
array[1..40] of var 0..1: B1;
array[1..40] of var 0..1: B2;
int: BigM = 10000;
constraint
    forall(F in 1..40)(F + 42 * (1 - B1[F]) >= K - 6);
constraint
    forall(F in 1..40)(F + 45 * (1 - B2[F]) >= K - 3);
constraint
    forall(F in 1..33)(BigM * B1[F] + sum(M in F..F + 7)(pl[M]) >= 1);
constraint
    forall(F in 1..36)(sum(M in F..F + 4)(pl[M]) < - 1 + BigM * B2[F]);
solve maximize 
    K;