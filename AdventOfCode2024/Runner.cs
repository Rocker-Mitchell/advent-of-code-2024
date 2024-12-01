namespace AdventOfCode2024;

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
        foreach (var line in solver.Solve(input))
        {
            ColorConsole.WriteLine($"Part {part}", ConsoleColor.DarkGray);
            Console.WriteLine($"{line}");
            part++;
        }
    }
}
