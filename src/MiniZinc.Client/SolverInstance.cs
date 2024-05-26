namespace MiniZinc.Client;

using Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Parser;
using Parser.Syntax;
using Process;

/// <summary>
/// MiniZinc + Solver + Model
/// </summary>
public class SolverInstance
{
    public readonly MiniZincClient Minizinc;
    public readonly SolverInfo Solver;
    public readonly SyntaxTree Model;
    public readonly Process Process;
    public readonly string ModelPath;
    public readonly string ModelText;
    private readonly ILogger _logger;

    internal SolverInstance(
        MiniZincClient minizinc,
        SolverInfo solver,
        SyntaxTree model,
        ILogger? logger = null
    )
    {
        _logger = logger ?? NullLogger.Instance;
        Minizinc = minizinc;
        Solver = solver;
        Model = model;
        ModelText = model.Write();
        ModelPath = Path.ChangeExtension(Path.GetTempFileName(), ".mzn");
        File.WriteAllText(ModelPath, ModelText);
        _logger.LogInformation(
            "Model with {Nodes} nodes saved to {Path}",
            model.Nodes.Count,
            ModelPath
        );

        Process = minizinc.CreateProcess(
            $"--solver {solver.Id}",
            "--json-stream",
            "--output-objective",
            ModelPath
        );
    }

    /// <summary>
    /// Wait for the
    /// </summary>
    /// <param name="token"></param>
    public async Task<Solution> Wait(CancellationToken token = default)
    {
        var solveStart = DateTimeOffset.Now;
        var iterStart = solveStart;
        var iteration = 0;
        var variables = new Dictionary<string, object>();
        var warnings = new List<string>();
        var status = SolveStatus.Pending;
        int objective = 0;

        Solution? solution = null;

        await foreach (var msg in Process.Watch(token))
        {
            MiniZincMessage? message = null;
            switch (msg.EventType, msg.Content)
            {
                case (ProcessEventType.Started, _):
                    status = SolveStatus.Started;
                    break;
                case (ProcessEventType.StdErr, var data):
                    message = MiniZincMessage.Deserialize(data!);
                    break;
                case (ProcessEventType.StdOut, var data):
                    message = MiniZincMessage.Deserialize(data!);
                    break;
                case (ProcessEventType.Exited, _):
                    break;
            }

            if (message is null)
                continue;

            switch (message)
            {
                case StatusMessage m:
                    _logger.LogInformation("Status {Status}", m.Status);

                    switch (m.Status)
                    {
                        case "ALL_SOLUTIONS":
                            status = SolveStatus.AllSolutions;
                            break;
                        case "OPTIMAL_SOLUTION":
                            status = SolveStatus.Optimal;
                            break;
                        case "UNSATISFIABLE":
                            status = SolveStatus.Unsatisfiable;
                            break;
                        case "UNBOUNDED":
                            status = SolveStatus.Unbounded;
                            break;
                        case "UNSAT_OR_UNBOUNDED":
                            status = SolveStatus.UnsatOrUnbounded;
                            break;
                        case "UNKNOWN":
                            status = SolveStatus.Error;
                            break;
                        case "ERROR":
                            status = SolveStatus.Error;
                            break;
                    }
                    break;

                // TODO - specialised dzn parser
                case SolutionMessage m:
                    iteration++;
                    status = SolveStatus.Satisfied;
                    var dzn = m.Output["dzn"].ToString();
                    var parsed = Parser.ParseText(dzn!);
                    var data = parsed.Syntax;
                    foreach (var node in data.Nodes)
                    {
                        if (node is not AssignmentSyntax var)
                            throw new Exception();
                        variables[var.Name.ToString()] = var.Expr;
                    }

                    if (variables.TryGetValue("_objective", out var obj))
                    {
                        objective = (IntLiteralSyntax)obj;
                    }
                    break;
                case CommentMessage m:
                    _logger.LogInformation("{Comment}", m.Comment);
                    break;
                case WarningMessage m:
                    _logger.LogWarning("{Kind} - {Message}", m.Kind, m.Message);
                    break;
                case ErrorMessage m:
                    _logger.LogError("{Kind} - {Message}", m.Kind, m.Message);
                    break;
                case StatisticsMessage m:
                    break;
                case TraceMessage m:
                    break;
            }

            var time = DateTimeOffset.Now;
            var solveTime = solveStart.ToPeriod(time);
            var iterTime = iterStart.ToPeriod(time);
            solution = new Solution
            {
                Command = Process.Command.String,
                ProcessId = Process.ProcessId,
                TotalTime = solveTime,
                IterationTime = iterTime,
                Iteration = iteration,
                Status = status,
                Objective = objective,
                Bound = null,
                AbsoluteGap = null,
                RelativeGap = null,
                AbsoluteDelta = null,
                RelativeDelta = null,
                Variables = variables,
                Outputs = null,
                Statistics = null,
                Warnings = null
            };
            iterStart = time;
        }

        return solution!;
    }

    public void Dispose()
    {
        Process.Dispose();
    }

    public override string ToString() => Solver.Name;
};
