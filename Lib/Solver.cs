namespace AdventOfCode2024.Lib;

interface ISolver
{
    /// <summary>
    /// A descriptive name for the solver.
    /// </summary>
    string PrintName { get { return GetType().ToString(); } }

    /// <summary>
    /// The filepath to the input data expected by the solver.
    /// </summary>
    string InputFile
    {
        get
        {
            // expect the namespace styling to line up with the folder
            //  structure (project name, then following parts matching
            //  folders), and "input.txt" being co-located with the solution
            //  code
            var fullName = GetType().Namespace ?? "";
            var dir = Path.Combine(fullName.Split('.')[1..]);
            return Path.Combine(dir, "input.txt");
        }
    }

    /// <summary>
    /// The method to solve the first part.
    /// </summary>
    /// <param name="input">The input text to solve.</param>
    /// <returns>The solution found.</returns>
    object PartOne(string input);

    /// <summary>
    /// The method to solve the second part.
    /// <br/>
    /// This can be unimplemented while working on the first part.
    /// </summary>
    /// <param name="input">The input text to solve.</param>
    /// <returns>The solution found.</returns>
    object? PartTwo(string input) => null;
}

static class SolverExtensions
{
    /// <summary>
    /// Solve the input, enumerating for the two parts.
    /// </summary>
    /// <param name="solver"></param>
    /// <param name="input">The input text to solve.</param>
    /// <returns>
    /// An enumerable of the solution(s) found for each part.
    /// If the second part is not implemented, it will not be included.
    /// </returns>
    public static IEnumerable<object> Solve(this ISolver solver, string input)
    {
        yield return solver.PartOne(input);

        var result = solver.PartTwo(input);
        if (result != null)
        {
            yield return result;
        }
    }
}