namespace Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using MiniZinc.Build;
using MiniZinc.Parser;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.Monitoring, launchCount: 1, iterationCount: 1, warmupCount: 1)]
public class ParserBenchmarks
{
    public FileInfo[] TestFiles { get; }

    public ParserBenchmarks()
    {
        TestFiles = Repo.TestSpecDir.JoinDir("examples").GetFiles();
    }

    [ParamsSource(nameof(TestFiles))]
    public FileInfo TestFile;

    public string Mzn;

    [GlobalSetup]
    public void GlobalSetup()
    {
        Mzn = TestFile.OpenText().ReadToEnd();
    }

    [Benchmark]
    public void Benchmark()
    {
        var result = Parser.ParseString(Mzn);
    }
}
