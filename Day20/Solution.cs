using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day20;

/// <summary>
/// Solution code for Day 20: Race Condition
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 20: Race Condition"; }
    }

    /*
    public string InputFile
    {
        get
        {
            var dir = NamespacePath.GetFolderPathFromType(GetType());
            return Path.Combine(dir, "input.test.txt");
        }
    }
    //*/

    static readonly int minimumSteps = 100;

    private static readonly Size[] neighborDirections = [
        new(1,0),
        new(-1,0),
        new(0,1),
        new(0,-1),
    ];

    static IEnumerable<Point> GetNeighbors(Point point)
    {
        return neighborDirections.Select(nd => point + nd);
    }

    readonly Dictionary<Point, int> trackPathIndexes = [];

    void ParseMaze(string input)
    {
        var maze = input.Split('\n');

        Point? start = null;
        HashSet<Point> trackPoints = [];

        for (int y = 0; y < maze.Length; y++)
        {
            for (int x = 0; x < maze[y].Length; x++)
            {
                switch (maze[y][x])
                {
                    case 'S':
                        start = new(x, y);
                        break;
                    case 'E':
                    case '.':
                        trackPoints.Add(new Point(x, y));
                        break;
                }
            }
        }

        if (!start.HasValue)
            throw new ArgumentException("No start point was found.", nameof(input));

        // calculate path indexes for track points, save in dictionary
        trackPathIndexes.Clear();
        trackPathIndexes.Add(start.Value, 0);

        Point last = start.Value;
        int index = 1;
        while (trackPoints.Count > 0)
        {
            Point neighbor = GetNeighbors(last)
                .Where(trackPoints.Contains)
                .Single();
            trackPathIndexes.Add(neighbor, index);
            trackPoints.Remove(neighbor);
            last = neighbor;
            index++;
        }
    }

    record struct Cheat(
        Point Start,
        Point End
    )
    {
        public readonly int ManhattanDistance()
        {
            return Math.Abs(End.X - Start.X) + Math.Abs(End.Y - Start.Y);
        }
    }

    HashSet<Cheat> GenerateShortCheats()
    {
        HashSet<Cheat> cheats = [];
        foreach (var track in trackPathIndexes)
        {
            foreach (
                var cheatStart in GetNeighbors(track.Key)
                    .Where(s => !trackPathIndexes.ContainsKey(s))
            )
            {
                foreach (
                    var cheatEnd in GetNeighbors(cheatStart)
                        .Where(
                            e => trackPathIndexes.GetValueOrDefault(e, -1)
                                > track.Value
                        )
                )
                {
                    cheats.Add(new Cheat(track.Key, cheatEnd));
                }
            }
        }

        return cheats;
    }

    int GetLengthSaved(Cheat cheat)
    {
        int beforeCheatIdx = trackPathIndexes[
            GetNeighbors(cheat.Start)
                .Where(trackPathIndexes.ContainsKey)
                .MinBy(point => trackPathIndexes[point])
        ];

        int afterCheatIdx = trackPathIndexes[cheat.End];

        // have to factor out the length of the cheat from the difference
        return afterCheatIdx - beforeCheatIdx - cheat.ManhattanDistance();
    }

    public object PartOne(string input)
    {
        ParseMaze(input);

        var cheatsAbove = GenerateShortCheats()
            .Where(cheat => GetLengthSaved(cheat) >= minimumSteps)
            .Count();

        return cheatsAbove;
    }

    static HashSet<Point> GetDeepNeighbors(Point point, int depth)
    {
        HashSet<Point> neighbors = [];
        for (int distance = depth; distance > 0; distance--)
        {
            int crossDistance = depth - distance;

            /*
            The idea is that the first line starts at (D,0) then loops to draw
            diagonally for (D-i,i) up to (1,D-1) which will complete a fourth
            of the perimeter.
            Subsequent lines would traverse (0,D) to (-D+1,1), (-D,0) to
            (-1,-D+1), and (0,-D) to (D-1,-1).
            */
            neighbors.Add(point + new Size(distance, crossDistance));
            neighbors.Add(point + new Size(-crossDistance, distance));
            neighbors.Add(point + new Size(-distance, -crossDistance));
            neighbors.Add(point + new Size(crossDistance, -distance));
        }
        return neighbors;
    }

    HashSet<Cheat> GenerateLongCheats()
    {
        HashSet<Cheat> cheats = [];
        foreach (var track in trackPathIndexes)
        {
            for (int depth = 1; depth <= 20; depth++)
            {
                // NB cheat paths could backtrack away from the perimeter
                //  described by GetDeepNeighbors, but the sweep from
                //  depth 1 outward should cover those cases
                foreach (
                    var cheatEnd in GetDeepNeighbors(track.Key, depth)
                        .Where(
                            e => trackPathIndexes.GetValueOrDefault(e, -1)
                                > track.Value
                        )
                )
                {
                    cheats.Add(new Cheat(track.Key, cheatEnd));
                }
            }
        }

        return cheats;
    }

    public object PartTwo(string input)
    {
        ParseMaze(input);

        var cheatsAboveQuery = GenerateLongCheats()
            .Where(cheat => GetLengthSaved(cheat) >= minimumSteps);
        var cheatsAbove = cheatsAboveQuery.Count();

        return cheatsAbove;
    }
}
