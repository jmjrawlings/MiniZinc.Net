namespace MiniZinc.Tests;

using Client;

public class ClientFixture : IDisposable
{
    public readonly MiniZincClient MiniZinc;

    public ClientFixture()
    {
        MiniZinc = MiniZincClient.Create();
    }

    public void Dispose() { }
}
