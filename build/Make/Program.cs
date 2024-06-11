﻿using System.CommandLine;
using Make;

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

Add("--make-lexer-tests", "Generate lexer tests", MakeLexerTests.Run);

Add("--make-parser-tests", "Generate parser tests", MakeParserTests.Run);

Add("--make-solver-tests", "Generate solver tests", MakeClientTests.Run);

var result = await root.InvokeAsync(args);
return result;

void Add(string name, string desc, Func<Task> handler)
{
    var command = new Command(name: name, description: desc);
    command.SetHandler(handler);
    root.AddCommand(command);
}