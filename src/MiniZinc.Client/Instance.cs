namespace MiniZinc.Client;

using MiniZinc.Client.Messages;
using MiniZinc.Process;

/// <summary>
/// An instance is a model being solved by a solver
/// </summary>
public sealed class Instance
{
    // public readonly Command Command;
    // private readonly Process _process;

    // public Instance(Command command)
    // {
    //     Command = command;
    //     _process = Process.Create(command);
    // }
    //
    // public async MiniZincMessage Solve(CancellationToken token)
    // {
    //     MiniZincMessage? message = null;
    //     await foreach (var msg in _process.Listen(token))
    //     {
    //         switch (msg.EventType, msg.Content)
    //         {
    //             case (ProcessEventType.Started, _):
    //                 break;
    //             case (ProcessEventType.StdErr, var data):
    //                 message = MiniZincMessage.Deserialize(data);
    //                 break;
    //             case (ProcessEventType.StdOut, var data):
    //                 message = MiniZincMessage.Deserialize(data);
    //                 break;
    //             case (ProcessEventType.Exited, _):
    //                 break;
    //         }
    //     }
    // }
    //
    // public void Dispose()
    // {
    //     _process.Dispose();
    // }
}
