using System.Text.RegularExpressions;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day03;

/// <summary>
/// Solution code for Day 3: Mull It Over
/// </summary>
partial class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 3: Mull It Over"; }
    }

    [GeneratedRegex(@"mul\((\d+),(\d+)\)", RegexOptions.Multiline)]
    private static partial Regex MultiplyRegex();

    [GeneratedRegex(@"do\(\)|don't\(\)|mul\((\d+),(\d+)\)", RegexOptions.Multiline)]
    private static partial Regex DoDontMultiplyRegex();

    public object PartOne(string input)
    {
        int total = 0;
        foreach (var match in MultiplyRegex().Matches(input).AsEnumerable())
        {
            var groups = match.Groups;
            var left = int.Parse(groups[1].Value);
            var right = int.Parse(groups[2].Value);
            total += left * right;
        }

        return total;
    }

    public object PartTwo(string input)
    {
        int total = 0;
        bool enabled = true;
        foreach (var match in DoDontMultiplyRegex().Matches(input).AsEnumerable())
        {
            if (match.Value == "do()")
            {
                enabled = true;
            }
            else if (match.Value == "don't()")
            {
                enabled = false;
            }
            else if (enabled)
            {
                var groups = match.Groups;
                var left = int.Parse(groups[1].Value);
                var right = int.Parse(groups[2].Value);
                total += left * right;
            }
        }

        return total;
    }
}
