using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day16;

/// <summary>
/// Solution code for Day 16: Reindeer Maze
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 16: Reindeer Maze"; }
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

    private string[] maze = [];
    private Point start;
    private Point end;
    private readonly HashSet<Point> openSpaces = [];

    private void ParseMaze(string input)
    {
        openSpaces.Clear();

        maze = input.Split('\n');
        for (int y = 0; y < maze.Length; y++)
        {
            for (int x = 0; x < maze[y].Length; x++)
            {
                switch (maze[y][x])
                {
                    case 'S':
                        start = new Point(x, y);
                        break;
                    case 'E':
                        end = new Point(x, y);
                        break;
                    case '.':
                        openSpaces.Add(new Point(x, y));
                        break;
                    case '#':
                        break;
                    default:
                        throw new ArgumentException(
                            $"Character '{maze[y][x]}' could not be handled, line {y + 1} column {x + 1}",
                            nameof(input)
                        );
                }
            }
        }
    }

    private enum Orientation
    {
        East,
        South,
        West,
        North
    }

    private static Orientation GetOrientation(Point from, Point to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        if (Math.Abs(dx) + Math.Abs(dy) != 1)
            throw new ArgumentException("Points must be adjacent.");

        if (dx == 1) return Orientation.East;
        if (dx == -1) return Orientation.West;
        if (dy == 1) return Orientation.South;
        return Orientation.North;
    }

    private static Orientation ClockwiseOrientation(Orientation orientation)
    {
        return orientation switch
        {
            Orientation.East => Orientation.South,
            Orientation.South => Orientation.West,
            Orientation.West => Orientation.North,
            Orientation.North => Orientation.East,
            _ => throw new ArgumentException(
                $"Could not rotate given Orientation: {orientation}.",
                nameof(orientation)
            ),
        };
    }

    private static int GetMinimumTurns(Orientation from, Orientation to)
    {
        if (from == to) return 0;

        // convenient to rotate both until 'from' is a specific known value
        while (from != Orientation.East)
        {
            from = ClockwiseOrientation(from);
            to = ClockwiseOrientation(to);
        }

        return to switch
        {
            Orientation.South => 1,
            Orientation.West => 2,
            Orientation.North => 1,
            _ => throw new ArgumentException(
                $"Could not handle given Orientation: {to}.", nameof(to)
            ),
        };
    }

    // NB distance costs between points can get significantly different
    //  depending on the direction the first point was entered into, need
    //  it encoded
    private record struct PointEntered(Point Position, Orientation EntryDirection);

    private static PointEntered[] GenerateAllDirections(Point position)
    {
        return [
            new (position, Orientation.East),
            new (position, Orientation.South),
            new (position, Orientation.West),
            new (position, Orientation.North),
        ];
    }

    private static
    PointEntered GenerateNeighborPointEntered(Point position, Orientation orientation)
    {
        var neighborPosition = orientation switch
        {
            Orientation.East => position + new Size(1, 0),
            Orientation.South => position + new Size(0, 1),
            Orientation.West => position + new Size(-1, 0),
            Orientation.North => position + new Size(0, -1),
            _ => throw new ArgumentException(
                $"Could not handle given Orientation: {orientation}.",
                nameof(orientation)
            ),
        };
        return new(neighborPosition, orientation);
    }

    private int DijkstraSearch()
    {
        Dictionary<PointEntered, int> distance =
            new((openSpaces.Count + 2) * 4) {
                {new (start, Orientation.East), 0}
            };
        Dictionary<PointEntered, PointEntered> previous =
            new((openSpaces.Count + 2) * 4);

        int GetDistanceDefaultMax(PointEntered pe) =>
            distance.GetValueOrDefault(pe, int.MaxValue);

        HashSet<PointEntered> toVisit = new((openSpaces.Count + 2) * 4) {
            new (start, Orientation.East),
        };
        toVisit.UnionWith(openSpaces.SelectMany(GenerateAllDirections));
        toVisit.UnionWith(GenerateAllDirections(end));

        while (toVisit.Count > 0)
        {
            var next = toVisit.MinBy(GetDistanceDefaultMax);
            // don't continue if we reached the target
            if (next.Position == end) break;
            toVisit.Remove(next);

            var nextDist = GetDistanceDefaultMax(next);

            var neighbors = toVisit.Intersect([
                GenerateNeighborPointEntered(next.Position, Orientation.East),
                GenerateNeighborPointEntered(next.Position, Orientation.South),
                GenerateNeighborPointEntered(next.Position, Orientation.West),
                GenerateNeighborPointEntered(next.Position, Orientation.North),
            ]);
            foreach (var neighbor in neighbors)
            {
                int cost = 1
                    + GetMinimumTurns(
                        next.EntryDirection, neighbor.EntryDirection
                    ) * 1000;

                var dist = nextDist + cost;
                // worried about overflow to negative, checking it's positive
                if (dist >= 0 && dist < GetDistanceDefaultMax(neighbor))
                {
                    distance[neighbor] = dist;
                    previous[neighbor] = next;
                }
            }
        }

        PointEntered? endPE = distance.Keys.Single(k => k.Position == end);
        if (endPE.HasValue)
        {
            /*
            HashSet<Point> minimumPath = [end];
            PointEntered last = endPE.Value;
            while (previous.TryGetValue(last, out var prev))
            {
                minimumPath.Add(prev.Position);
                last = prev;
            }
            DebugPrint(minimumPath);
            Console.WriteLine("length: {0}", minimumPath.Count);
            */
            return distance[endPE.Value];
        }
        else
        {
            throw new Exception("Failed to reach end.");
        }
    }

    private void DebugPrint(HashSet<Point> minimumPath)
    {
        for (int y = 0; y < maze.Length; y++)
        {
            for (int x = 0; x < maze[y].Length; x++)
            {
                if (maze[y][x] == '#')
                {
                    Console.Write('█');
                }
                else if (minimumPath.Contains(new Point(x, y)))
                {
                    Console.Write('O');
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
    }

    public object PartOne(string input)
    {
        ParseMaze(input);

        var result = DijkstraSearch();

        return result;
        // TODO taking 20s to run
    }

    public object PartTwo(string input)
    {
        return "TODO implement";
    }
}
