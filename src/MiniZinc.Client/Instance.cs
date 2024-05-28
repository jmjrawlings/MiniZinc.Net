namespace MiniZinc.Client;

using System.Runtime.CompilerServices;
using Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Parser;
using Parser.Syntax;
using Process;

/// <summary>
/// An instance of a model being solved
/// by a solver with minizinc
/// </summary>
public class Instance
{
    public readonly MiniZincClient Minizinc;
    public readonly Solver Solver;
    public readonly SyntaxTree Model;
    public readonly string ModelPath;
    public readonly string ModelText;
    public readonly SolveMethod SolveMethod;
    public Process? Process;
    private readonly ILogger _logger;
    private readonly Args _args;

    internal Instance(
        MiniZincClient minizinc,
        SyntaxTree model,
        SolveOptions options,
        ILogger? logger = null
    )
    {
        _logger = logger ?? NullLogger.Instance;
        _args = new Args();
        _args.Add("--solver", options.SolverId);
        Minizinc = minizinc;
        Solver = minizinc.GetSolver(options.SolverId);
        Model = model;
        ModelText = model.Write(new WriteOptions { SkipOutput = true });
        ModelPath = Path.ChangeExtension(Path.GetTempFileName(), ".mzn");

        SolveMethod = SolveMethod.Satisfy;
        foreach (var node in model.Nodes)
        {
            switch (node)
            {
                case SolveSyntax n:
                    SolveMethod = n.Method;
                    break;
            }
        }

        File.WriteAllText(ModelPath, ModelText);
        _logger.LogInformation(
            "Model with {Nodes} nodes saved to {Path}",
            model.Nodes.Count,
            ModelPath
        );
    }

    /// <summary>
    /// Wait for the
    /// </summary>
    /// <param name="token"></param>
    public async Task<Solution> Solution(CancellationToken token = default)
    {
        Solution? solution = null;
        await foreach (var sol in Start(token))
            continue;

        return solution;
    }

    /// <summary>
    /// Wait for the
    /// </summary>
    /// <param name="token"></param>
    public async IAsyncEnumerable<Solution> Start(
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        _args.Add("--json-stream", "--output-objective", ModelPath);
        Process = Minizinc.CreateProcess(_args);
        var solveStart = DateTimeOffset.Now;
        var iterStart = solveStart;
        var iteration = 0;
        var variables = new Dictionary<string, object>();
        var warnings = new List<string>();
        var status = SolveStatus.Pending;
        int objective = 0;

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
            var solution = new Solution
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
            yield return solution;
        }
    }

    public void Dispose()
    {
        Process?.Dispose();
    }

    public override string ToString() => Solver.Name;
};
