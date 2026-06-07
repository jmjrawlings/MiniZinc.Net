using Xunit;

// Integration tests shell out to the MiniZinc executable and write a shared temp
// .mzn file, so they must not run concurrently.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
