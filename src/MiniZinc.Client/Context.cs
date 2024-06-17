namespace MiniZinc.Client;

using System.Runtime.CompilerServices;

public class Context : IAsyncEnumerable<int>, IAsyncEnumerator<int>
{
    public readonly int X;
    public readonly int Count;
    public int I = 0;

    public Context(int x)
    {
        X = x;
        Count = 3;
    }

    public TaskAwaiter<int> GetAwaiter()
    {
        return Task.FromResult(X).GetAwaiter();
    }

    public async ValueTask DisposeAsync() { }

    public ValueTask<bool> MoveNextAsync()
    {
        if (I++ >= Count)
            return ValueTask.FromResult(false);
        else
            return ValueTask.FromResult(true);
    }

    public int Current => X;

    public IAsyncEnumerator<int> GetAsyncEnumerator(
        CancellationToken cancellationToken = new CancellationToken()
    ) => this;
}

public static class Xd
{
    public static async Task Dx()
    {
        var x = await new Context(3);
        await foreach (var z in new Context(100)) { }
    }
}
