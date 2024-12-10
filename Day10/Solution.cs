using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day10;

record struct Position(int Row, int Col);

/// <summary>
/// Solution code for Day 10: Hoof It
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 10: Hoof It"; }
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

    private byte[,] map = { };

    private void ParseMap(string input)
    {
        var lines = input.Split('\n');

        map = new byte[lines.Length, lines[0].Length];

        for (int row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (int col = 0; col < line.Length; col++)
            {
                var parseNum = char.GetNumericValue(line[col]);
                if (parseNum < 0)
                    throw new ArgumentException(
                        $"Failed to parse number from character: {line[col]}",
                        nameof(input)
                    );

                map[row, col] = (byte)parseNum;
            }
        }
    }

    private bool IsInBounds(Position pos)
    {
        return pos.Row >= 0 && pos.Row < map.GetLength(0)
            && pos.Col >= 0 && pos.Col < map.GetLength(1);
    }

    private byte GetHeight(Position pos)
    {
        return map[pos.Row, pos.Col];
    }

    private HashSet<Position> GetPossibleTrailHeads()
    {
        HashSet<Position> trailHeads = [];

        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col] == 0)
                {
                    trailHeads.Add(new Position(row, col));
                }
            }
        }

        return trailHeads;
    }

    private List<Position[]> GetTrailPaths(Position trailHead)
    {
        if (!IsInBounds(trailHead))
            throw new ArgumentException(
                "The trail head is expected to be within bounds",
                nameof(trailHead)
            );

        if (GetHeight(trailHead) != 0)
            throw new ArgumentException(
                $"The trail head height is expected to be 0: {GetHeight(trailHead)}",
                nameof(trailHead)
            );

        List<Position[]> RecursiveSearch(List<Position> partialPath)
        {
            var last = partialPath[^1];
            var lastHeight = GetHeight(last);

            // base case
            if (lastHeight == 9)
                return [[.. partialPath]];

            List<Position[]> foundPaths = [];

            var neighbors = new Position[] {
                new(last.Row, last.Col + 1),
                new(last.Row, last.Col - 1),
                new(last.Row + 1, last.Col),
                new(last.Row - 1, last.Col),
            };
            foreach (var neighbor in neighbors)
            {
                if (
                    IsInBounds(neighbor)
                    && !partialPath.Contains(neighbor)
                    && GetHeight(neighbor) == lastHeight + 1
                )
                {
                    // add the neighbor for the recursive step, then revert
                    partialPath.Add(neighbor);
                    foundPaths.AddRange(RecursiveSearch(partialPath));
                    partialPath.RemoveAt(partialPath.Count - 1);
                }
            }

            return foundPaths;
        }

        return RecursiveSearch(new List<Position>(10) { trailHead });
    }

    private HashSet<Position> GetTrailEnds(Position trailHead)
    {
        var trailPaths = GetTrailPaths(trailHead);

        return trailPaths.Select(tp => tp[^1]).ToHashSet();
    }

    public object PartOne(string input)
    {
        ParseMap(input);

        var trailHeads = GetPossibleTrailHeads();

        int totalScores = 0;

        foreach (var trailHead in trailHeads)
        {
            var trailEnds = GetTrailEnds(trailHead);
            totalScores += trailEnds.Count;
        }

        return totalScores;
    }

    public object PartTwo(string input)
    {
        ParseMap(input);

        var trailHeads = GetPossibleTrailHeads();

        int totalRatings = 0;

        foreach (var trailHead in trailHeads)
        {
            var trailPaths = GetTrailPaths(trailHead);
            totalRatings += trailPaths.Count;
        }

        return totalRatings;
    }
}
