using System.Diagnostics;

namespace AdventOfCode2024.Lib;

class Runner
{
    private static string GetNormalizedInput(string file)
    {
        var input = File.ReadAllText(file);

        // handle Windows' "\r\n" format
        input = input.Replace("\r", "");

        // handle removing newline at end
        if (input.EndsWith('\n'))
        {
            input = input[..^1];
        }

        return input;
    }

    public static void RunSolver(ISolver solver)
    {
        ColorConsole.WriteLine(solver.PrintName, ConsoleColor.Yellow);

        var input = GetNormalizedInput(solver.InputFile);

        var part = 1;
        var stopwatch = Stopwatch.StartNew();
        foreach (var line in solver.Solve(input))
        {
            var time = stopwatch.ElapsedMilliseconds;

            ColorConsole.WriteLine($"Part {part}", ConsoleColor.DarkGray);
            Console.Write($"{line} ");
            ColorConsole.WriteLine(
                $"({time} ms)",
                time > 1000 ? ConsoleColor.Red : ConsoleColor.DarkBlue
            );

            part++;
            stopwatch.Restart();
        }
    }
}
