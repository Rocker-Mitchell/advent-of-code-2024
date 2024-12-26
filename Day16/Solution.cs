using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day16;

using DistanceMap = Dictionary<Step, int>;

/// <summary>
/// The representation of a step in the maze.
/// </summary>
/// <param name="Position">The position in the maze.</param>
/// <param name="OffsetDirection">
/// A relative offset that reflects the direction being faced.
/// </param>
record struct Step(Point Position, Size OffsetDirection)
{
    public readonly Step Reverse() => new(Position, OffsetDirection * -1);

    public readonly IEnumerable<Step> NextSteps()
    {
        // the step continuing in the direction of the offset
        yield return new Step(Position + OffsetDirection, OffsetDirection);
        // the steps turning 90 degrees to the left and right, without moving
        Size offsetLeft = new(OffsetDirection.Height, -OffsetDirection.Width);
        yield return new Step(Position, offsetLeft);
        Size offsetRight = new(-OffsetDirection.Height, OffsetDirection.Width);
        yield return new Step(Position, offsetRight);
    }

    public readonly int PointsToNextStep(Step next)
    {
        var points = 0;
        // one point for a change in position
        if (Position != next.Position)
        {
            points++;
        }
        // if we are turning 90 degrees either direction, add 1000 points
        if (OffsetDirection != next.OffsetDirection)
        {
            points += 1000;
        }
        return points;
    }
}

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

    private Point _start;
    private Point _end;
    private readonly HashSet<Point> _openSpaces = [];

    private void ParseMaze(string input)
    {
        _openSpaces.Clear();

        var maze = input.Split('\n');
        for (int y = 0; y < maze.Length; y++)
        {
            for (int x = 0; x < maze[y].Length; x++)
            {
                switch (maze[y][x])
                {
                    case 'S':
                        _start = new Point(x, y);
                        break;
                    case 'E':
                        _end = new Point(x, y);
                        break;
                    case '.':
                        _openSpaces.Add(new Point(x, y));
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

    private DistanceMap DijkstraSearch(Step start)
    {
        var distances = new DistanceMap();
        var queue = new PriorityQueue<Step, int>();

        distances[start] = 0;
        queue.Enqueue(start, 0);

        while (queue.TryDequeue(out var current, out var totalDistance))
        {
            foreach (
                var next in current
                    .NextSteps()
                    .Where(n =>
                        n.Position == _start
                        || n.Position == _end
                        || _openSpaces.Contains(n.Position)
                    )
            )
            {
                var nextDistance = totalDistance + current.PointsToNextStep(next);
                if (nextDistance < distances.GetValueOrDefault(next, int.MaxValue))
                {
                    distances[next] = nextDistance;
                    queue.Enqueue(next, nextDistance);
                }
            }
        }

        return distances;
    }

    private int ShortestEndDistance(DistanceMap distancesFromStart)
    {
        // the shortest step on end may have been offset from any direction,
        //  so we filter for the end position then pick the one with the
        //  shortest distance
        return distancesFromStart.Where(kvp => kvp.Key.Position == _end).Min(kvp => kvp.Value);
    }

    // we start facing east, so offset moves to the right
    private Step StartingStep() => new(_start, new Size(1, 0));

    public object PartOne(string input)
    {
        ParseMaze(input);

        var distancesFromStart = DijkstraSearch(StartingStep());

        return ShortestEndDistance(distancesFromStart);
    }

    public object PartTwo(string input)
    {
        ParseMaze(input);

        var distancesFromStart = DijkstraSearch(StartingStep());

        // find the single step on the end that's the shortest distance, then
        //  search from that step (reversed in direction)
        var shortestDistance = ShortestEndDistance(distancesFromStart);
        var shortestEndStep = distancesFromStart
            .Single(kvp => kvp.Key.Position == _end && kvp.Value == shortestDistance)
            .Key;
        var distancesFromEnd = DijkstraSearch(shortestEndStep.Reverse());

        // we can evaluate for a step the distances to the start and end, so
        //  any that sum to equal the shortest distance should be a step on on
        //  one of the many possible shortest paths; steps to turn may lead to
        //  multiple instances with the same position, so pick out the
        //  distinct ones
        return distancesFromStart
            .Where(kvp =>
                distancesFromEnd.ContainsKey(kvp.Key.Reverse())
                && kvp.Value + distancesFromEnd[kvp.Key.Reverse()] == shortestDistance
            )
            .Select(kvp => kvp.Key.Position)
            .Distinct()
            .Count();
    }
}
