using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day01;

/// <summary>
/// Solution code for Day 1: Historian Hysteria
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 1: Historian Hysteria"; }
    }

    /// <summary>
    /// With a zipped number list input, sort the lists, pair them across, and
    /// sum the absolute difference between pairs.
    /// </summary>
    public object PartOne(string input)
    {
        var (left, right) = ParseLists(input);

        Array.Sort(left);
        Array.Sort(right);

        var total = 0;
        foreach (var (l, v) in left.Zip(right))
        {
            total += Math.Abs(l - v);
        }

        return total;
    }

    /// <summary>
    /// With a zipped number list input, sum the product of items from the
    /// first list multiplied by the frequency of matching items in the second
    /// list.
    /// </summary>
    public object PartTwo(string input)
    {
        var (left, right) = ParseLists(input);

        var rightCounts = right.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());

        var total = 0;
        foreach (var l in left)
        {
            var count = rightCounts.GetValueOrDefault(l, 0);
            total += l * count;
        }

        return total;
    }

    private (int[], int[]) ParseLists(string input)
    {
        var lines = input.Split('\n');
        var left = new List<int>(lines.Length);
        var right = new List<int>(lines.Length);
        foreach (var line in lines)
        {
            var parts = line.Split();
            left.Add(int.Parse(parts[0]));
            right.Add(int.Parse(parts[^1]));
        }

        return ([.. left], [.. right]);
    }
}
