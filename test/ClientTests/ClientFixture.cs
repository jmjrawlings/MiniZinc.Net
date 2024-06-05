namespace MiniZinc.Tests;

using Client;

public class ClientFixture : IDisposable
{
    public readonly MiniZincClient Client;

    public ClientFixture()
    {
        Client = MiniZincClient.Create();
    }

    public void Dispose() { }
}
