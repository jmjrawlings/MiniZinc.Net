namespace MiniZinc.Net.Tests;

using static Prelude;

[CollectionDefinition("libminizinc")]
public sealed class LibMiniZincCollection : IClassFixture<TestSpecFixture> { }

public sealed class TestSpecFixture
{
    public readonly TestSpec TestSpec;

    public TestSpecFixture()
    {
        TestSpec = TestSpec.ParseTestSuitesFromJson(TestSpecJson);
    }
}
