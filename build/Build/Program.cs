using System.CommandLine;
using Build;

var root = new RootCommand("MiniZinc.NET build options");

Add(
    "--clone-libminizinc-tests",
    "Clone the test suite from libminizinc",
    CloneLibMiniZincTests.Run
);

Add(
    "--parse-libminizinc-tests",
    "Generate test cases from the libminizinc test spec",
    ParseLibMiniZincTests.Run
);

Add("--generate-lexer-tests", "Generate lexer tests", MakeLexerTests.Run);

Add("--generate-parser-tests", "Generate parser tests", MakeParserTests.Run);

Add("--generate-solver-tests", "Generate solver tests", MakeSolverTests.Run);

var result = await root.InvokeAsync(args);
return result;

void Add(string name, string desc, Func<Task> handler)
{
    var command = new Command(name: name, description: desc);
    command.SetHandler(handler);
    root.AddCommand(command);
}
