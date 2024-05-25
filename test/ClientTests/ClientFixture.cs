namespace MiniZinc.Tests;

using Client;

public class ClientFixture : IDisposable
{
    const string PATH = "C://Program Files//MiniZinc//minizinc.exe";

    public readonly MiniZincClient Client;

    public ClientFixture()
    {
        Client = MiniZincClient.Create(PATH);
    }

    public void Dispose() { }
}
