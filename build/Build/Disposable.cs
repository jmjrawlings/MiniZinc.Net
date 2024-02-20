namespace Build;

/// <summary>
/// Anonymous diposable
/// </summary>
public readonly struct Disposable : IDisposable
{
    private readonly Action _action;

    public Disposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}
