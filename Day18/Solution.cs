using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day18;

/// <summary>
/// Solution code for Day 18: RAM Run
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 18: RAM Run"; }
    }

    private static Size spaceSize = new(70, 70);

    private static bool IsInBounds(Point point)
    {
        return point.X >= 0 && point.X <= spaceSize.Width
            && point.Y >= 0 && point.Y <= spaceSize.Height;
    }

    private Point[] corruptionOrder = [];

    private void ParseCorruptionOrder(string input)
    {
        var pointsQuery = input.Split('\n').Select(line =>
        {
            var values = line.Split(',');
            return new Point(int.Parse(values[0]), int.Parse(values[1]));
        });
        corruptionOrder = pointsQuery.ToArray();
    }

    private static int DijkstraSearch(HashSet<Point> corruptedPoints)
    {
        Point start = new(0, 0), end = new(spaceSize.Width, spaceSize.Height);

        Size[] neighborOffsets = [
            new(1,0),
            new(-1,0),
            new(0,1),
            new(0,-1),
        ];

        Dictionary<Point, int> distance =
            new(spaceSize.Width * spaceSize.Height)
            {
                { start, 0 }
            };
        int GetDistanceDefaultMax(Point p) =>
            distance.GetValueOrDefault(p, int.MaxValue);

        PriorityQueue<Point, int> queue = new();

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            Point next = queue.Dequeue();
            if (next == end) break;

            var neighbors = neighborOffsets.Select(s => next + s).Where(
                n => IsInBounds(n) && !corruptedPoints.Contains(n)
            );
            foreach (Point neighbor in neighbors)
            {
                var neighborDist = distance[next] + 1;
                if (neighborDist < GetDistanceDefaultMax(neighbor))
                {
                    distance[neighbor] = neighborDist;
                    queue.Enqueue(neighbor, neighborDist);
                }
            }
        }

        return distance[end];
    }

    public object PartOne(string input)
    {
        ParseCorruptionOrder(input);

        var firstKilobyte = corruptionOrder[..1024].ToHashSet();
        var steps = DijkstraSearch(firstKilobyte);

        return steps;
    }

    public object PartTwo(string input)
    {
        ParseCorruptionOrder(input);

        bool CanSolve(int corruptionLength)
        {
            var corruptedPoints =
                corruptionOrder[..corruptionLength].ToHashSet();
            try
            {
                DijkstraSearch(corruptedPoints);
            }
            catch
            {
                return false;
            }
            return true;
        }

        // binary search, though starting left at 1024 since part 1 proved
        //  that as solvable
        int left = 1024, right = corruptionOrder.Length - 1, middle;
        while (left <= right)
        {
            middle = (left + right) / 2;
            if (CanSolve(middle))
            {
                left = middle + 1;
            }
            else
            {
                right = middle - 1;
            }
        }
        middle = (left + right) / 2;
        var firstUnsolvableIndex = CanSolve(middle) ? middle : middle - 1;

        var firstUnsolvablePoint = corruptionOrder[firstUnsolvableIndex];
        return $"{firstUnsolvablePoint.X},{firstUnsolvablePoint.Y}";
    }
}
