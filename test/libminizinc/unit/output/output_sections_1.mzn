/***
--- !Test
solvers: [gecode]
expected: !Result
  solution: !Solution
    _output_item: 'aebdc'
--- !Test
solvers: [gecode]
options:
  only-sections: foo,bar
expected: !Result
  solution: !Solution
    _output_item: 'bdc'
--- !Test
solvers: [gecode]
options:
  not-sections: foo,bar
expected: !Result
  solution: !Solution
    _output_item: 'ae'
***/

output ["a"];
output :: "foo" ["b"];
output :: "bar" ["c"];
output :: "foo" ["d"];
output ["e"];
