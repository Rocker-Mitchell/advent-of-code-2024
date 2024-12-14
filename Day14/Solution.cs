using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day14;

/// <summary>
/// Solution code for Day 14: Restroom Redoubt
/// </summary>
partial class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 14: Restroom Redoubt"; }
    }

    private record struct Robot(Point Position, Size Velocity)
    {
        public readonly Point GetProjectedPosition(int seconds)
        {
            var offset = Velocity * seconds;
            return Position + offset;
        }
    }

    private Robot[] robots = [];

    [GeneratedRegex(@"p=(\S+) v=(\S+)")]
    private static partial Regex RobotRegex();

    private static Robot ParseRobot(string line)
    {
        var match = RobotRegex().Match(line);
        var position = match.Groups[1].Value.Split(',');
        var velocity = match.Groups[2].Value.Split(',');
        return new Robot(
            new Point(int.Parse(position[0]), int.Parse(position[1])),
            new Size(int.Parse(velocity[0]), int.Parse(velocity[1]))
        );
    }

    private void ParseRobots(string input)
    {
        var robotsQuery =
            from line in input.Split('\n')
            select ParseRobot(line);
        robots = robotsQuery.ToArray();
    }

    private static readonly Size roomSize = new(101, 103);

    private static Point ScreenWrap(Point position)
    {
        // use the remainder of dividing by room size as the wrapped position
        Point wrappedPosition = new(
            position.X % roomSize.Width,
            position.Y % roomSize.Height
        );

        // handle negative values by incrementing with room size
        if (wrappedPosition.X < 0)
            wrappedPosition.X += roomSize.Width;

        if (wrappedPosition.Y < 0)
            wrappedPosition.Y += roomSize.Height;

        return wrappedPosition;
    }

    private Point[] CalculateFuturePositions(int seconds = 100)
    {
        var futurePositionQuery =
            from robot in robots
            select ScreenWrap(robot.GetProjectedPosition(seconds));
        return futurePositionQuery.ToArray();
    }

    private static int[] GetPositionQuadrantCounts(Point[] positions)
    {
        int[] quadrants = new int[4];

        var middle = roomSize / 2;
        foreach (var pos in positions)
        {
            if (pos.X < middle.Width && pos.Y < middle.Height)
                quadrants[0]++;
            else if (pos.X > middle.Width && pos.Y < middle.Height)
                quadrants[1]++;
            else if (pos.X < middle.Width && pos.Y > middle.Height)
                quadrants[2]++;
            else if (pos.X > middle.Width && pos.Y > middle.Height)
                quadrants[3]++;
        }

        return quadrants;
    }

    public object PartOne(string input)
    {
        ParseRobots(input);

        var futurePositions = CalculateFuturePositions();

        var counts = GetPositionQuadrantCounts(futurePositions);

        return counts.Aggregate(1, (a, b) => a * b);
    }

    private static string RoomString(Point[] robotPositions)
    {
        StringBuilder builder = new(roomSize.Width * roomSize.Height);

        var pointCountMap = robotPositions.Distinct().ToDictionary(
            pos => pos,
            pos => robotPositions.Count(rp => rp == pos)
        );

        for (int y = 0; y <= roomSize.Height; y++)
        {
            for (int x = 0; x <= roomSize.Width; x++)
            {
                Point testPoint = new(x, y);

                if (pointCountMap.TryGetValue(testPoint, out var count))
                    builder.Append(count);
                else
                    builder.Append('.');
            }
            builder.Append('\n');
        }

        return builder.ToString();
    }

    private int DurationForSuspectedTree()
    {
        // inspiration: https://www.reddit.com/r/adventofcode/comments/1he0asr/2024_day_14_part_2_why_have_fun_with_image/
        /*
        The positions along an axis should repeat within a number of steps
        equal to the relevant room dimension.
        
        The image apparently clusters many points together to make a solid
        area. This can be detected by measuring variance over steps then
        finding the lowest variance for an axis.

        The Chinese Remainder Theorem can be used to determine the time to
        satisfy both axes' best variances:
        
        t = bx (mod w) // bx as best x time
        t = by (mod h) // by as best y time

        t = bx + k * w

        bx + k * w = by (mod h)
        k = inverse(w) * (by - bx) (mod h)
            - `inverse(w)` equates to `w ^ -1 (mod h)`

        finally
        t = bx + (inverse(w) * (by - bx) (mod h)) * w
        */

        static double Variance(int[] values)
        {
            if (values.Length == 0)
                return 0.0;

            double average = (double)values.Sum() / values.Length;
            double variance = 0.0;
            foreach (var val in values)
            {
                variance += Math.Pow(val - average, 2.0);
            }
            return variance / values.Length;
        }

        static (double xVariance, double yVariance) PointVariance(Point[] points)
        {
            int[] xValues = points.Select(p => p.X).ToArray(),
                yValues = points.Select(p => p.Y).ToArray();

            return (
                xVariance: Variance(xValues),
                yVariance: Variance(yValues)
            );
        }

        var variances = (
            from time in Enumerable.Range(0, Math.Max(roomSize.Width, roomSize.Height))
            select PointVariance(CalculateFuturePositions(time))
        ).ToArray();

        int bestXTime = 0, bestYTime = 0;
        double bestXVariance = variances[0].xVariance,
            bestYVariance = variances[0].yVariance;
        for (int time = 0; time < variances.Length; time++)
        {
            var (xVariance, yVariance) = variances[time];

            if (xVariance < bestXVariance)
            {
                bestXTime = time;
                bestXVariance = xVariance;
            }

            if (yVariance < bestYVariance)
            {
                bestYTime = time;
                bestYVariance = yVariance;
            }
        }

        // NB floating point errors are a pain, `Math.Pow(w, -1) % h` just
        //  doesn't work. I got told by an LLM this can be worked around with
        //  the modular multiplicative inverse & the extended Euclidean
        //  algorithm. This results in the working expression
        //  `ModularInverse(w, h) % h` being the substitution.
        static int ModularInverse(int a, int modulo)
        {
            int modulo0 = modulo;
            int y = 0, x = 1;

            if (modulo == 1)
                return 0;

            while (a > 1)
            {
                int quotient = a / modulo;
                
                int temp = modulo;
                // modulo is remainder, process as Euclid's GCD
                modulo = a % modulo;
                a = temp;

                temp = y;
                y = x - quotient * y;
                x = temp;
            }

            // handle negative x
            if (x < 0)
                x += modulo0;

            return x;
        }

        int k =
            ModularInverse(roomSize.Width, roomSize.Height)
            % roomSize.Height
            * (bestYTime - bestXTime)
            % roomSize.Height;
        // NB because we'd prefer the k coefficient is positive for a positive
        //  return value
        if (k < 0)
            k += roomSize.Height;

        return bestXTime + k * roomSize.Width;
    }

    public object PartTwo(string input)
    {
        ParseRobots(input);

        var suspectTime = DurationForSuspectedTree();

        var futurePositions = CalculateFuturePositions(suspectTime);
        var roomString = RoomString(futurePositions);
        Console.Write(roomString);

        return suspectTime;
    }
}
