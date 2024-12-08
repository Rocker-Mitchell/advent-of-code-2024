using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day08;

/// <summary>
/// Solution code for Day 8: Resonant Collinearity
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 8: Resonant Collinearity"; }
    }

    private Size bounds;
    private readonly Dictionary<char, HashSet<Point>> antennas = [];

    private void ParseMap(string input)
    {
        var lines = input.Split('\n');
        bounds = new(lines[0].Length, lines.Length);

        antennas.Clear();

        for (int row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (int col = 0; col < line.Length; col++)
            {
                char pos = line[col];

                if (char.IsLetterOrDigit(pos))
                {
                    Point newPoint = new(col, row);

                    if (antennas.TryGetValue(pos, out var points))
                    {
                        points.Add(newPoint);
                    }
                    else
                    {
                        points = [newPoint];
                        antennas.Add(pos, points);
                    }
                }
            }
        }
    }

    private bool IsInBounds(Point point)
    {
        return point.X >= 0 && point.X < bounds.Width
            && point.Y >= 0 && point.Y < bounds.Height;
    }

    private HashSet<Point> GetAntinodesFromPair(
        Point a,
        Point b,
        bool partTwo = false
    )
    {
        HashSet<Point> output = [];

        if (partTwo)
        {
            output.Add(a);
            output.Add(b);
        }

        var diff = (Size)(a - (Size)b);

        for (Point da = a + diff; IsInBounds(da); da += diff)
        {
            output.Add(da);

            // only run once for part one
            if (!partTwo)
                break;
        }

        for (Point db = b - diff; IsInBounds(db); db -= diff)
        {
            output.Add(db);

            // only run once for part one
            if (!partTwo)
                break;
        }

        return output;
    }

    private HashSet<Point> GetAntinodes(
        HashSet<Point> antennaPoints,
        bool partTwo = false
    )
    {
        HashSet<Point> output = [];

        Point[] sequence = [.. antennaPoints];
        for (int idx = 0; idx + 1 < sequence.Length; idx++)
        {
            Point a = sequence[idx];

            for (int jdx = idx + 1; jdx < sequence.Length; jdx++)
            {
                Point b = sequence[jdx];

                output.UnionWith(GetAntinodesFromPair(a, b, partTwo));
            }
        }

        return output;
    }

    public object PartOne(string input)
    {
        ParseMap(input);

        HashSet<Point> antinodes = [];
        foreach (var points in antennas.Values)
        {
            antinodes.UnionWith(GetAntinodes(points));
        }

        return antinodes.Count;
    }

    public object PartTwo(string input)
    {
        ParseMap(input);

        HashSet<Point> antinodes = [];
        foreach (var points in antennas.Values)
        {
            antinodes.UnionWith(GetAntinodes(points, true));
        }

        return antinodes.Count;
    }
}
