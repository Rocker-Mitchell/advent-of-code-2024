using System.Collections.Immutable;
using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day06;

enum Direction
{
    North,
    East,
    South,
    West
}

static class DirectionExtensions
{
    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };
    }
}

readonly struct PointDirection(Point point, Direction direction)
{
    public Point P { get; init; } = point;
    public Direction D { get; init; } = direction;
}

/// <summary>
/// Solution code for Day 6: Guard Gallivant
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 6: Guard Gallivant"; }
    }

    private static
    (Point bounds, Point guard, ImmutableHashSet<Point> obstacles)
    ParseInput(string input)
    {
        var lines = input.Split('\n');
        Point bounds = new(lines[0].Length, lines.Length);

        Point? guard = null;
        HashSet<Point> obstacles = [];

        for (int row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (int col = 0; col < line.Length; col++)
            {
                char pos = line[col];

                switch (pos)
                {
                    case '^':
                        guard = new Point(col, row);
                        break;
                    case '#':
                        obstacles.Add(new Point(col, row));
                        break;
                }
            }
        }

        if (!guard.HasValue)
        {
            throw new ArgumentException("input did not contain a guard ('^' char)");
        }

        return (bounds, guard.Value, obstacles.ToImmutableHashSet());
    }

    private static readonly
    ImmutableDictionary<Direction, Size>
    directionVectors = ImmutableDictionary.CreateRange(
        [
            KeyValuePair.Create(Direction.North, new Size(0,-1)),
            KeyValuePair.Create(Direction.East, new Size(1,0)),
            KeyValuePair.Create(Direction.South, new Size(0,1)),
            KeyValuePair.Create(Direction.West, new Size(-1,0))
        ]
    );

    private static Direction GetNextGuardDirection(
        Point currentPosition,
        Direction currentDirection,
        ImmutableHashSet<Point> obstacles
    )
    {
        var nextDirection = currentDirection;

        while (
            directionVectors.TryGetValue(nextDirection, out var directionVector)
            && obstacles.Contains(currentPosition + directionVector)
        )
        {
            nextDirection = nextDirection.TurnRight();

            if (nextDirection == currentDirection)
            {
                throw new Exception(
                    "Completed a full rotation without a valid direction to move"
                );
            }
        }

        return nextDirection;
    }

    private static HashSet<Point> FindVisited(
        Point bounds,
        Point guard,
        ImmutableHashSet<Point> obstacles
    )
    {
        HashSet<Point> visited = [];

        Direction direction = Direction.North;

        // calculate the guard's motion while in bounds
        while (
            guard.X >= 0 && guard.X < bounds.X
            && guard.Y >= 0 && guard.Y < bounds.Y
        )
        {
            visited.Add(guard);

            // walk the guard, handling obstacles
            direction = GetNextGuardDirection(guard, direction, obstacles);
            guard += directionVectors.GetValueOrDefault(direction);
        }

        return visited;
    }

    public object PartOne(string input)
    {
        var (bounds, guard, obstacles) = ParseInput(input);

        var visited = FindVisited(bounds, guard, obstacles);

        return visited.Count;
    }

    private static bool WillLoop(
        Point bounds,
        Point guard,
        ImmutableHashSet<Point> obstacles,
        Point newObstacle
    )
    {
        ImmutableHashSet<Point> modifiedObstacles = [.. obstacles, newObstacle];

        HashSet<PointDirection> visited = [];

        Direction direction = Direction.North;

        while (
            guard.X >= 0 && guard.X < bounds.X
            && guard.Y >= 0 && guard.Y < bounds.Y
        )
        {
            if (!visited.Add(new PointDirection(guard, direction)))
            {
                // we closed a loop by revisiting a point in the same direction
                return true;
            }

            direction = GetNextGuardDirection(guard, direction, modifiedObstacles);
            guard += directionVectors.GetValueOrDefault(direction);
        }

        // guard walked out of map, failed to loop
        return false;
    }

    public object PartTwo(string input)
    {
        var (bounds, guard, obstacles) = ParseInput(input);

        // use the points originally visited as new obstacles to test, minus
        //  the guard's starting point
        var visited = FindVisited(bounds, guard, obstacles);
        visited.Remove(guard);

        int total = 0;
        foreach (var v in visited)
        {
            if (WillLoop(bounds, guard, obstacles, v))
            {
                total++;
            }
        }

        return total;
    }
}
