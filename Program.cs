using AdventOfCode2024.Lib;

if (args.Length == 0)
{
    Console.WriteLine($"""
    Usage: dotnet run [arguments]

    Run code that solves Advent of Code 2024 challenges.

    With no arguments, display this usage documentation.

    arguments:
    day         solve the specified problems for this day
    """);
    return;
}

string dayStr = args[0];
int day;
try
{
    day = int.Parse(dayStr);
}
catch (FormatException e)
{
    Console.WriteLine($"Failed to parse day argument to number: {e.Message}");
    Environment.Exit(1);
    return;
}
if (day < 1)
{
    Console.WriteLine($"Day must be greater than 0: {dayStr}");
    Environment.Exit(1);
    return;
}

string fullSolverTypeName = $"AdventOfCode2024.Day{day:00}.Solution";
// TODO more specific exception type/message, or console message then exit?
Type solverType = Type.GetType(fullSolverTypeName) ?? throw new Exception($"Failed to get type: {fullSolverTypeName}");
// TODO more specific exception type/message?
ISolver solver = (ISolver?)Activator.CreateInstance(solverType) ?? throw new Exception($"Failed to create instance: {solverType}");

Runner.RunSolver(solver);
